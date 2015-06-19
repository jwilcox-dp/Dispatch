using System;
using System.Diagnostics; 
using System.Collections.Generic;
using System.Windows.Forms;
using DNACircSynchronizer.Processes.DSSISTableAdapters;
using DNACircSynchronizer.Processes.DSDispatchTableAdapters;
using DNACircSynchronizer.Processes.DSCJTableAdapters;
using DNACircSynchronizer.Processes;
using System.Data;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Configuration;
using System.Threading;



namespace DNACircSynchronizer.Processes
{
    public class Synchronizer
    {
        static DateTime lastCJToSISRunTime; 
        static int timeDiscrepancy = 15;
        // Connection pooling can fail due to memory leaks (extremely rare, but does seem to happen) or database mirror failover. 
        // Exception handling will catch SQL Server errors and clear the connection pool in an attempt to fix the problem.  
        // In order to prevent a race condition, we don't want to clear the connection pool more than once every 2 minutes, the following
        // flag holds the last time we cleared the connection pool, to prevent the race condition.
        static DateTime _lastConnectionPoolClearing = DateTime.MinValue; 
        static int sleepTime = 60000;
        static int sleepTimeCJ = 300000;
        static string SISConnectionString;
        static string DispatchReaderConnectionString;
        static string DispatchWriterConnectionString;
        static string CJConnectionString;
        static string synchronizer = "Synchronizer";


		private static EventLog _eventLog = new EventLog("Application");

        static Synchronizer()
        {
            _eventLog.Source = "DNASynchronizer 1.0.0.0"; 
            _eventLog.WriteEntry("Synchronizer Constructor Start", EventLogEntryType.Information);
           

            lastCJToSISRunTime = DateTime.Now;

            try 
            {
                if (ConfigurationManager.AppSettings["CJvsDispatchTimeDiscrepancy"] != null)
                    Int32.TryParse(ConfigurationManager.AppSettings["CJvsDispatchTimeDiscrepancy"], out timeDiscrepancy); 

                if (ConfigurationManager.AppSettings["DotNetSleepTime"] != null)
                    Int32.TryParse(ConfigurationManager.AppSettings["DotNetSleepTime"], out sleepTime);

                if (ConfigurationManager.AppSettings["CJSleepTime"] != null)
                    Int32.TryParse(ConfigurationManager.AppSettings["CJSleepTime"], out sleepTimeCJ);

                if (ConfigurationManager.AppSettings["Synchronizer"] != null &&
                    ConfigurationManager.AppSettings["Synchronizer"].Trim().Length > 0)
                    synchronizer = ConfigurationManager.AppSettings["Synchronizer"];

                SISConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DNACircSynchronizer.Processes.Properties.Settings.SISConnectionString"].ConnectionString;
                DispatchReaderConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DNACircSynchronizer.Processes.Properties.Settings.DispatchReaderConnectionString"].ConnectionString;
                DispatchWriterConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DNACircSynchronizer.Processes.Properties.Settings.DispatchWriterConnectionString"].ConnectionString;
                CJConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DNACircSynchronizer.Processes.Properties.Settings.CJConnectionString"].ConnectionString;
            }
            catch (Exception ex)
            {
                _eventLog.WriteEntry("Synchronizer Constructor Exception: " + ex.ToString(), EventLogEntryType.Information);

                string emailBody = string.Format("Synchronizer Constructor failed with the following exception: {0} {2} Stacktrace: {1}",
                    ex.Message, ex.StackTrace, Utility.NewLine);
                Email.ReportErrorToProgrammers("Dispatch Synchronizer Failure", emailBody);
            }
            _eventLog.WriteEntry("Synchronizer Constructor End", EventLogEntryType.Information);
        }

        /// <summary>
        /// 1) Get up to 10 rows from the SIS database DispatchRequest table. The LockDate 
        /// on these rows will be set to the current DateTime stamp until the synchronization 
        /// is complete.
        /// 2) Insert up to 10 rows that had been read the SIS DispatchRequest table into 
        /// Dispatch database DispatchRequest table.
        /// 3) Update the up to 10 rows from the SIS database DispatchRequest table that have been 
        /// 'synchronized' with the Dispatch database with the 'TransferredToDispatch' flag (Time Stamp).
        /// </summary>
        public static void SynchronizeSISToDispatchTables()
        {
            SqlTransaction sqlTransactionForSIS = null;
            SqlTransaction sqlTransactionForDispatch = null;
            SISDispatchRequestTableAdapter forLoadAndUpdate = new SISDispatchRequestTableAdapter();
            DispatchRequestTableAdapter forInsert = new DispatchRequestTableAdapter(); 
            DSSIS dsSIS;
            bool sleep = true;
            bool isSuccess = true;

            DataSQLUtility sisSave = null;
            DataSQLUtility dispatchSave = null;
            SqlConnection sisConnection = null;
            SqlConnection dispatchConnection = null;
            string additionalLogInfo = string.Empty; 

            while (true)
            {
                try
                {
                    sisSave = new DataSQLUtility("Get SIS Complaints to transfer to Dispatch", DataSQLUtility.AccessType.Write);
                    dispatchSave = new DataSQLUtility("Insert SIS Dispatch Rows (for Complaints) into Dispatch Database", DataSQLUtility.AccessType.Write);

                    // Create Dispatches for Dispatch Complaints
                    sqlTransactionForSIS = sisSave.StartSQLTransaction("SISTransaction", "DNACircSynchronizer.Processes.Properties.Settings.SISConnectionString");
                    sqlTransactionForDispatch = dispatchSave.StartSQLTransaction("DispatchComplaints", "DNACircSynchronizer.Processes.Properties.Settings.DispatchWriterConnectionString");
                    forLoadAndUpdate.SetTransaction(sqlTransactionForSIS);

                    dsSIS = GetSISDispatchComplaintsTransferToDispatch(forLoadAndUpdate);

                    sleep = true;
                    isSuccess = true;
                    additionalLogInfo = string.Format("{0} SIS Complaints found to copy to the Dispatch DB", dsSIS.SISDispatchRequest.Count);

                    if (dsSIS.SISDispatchRequest.Count > 0)
                    {
                        _eventLog.WriteEntry(string.Format("SynchronizeSISToDispatchTables() found {0} SIS Dispatch Complaints to copy to Dispatch", dsSIS.SISDispatchRequest.Count), EventLogEntryType.Information);

                        forInsert.SetTransaction(sqlTransactionForDispatch);
                        isSuccess = InsertSISDispatchRowsIntoDispatch(dsSIS.SISDispatchRequest, forInsert);
                        if (isSuccess)
                            isSuccess = SetSISDispatchRequestToTransferredToDispatch(dsSIS.SISDispatchRequest,
                                                                                    forLoadAndUpdate);
                        sleep = false;
                    }
                    else
                    {
                        _eventLog.WriteEntry(string.Format("SynchronizeSISToDispatchTables() found NO SIS Dispatch Complaints to copy to Dispatch"), EventLogEntryType.Information);
                    }


                    if (isSuccess)
                    {
                        sisSave.CommitTransaction(sqlTransactionForSIS, additionalLogInfo);
                        dispatchSave.CommitTransaction(sqlTransactionForDispatch);
                    }
                    else
                    {
                        sisSave.RollbackTransaction(sqlTransactionForSIS, additionalLogInfo + " *** Transaction was Rolled Back ***");
                        dispatchSave.RollbackTransaction(sqlTransactionForDispatch);
                    }

                    sisSave = new DataSQLUtility("Get SIS Memos to transfer to Dispatch", DataSQLUtility.AccessType.Write);
                    dispatchSave = new DataSQLUtility("Insert SIS Dispatch Rows (for Memos) into Dispatch Database", DataSQLUtility.AccessType.Write);


                    // Create Dispatches for Dispatched Memos
                    sqlTransactionForSIS = sisSave.StartSQLTransaction("SISTransaction", "DNACircSynchronizer.Processes.Properties.Settings.SISConnectionString");
                    sqlTransactionForDispatch = dispatchSave.StartSQLTransaction("DispatchMemos", "DNACircSynchronizer.Processes.Properties.Settings.DispatchWriterConnectionString");
                    sisConnection = sqlTransactionForSIS.Connection;
                    dispatchConnection = sqlTransactionForDispatch.Connection;  

                    forLoadAndUpdate.SetTransaction(sqlTransactionForSIS);
                    dsSIS = GetSISDispatchMemosForTransferToDispatch(forLoadAndUpdate);

                    additionalLogInfo = string.Format("{0} SIS Memos found to copy to the Dispatch DB", dsSIS.SISDispatchRequest.Count);


                    if (dsSIS.SISDispatchRequest.Count > 0)
                    {
                        _eventLog.WriteEntry(string.Format("SynchronizeSISToDispatchTables() found {0} SIS Dispatch Memos to copy to Dispatch", dsSIS.SISDispatchRequest.Count), EventLogEntryType.Information);

                        sleep = false;
                        forInsert.SetTransaction(sqlTransactionForDispatch);
                        isSuccess = InsertSISDispatchRowsIntoDispatch(dsSIS.SISDispatchRequest, forInsert);
                        if (isSuccess)
                            isSuccess = SetSISDispatchRequestToTransferredToDispatch(dsSIS.SISDispatchRequest, forLoadAndUpdate);
                    }
                    else
                    {
                        _eventLog.WriteEntry(string.Format("SynchronizeSISToDispatchTables() found NO SIS Dispatch Memos to copy to Dispatch"), EventLogEntryType.Information);
                    }

                    if (isSuccess)
                    {
                        sisSave.CommitTransaction(sqlTransactionForSIS, additionalLogInfo);
                        dispatchSave.CommitTransaction(sqlTransactionForDispatch);
                    }
                    else
                    {
                        sisSave.RollbackTransaction(sqlTransactionForSIS, additionalLogInfo + " *** Transaction was rolled back ***");
                        dispatchSave.RollbackTransaction(sqlTransactionForDispatch);
                    }
                    sisSave = null;
                    dispatchSave = null; 
                }
                catch (SqlException ex)
                {
                    if (sqlTransactionForSIS != null && sisSave != null)
                        sisSave.RollbackTransaction(sqlTransactionForSIS, "Exception Encountered: " + ex.Message);
                    if (sqlTransactionForDispatch != null && sisSave != null)
                        dispatchSave.RollbackTransaction(sqlTransactionForDispatch, "Exception Encountered: " + ex.Message);

                    try
                    {
                        ProcessSQLException(ex);
                    }
                    catch (Exception ex2) { };
                    sleep = false; 
                }
                catch (Exception ex)
                {
                    _eventLog.WriteEntry(string.Format("SynchronizeSISToDispatchTables() Exception: {0}", ex.ToString()), EventLogEntryType.Information);

                    string emailBody = string.Format("SynchronizeSISToDispatchTables failed with the following exception: {0} {2} Stacktrace: {1}",
                        ex.Message, ex.StackTrace, Utility.NewLine);
                    Email.ReportErrorToProgrammers("Dispatch Synchronizer Failure", emailBody);
                    sleep = false;
                }
                if (sleep)
                {
                    // Sometimes new Complaint and Memos aren't found in a timely manner, only 
                    // restarting servies will fix the problem.  Try to fix that situation by 
                    // closing the connection pools when we sleep.  
                    if (sisConnection != null)
                        SqlConnection.ClearPool(sisConnection); 
                    if (dispatchConnection != null)
                        SqlConnection.ClearPool(dispatchConnection); 


                    Thread.Sleep(sleepTime);
                }
            }
        }


        /// <summary>
        /// 1) Get up to 10 rows from the Dispatch database DispatchRequest table that have been dispatched and delivered.
        /// The LockDate on these rows will be set to the current DateTime stamp until the synchronization 
        /// is complete.
        /// 2) Update up to 10 SIS DispatchRequest table rows with the Dispatched and Delivered dates to match
        /// the Dispatched and Delivered dates from the Dispatch database DispatchRequest table.
        /// 3) Update the up to 10 rows from the Dispatch database DispatchRequest table that have been 
        /// 'synchronized' with the SIS database with the 'TransferredToSIS' flag (Time Stamp).
        /// </summary>
        public static void SynchronizeDispatchToSISTables()
        {
            while (true)
            {
                bool sleep = true;  // Sleep if no records were found

                DataSQLUtility sisSave = new DataSQLUtility("Set the Redelivery flag in SIS' Dispatch for Complaints/Memos that have been Redelivered.", DataSQLUtility.AccessType.Write);
                DataSQLUtility dispatchSave = new DataSQLUtility("Get DISPATCH rows that need to be sent to SIS (need to set the Redeliver flag in the SIS database).", DataSQLUtility.AccessType.Write);
                CJUtility cjSave = new CJUtility("Set CJ's Service History Record(s) to Dispatched", CJUtility.AccessType.Write); 

                try 
                {
                    SqlTransaction sqlTransactionForSIS = sisSave.StartSQLTransaction("DispSISTransaction", "DNACircSynchronizer.Processes.Properties.Settings.SISConnectionString");
                    SqlTransaction sqlTransactionForDispatch = dispatchSave.StartSQLTransaction("DispDispatchTransaction", "DNACircSynchronizer.Processes.Properties.Settings.DispatchWriterConnectionString");
                    OdbcTransaction cjTransaction = cjSave.StartTransaction(); 

                    DispatchRequestTableAdapter forLoadAndUpdate = new DispatchRequestTableAdapter();
                    forLoadAndUpdate.SetTransaction(sqlTransactionForDispatch);

                    DSDispatch dsDispatch = GetDispatchDispatchRowsForTransferToSIS(forLoadAndUpdate);

                    bool isSuccess = false;
                    if (dsDispatch.DispatchRequest.Count > 0)
                    {
                        _eventLog.WriteEntry(string.Format("SynchronizeDispatchToSISTables() found {0} Dispatches to update in SIS", dsDispatch.DispatchRequest.Count), EventLogEntryType.Information);

                        // Set the CJ Service History record to Dispatched
                        isSuccess = SetCJServiceHistoryToDispatched(dsDispatch.DispatchRequest,
                            cjTransaction);

                        // IMPORTANT: The CJ Service History step must read the SIS.ComplaintHistory record that 
                        // is locked in the code below.  The order in which these two pieces of code are 
                        // executed is very important (must set the CJ Service History to Dispatched before 
                        // reconciling dipsatched rows back into SIS.
                        // Set the SIS.ComplaintHistory and SIS.DispatchRequest records to Disptached
                        if (isSuccess)
                        {
                            isSuccess = ReconcileDispatchedDispatchRowsIntoSIS(dsDispatch.DispatchRequest,
                                                       sqlTransactionForSIS);
                        }

                        // Set the Dispatch.DispatchRequest record to transferred back to SIS.
                        if (isSuccess)
                            isSuccess = SetDispatchDispatchRequestToTransferredToSIS(dsDispatch.DispatchRequest,
                                                            forLoadAndUpdate);

                        sleep = false;
                    }
                    else 
                    {
                        _eventLog.WriteEntry(string.Format("SynchronizeDispatchToSISTables() found NO Dispatches to update in SIS"), EventLogEntryType.Information);
                    }

                    if (isSuccess)
                    {
                        sisSave.CommitTransaction(sqlTransactionForSIS);
                        dispatchSave.CommitTransaction(sqlTransactionForDispatch);
                        cjSave.CommitTransaction(cjTransaction); 
                    }
                    else
                    {
                        sisSave.RollbackTransaction(sqlTransactionForSIS);
                        dispatchSave.RollbackTransaction(sqlTransactionForDispatch);
                        cjSave.CommitTransaction(cjTransaction); 
                    }
                }
                catch (SqlException ex)
                {
                    ProcessSQLException(ex);
                }
                catch (Exception ex)
                {
                    _eventLog.WriteEntry(string.Format("SynchronizeDispatchToSISTables() Exception: {0}", ex.ToString()), EventLogEntryType.Information);

                    string emailBody = string.Format("SynchronizeDispatchToSISTables failed with the following exception: {0} {2} Stacktrace: {1}",
                        ex.Message, ex.StackTrace, Utility.NewLine);
                    Email.ReportErrorToProgrammers("Dispatch Synchronizer Failure", emailBody);
                }

                if (sleep)
                {
                    OdbcConnection.ReleaseObjectPool(); 

                    Thread.Sleep(sleepTime);
                }
            }
        }

        /// <summary>
        /// SIS is the originator of 95% of complaints in CJ, but we need to make sure 100% of the complaints
        /// in CJ are in SIS.  This method reads the CJ Service History table for complaints that were created
        /// within the last X minutes, this method then determines if each complaint already exists in the 
        /// SIS ComplaintHistory table. For each complaint that doesn't exist in the SIS Complaint History table, 
        /// a web service to SIS is called to create that complaint in SIS.  
        /// 
        /// If the new complaint needs to be dispatched, the SynchronizeSISToDispatchTables thread will 
        /// copy the request from SIS into Dispatch and the complaint will then be dispatched.
        /// </summary>
        public static void SynchronizeCJToSISTables()
        {
            SISWebServices.Service SISServices = new SISWebServices.Service();

            DSSIS.RefComplaintTypeDataTable refComplaintTypes = new DSSIS.RefComplaintTypeDataTable();

            DSSISTableAdapters.RefComplaintTypeTableAdapter forRefFill = new RefComplaintTypeTableAdapter();
            forRefFill.Fill(refComplaintTypes);

            _eventLog.WriteEntry(string.Format("starting up SynchronizeCJToSISTables()", EventLogEntryType.Information));


            while (true)
            {
                try 
                {
                    DateTime thisRunStarted = DateTime.Now;
                    DSCJ dsCJ = GetCJComplaintRowsForTransferToSIS(thisRunStarted);

                    _eventLog.WriteEntry(string.Format("SynchronizeCJToSISTables() found {0} Complaints to reconcile in SIS", dsCJ.SERVICE_HISTORY.Count), EventLogEntryType.Information);

                    DSSIS dsSIS = new DSSIS();
                    DSSISTableAdapters.ComplaintHistoryTableAdapter forSISLoad = new ComplaintHistoryTableAdapter();

                    lastCJToSISRunTime = thisRunStarted;

                    foreach (DSCJ.SERVICE_HISTORYRow sh in dsCJ.SERVICE_HISTORY)
                    {
                        try 
                        {
                            bool chargeCarrier = false;
                            if (sh.CARRIER_CHARGE.Contains("Y") || sh.CARRIER_CHARGE.Contains("T"))
                                chargeCarrier = true;

                            string comment = sh.COMMENT;
                            string complaintCode = sh.SERVICE_ERROR_CD;
                            int creditDays = 0;
                            Int32.TryParse(sh.PBM_CREDIT, out creditDays);
                            DateTime dateEntered = Utility.FormatCJDateAndTimeToDateTime(sh.DATE_ENTERED, sh.TIME_ENTERED);

                            DateTime effectiveDate = Utility.FormatDateCJtoDateTime(sh.EFFECTIVE_DATE);
                            string addressKey = sh.ADDRESS_KEY;
                            string publicationCode = sh.PUBLICATION_CODE;
                            string sosEscalate = sh.SOS_TROUBLE;
                            string subscriptionNumber = sh.SUBSCRIBER_NUM;

                            // If complaint type doesn't exist in the reference table, then ignore it (for now).
                            if (refComplaintTypes.Select(string.Format("ComplaintCode = '{0}'", complaintCode)).Length == 0)
                                continue;

                            // See if we've already created that complaint in SIS
                            DataSQLUtility sqlReader = new DataSQLUtility("Find Complaint By CJ Data, AddressKey=" + addressKey, DataSQLUtility.AccessType.Read);
                            sqlReader.StartTimer(); 
                            forSISLoad.FindComplaintByCJData(dsSIS.ComplaintHistory,
                                complaintCode, addressKey, publicationCode, subscriptionNumber, effectiveDate);
                            sqlReader.StopTimer();
                            sqlReader = null; 

                            // If we didn't find the complaint, create it
                            if (dsSIS.ComplaintHistory.Count == 0)
                            {
                                _eventLog.WriteEntry(string.Format("SynchronizeCJToSISTables() Creating Complaint in SIS for: Address Key: {0}{4}Complaint Code: {1}{4}Date Entered: {2}{4}Effective Date: {3}",
                                    addressKey,
                                    complaintCode,
                                    dateEntered,
                                    effectiveDate,
                                    Utility.NewLine), EventLogEntryType.Information);

                                string results = SISServices.CreateComplaintInSISOnly(chargeCarrier, comment, complaintCode, creditDays, dateEntered, effectiveDate, addressKey, publicationCode, sosEscalate, subscriptionNumber);

                                if (results.ToUpper() != "SUCCESS")
                                {
                                    string emailBody = string.Format("Create Complaint in SIS (from Complaint in CJ) failed with error message: {5} {0} {5}Address Key: {1}{5}Complaint Code: {2}{5}Date Entered: {3}{5}Effective Date: {4}",
                                    results,
                                    addressKey,
                                    complaintCode,
                                    dateEntered,
                                    effectiveDate,
                                    Utility.NewLine);
                                    Email.SendEmail("MailTo", "MailFrom", "Dispatch Synchronizer Failure", emailBody, "SMTPMailServer");
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            ProcessSQLException(ex);
                        }
                        catch (Exception ex)
                        {
                            string emailBody = string.Format("Create Complaint in SIS (from Complaint in CJ) failed with Exception message: {6} {0} {6}Address Key: {1}{6}Complaint Code: {2}{6} Pub Code:{3}{6} Date Entered: {4}{6}Effective Date: {5}",
                                ex.Message,
                                sh.ADDRESS_KEY,
                                sh.SERVICE_CODE,
                                sh.PUBLICATION_CODE, 
                                sh.DATE_ENTERED,
                                sh.EFFECTIVE_DATE,
                                Utility.NewLine
                            );
                            Email.ReportErrorToProgrammers("Dispatch Synchronizer Failure", emailBody);
                        }
                    }


                    dsCJ = GetCJMemoRowsForTransferToSIS(thisRunStarted);

                    _eventLog.WriteEntry(string.Format("SynchronizeCJToSISTables() found {0} Memos to reconcile in SIS", dsCJ.SERVICE_HISTORY.Count), EventLogEntryType.Information);

                    DSSISTableAdapters.MemoHistoryTableAdapter forSISMemoLoad = new MemoHistoryTableAdapter();
                    lastCJToSISRunTime = thisRunStarted;
                    foreach (DSCJ.SERVICE_HISTORYRow sh in dsCJ.SERVICE_HISTORY)
                    {
                        try 
                        {
                            bool dispatchFlag = true;
                            if (sh.DISPATCHED_TIME == "9999")
                                dispatchFlag = false;


                            // According to Jeff, I should not dispatch any memo that came from CJ that has already been 
                            // posted.  This should prevent memos improperly created by the MATHER RATES process (which 
                            // are created with an empty DISPATCHED_TIME) from being dispatched.  
                            if (sh.POSTED_DATE.Trim().Length > 0)
                                dispatchFlag = false;

                            DateTime effectiveDate = Utility.FormatDateCJtoDateTime(sh.EFFECTIVE_DATE);
                            DateTime dateEntered = Utility.FormatCJDateAndTimeToDateTime(sh.DATE_ENTERED, sh.TIME_ENTERED);


                            // See if we've already created that Memo in SIS
                            DataSQLUtility sqlReader = new DataSQLUtility("Find Memo By CJ Data, AddressKey = " + sh.ADDRESS_KEY.Trim(), DataSQLUtility.AccessType.Read);
                            sqlReader.StartTimer(); 
                            forSISMemoLoad.FindMemoByCJData(dsSIS.MemoHistory, sh.ADDRESS_KEY.Trim(), sh.COMMENT.Trim(), effectiveDate, (dispatchFlag) ? "T" : "F", sh.SOS_TROUBLE.Trim(), sh.OPERATOR_ID.Trim());
                            sqlReader.StopTimer();
                            sqlReader = null; 

                            // If we didn't find the complaint, create it
                            if (dsSIS.MemoHistory.Count == 0)
                            {
                                _eventLog.WriteEntry(string.Format("SynchronizeCJToSISTables() Creating Memo in SIS for: Address Key: {0}{4} Memo Text: {1}{4}Date Entered: {2}{4}Effective Date: {3}",
                                    sh.ADDRESS_KEY,
                                    sh.COMMENT,
                                    sh.DATE_ENTERED,
                                    effectiveDate,
                                    Utility.NewLine), EventLogEntryType.Information);

                                string results = SISServices.CreateMemoInSISOnly(
                                    sh.COMMENT,
                                    dispatchFlag,
                                    false,
                                    false,
                                    sh.SOS_TROUBLE,
                                    dateEntered,
                                    effectiveDate,
                                    sh.ADDRESS_KEY,
                                    sh.OPERATOR_ID);


                                if (results.ToUpper() != "SUCCESS")
                                {
                                    string emailBody = string.Format("Create Memo in SIS (from Memo in CJ) failed with error message: {5} {0} {5}Address Key: {1}{5}Memo Text: {2}{5}Date Entered: {3}{5}Effective Date: {4}",
                                    results,
                                    sh.ADDRESS_KEY,
                                    sh.COMMENT,
                                    sh.DATE_ENTERED,
                                    sh.EFFECTIVE_DATE,
                                    Utility.NewLine);
                                    Email.SendEmail("MailTo", "MailFrom", "Dispatch Synchronizer Failure", emailBody, "SMTPMailServer");
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            ProcessSQLException(ex);
                        }
                        catch (Exception ex)
                        {
                            _eventLog.WriteEntry(string.Format("SynchronizeCJToSISTables() Exception: {0}", ex.ToString()), EventLogEntryType.Information);

                            string emailBody = string.Format("Create Memo in SIS (from Complaint in CJ) failed with Exception message: {5} {0} {5}Address Key: {1}{5}Memo Text: {2}{5}Date Entered: {3}{5}Effective Date: {4}",
                                ex.Message, 
                                sh.ADDRESS_KEY, 
                                sh.COMMENT, 
                                sh.DATE_ENTERED, 
                                sh.EFFECTIVE_DATE, 
                                Utility.NewLine
                            );
                           Email.ReportErrorToProgrammers("Dispatch Synchronizer Failure", emailBody);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    ProcessSQLException(ex);
                }
                catch (Exception ex)
                {
                    _eventLog.WriteEntry(string.Format("SynchronizeCJToSISTables() Exception: {0}", ex.ToString()), EventLogEntryType.Information);
                    string emailBody = string.Format("SynchronizeCJToSISTables failed with the following exception: {0} {2} Stacktrace: {1}",
                        ex.Message, ex.StackTrace, Utility.NewLine);
                    Email.ReportErrorToProgrammers("Dispatch Synchronizer Failure", emailBody);
                }


                // Default 5 minutes
                OdbcConnection.ReleaseObjectPool(); 
                Thread.Sleep(sleepTimeCJ);

            }
        }

        // SQL Exceptions are very rare, when they do happen it's usually due to a connection pool problem - either the 
        // connection pool is corrupted or a datbaase failover has occurred.  Both problems will hopefully be corrected 
        // by clearing the connection pool - so do it.  
        private static void ProcessSQLException(SqlException ex)
        {
            // In order to prevent a race condition, clear the connection pool at most every 2 minutes.  
            if (_lastConnectionPoolClearing.AddMinutes(2) < DateTime.Now)
            {
                SqlConnection.ClearAllPools();
                _lastConnectionPoolClearing = DateTime.Now;
                _eventLog.WriteEntry(string.Format("SynchronizeSISToDispatchTables() Exception: {0}", ex.ToString()), EventLogEntryType.Information);

                string emailBody = string.Format("SynchronizeSISToDispatchTables failed with a SQL Exception.  In an attempt to correct this problem the Connection Pools were cleared.  Exception information: {0} {2} Stacktrace: {1}",
                    ex.Message, ex.StackTrace, Utility.NewLine);
                Email.ReportErrorToProgrammers("Dispatch Synchronizer Failure, SQL Connection Pools were Cleared", emailBody);
            }
            else
            {
                _eventLog.WriteEntry(string.Format("SynchronizeSISToDispatchTables() SQL Exception: {0}", ex.ToString()), EventLogEntryType.Information);

                string emailBody = string.Format("SynchronizeSISToDispatchTables failed with the following SQL Exception: {0} {2} Stacktrace: {1}",
                    ex.Message, ex.StackTrace, Utility.NewLine);
                Email.ReportErrorToProgrammers("Dispatch Synchronizer Failure", emailBody);
            }
        }


        /// <summary>
        /// Will return up to 10 rows from the SIS database DispatchRequest table for Complaints
        /// that have not been transferred to the Dispatch database. 
        /// </summary>
        public static DSSIS GetSISDispatchComplaintsTransferToDispatch(SISDispatchRequestTableAdapter inForLoad)
        {
            DSSIS toReturn = new DSSIS();
            toReturn.EnforceConstraints = false; 

            inForLoad.GetDispatchRequestComplaintsForTransferFromSISToDispatch(toReturn.SISDispatchRequest, synchronizer);

            return toReturn;
        }

        /// <summary>
        /// Will return up to 10 rows from the SIS database DispatchRequest table for Memos
        /// that have not been transferred to the Dispatch database. 
        /// </summary>
        public static DSSIS GetSISDispatchMemosForTransferToDispatch(SISDispatchRequestTableAdapter inForLoad)
        {
            DSSIS toReturn = new DSSIS();
            toReturn.EnforceConstraints = false; 

            inForLoad.GetDispatchRequestMemosForTransferFromSISToDispatch(toReturn.SISDispatchRequest, synchronizer);

            return toReturn;
        }


        /// <summary>
        /// Will return up to 10 rows from the Dispatch database DispatchRequest table
        /// that have not been transferred to the SIS database. 
        /// </summary>
        public static DSDispatch GetDispatchDispatchRowsForTransferToSIS(DispatchRequestTableAdapter inForLoad)
        {
            DSDispatch toReturn = new DSDispatch();
           
            inForLoad.GetDispatchRequestsForTransferFromDispatchToSIS(toReturn.DispatchRequest);

            return toReturn;
        }


        /// <summary>
        /// Will return all Complaint rows from CJ Service History table that were created within the last x minutes.  
        /// The selection time frame starts at lastCJToSISRunTime - number of minutes set in the app.config file.
        /// 
        /// The Complaints will be compared (in the calling method) to Complaints that exist in SIS to see if any
        /// Complaints were created in CJ instead of SIS.  When that is the case the Complaints will be added to the 
        /// SIS database and if the Complaint should have been dispatched, a SIS Dispatch Request will be created.
        /// </summary>
        public static DSCJ GetCJComplaintRowsForTransferToSIS(DateTime inThisCJToSISRunTime)
        {

            DSCJ toReturn = new DSCJ();
            toReturn.EnforceConstraints = false; 
            SERVICE_HISTORYTableAdapter forLoad = new SERVICE_HISTORYTableAdapter();
            forLoad.Connection.ConnectionString = CJConnectionString;

            CJUtility cjReadTimer = new CJUtility(string.Format("Get Service History records for Complaints that were entered within the last {0} minutes to see if they need to be added to the SIS database",
                timeDiscrepancy), CJUtility.AccessType.Read);

            cjReadTimer.StartTimer(); 

            // Adjust the time of the select statement 'from' parameter to the time discrepancy between 
            // CJ and Dispatch servers.
            DateTime thisCJToSISStartTime = inThisCJToSISRunTime.AddMinutes(timeDiscrepancy * -1);
            
            // If the last processing took place on the same day as the 'now' date, retrieve rows for 
            // today only:
            if (thisCJToSISStartTime.Day == inThisCJToSISRunTime.Day)
            {
                forLoad.GetAllComplaintsBy1DateTimeEntered(toReturn.SERVICE_HISTORY,
                        Utility.FormatDatetoCJDateString(thisCJToSISStartTime),
                        Utility.FormatTimetoCJTimeString(thisCJToSISStartTime));

            }
            else
            // If the last processing took place before before midnight and 'now' is after midnight
            // we need to retrieve rows for today and yesterday:
            {
                forLoad.GetAllComplaintsBy2DateTimeEntered(toReturn.SERVICE_HISTORY,
                        Utility.FormatDatetoCJDateString(thisCJToSISStartTime),
                        Utility.FormatTimetoCJTimeString(thisCJToSISStartTime),
                        Utility.FormatDatetoCJDateString(DateTime.Now));
            }

            cjReadTimer.StopTimer(); 


            return toReturn;
        }


        /// <summary>
        /// Will return all Memo rows from CJ Service History table that were created within the last x minutes.  
        /// The selection time frame starts at lastCJToSISRunTime - number of minutes set in the app.config file.
        /// 
        /// The memos will be compared (in the calling method) to memos that exist in SIS to see if any
        /// memos were created in CJ instead of SIS.  When that is the case the memos will be added to the 
        /// SIS database and if the memo should have been dispatched, a SIS Dispatch Request will be created.
        /// </summary>
        public static DSCJ GetCJMemoRowsForTransferToSIS(DateTime inThisCJToSISRunTime)
        {
            DSCJ toReturn = new DSCJ();
            toReturn.EnforceConstraints = false; 
            SERVICE_HISTORYTableAdapter forLoad = new SERVICE_HISTORYTableAdapter();
            forLoad.Connection.ConnectionString = CJConnectionString;

            CJUtility cjReadTimer = new CJUtility(string.Format("Get Service History records for Memos that were entered within the last {0} minutes to see if they need to be added to the SIS database",
                timeDiscrepancy), CJUtility.AccessType.Read);

            cjReadTimer.StartTimer(); 


            // Adjust the time of the select statement 'from' parameter to the time discrepancy between 
            // CJ and Dispatch servers.
            DateTime thisCJToSISStartTime = inThisCJToSISRunTime.AddMinutes(timeDiscrepancy * -1);

            // If the last processing took place on the same day as the 'now' date, retrieve rows for 
            // today only:
            if (thisCJToSISStartTime.Day == inThisCJToSISRunTime.Day)
            {
                forLoad.GetMemosBy1DateTimeEntered(toReturn.SERVICE_HISTORY,
                        Utility.FormatDatetoCJDateString(thisCJToSISStartTime),
                        Utility.FormatTimetoCJTimeString(thisCJToSISStartTime));

            }
            else
            // If the last processing took place before before midnight and 'now' is after midnight
            // we need to retrieve rows for today and yesterday:
            {
                forLoad.GetMemosBy2DateTimeEntered(toReturn.SERVICE_HISTORY,
                        Utility.FormatDatetoCJDateString(thisCJToSISStartTime),
                        Utility.FormatTimetoCJTimeString(thisCJToSISStartTime),
                        Utility.FormatDatetoCJDateString(DateTime.Now));
            }

            cjReadTimer.StopTimer(); 

            return toReturn;
        }



        /// <summary>
        /// Will insert up to 10 rows from the SIS database DispatchRequest table
        /// into the Dispatch database DispatchRequest table. 
        /// </summary>
        public static bool InsertSISDispatchRowsIntoDispatch(DSSIS.SISDispatchRequestDataTable inDispatchRequestTable, 
            DispatchRequestTableAdapter inForInsert)
        {
            System.Nullable<System.DateTime> dispatchDate = new System.Nullable<DateTime>();
            System.Nullable<System.DateTime> deliveredDate = new System.Nullable<DateTime>();

            int processedRows = 0;
            foreach (DSSIS.SISDispatchRequestRow dr in inDispatchRequestTable)
            {
                if (!dr.IsDispatchDateNull())
                    dispatchDate = dr.DispatchDate;

                if (!dr.IsDeliveredDateNull())
                    deliveredDate = dr.DeliveredDate;

                string citycode, streetName, streetType, streetDirection, houseNumber, houseNumberFraction, apartmentNumber;  
                citycode = streetName = streetType = streetDirection = houseNumber = houseNumberFraction = apartmentNumber = string.Empty;  

                Utility.ParseAddressKey(dr.AddressKey, out citycode, out streetName, out streetType, out streetDirection, out houseNumber, out houseNumberFraction, out apartmentNumber);



                // Need to determine if we're savings a dispatched COMPLAINT or MEMO.  If ComplaintCode isn't null, then we're saving a Dispatch COMPLAINT
                if (!dr.IsComplaintCodeNull())
                {
                    if (dr.IsMessengerIdNull())
                        dr.MessengerId = "";
                    if (dr.IsDispatcherIdNull())
                        dr.DispatcherId = "";
                    if (dr.IsCJRequestNumberNull())
                        dr.CJRequestNumber = ""; 

                    processedRows = inForInsert.InsertDispatchComplaint(dr.OperatorId,
                        dr.PubCode, dr.AddressKey, dr.PhoneNumber,dr.HouseholdName,
                        dr.RouteCode, dr.DistrictCode, dr.Status, dr.MessengerId,
                        dr.DispatcherId, dr.Message, dr.DeliveryInstructions,
                        dr.RedeliveryZone, dr.Draw, dr.PaperDate, dr.EnteredDate,
                        dispatchDate, deliveredDate, dr.RequestType, dr.AuditLogLinkId,
                        dr.CJRequestNumber, citycode, houseNumber,houseNumberFraction,
                        streetName, streetType, streetDirection,apartmentNumber, 
                        dr.ComplaintCode, dr.ComplaintDescription
                        );
                }
                // Else we're saving a Dispatch MEMO
                else
                {
                    if (dr.IsMessengerIdNull())
                         dr.MessengerId = ""; 
                    if (dr.IsDispatcherIdNull())
                        dr.DispatcherId = "";
                    if (dr.IsCJRequestNumberNull())
                        dr.CJRequestNumber = ""; 
                    processedRows = inForInsert.InsertDispatchMemo(dr.OperatorId,
                        dr.PubCode, dr.AddressKey, dr.PhoneNumber, dr.HouseholdName,
                        dr.RouteCode, dr.DistrictCode, dr.Status, dr.MessengerId,
                        dr.DispatcherId, dr.Message, dr.DeliveryInstructions,
                        dr.RedeliveryZone, dr.Draw, dr.PaperDate, dr.EnteredDate,
                        dispatchDate, deliveredDate, dr.RequestType, dr.AuditLogLinkId,
                        dr.CJRequestNumber, citycode, houseNumber, houseNumberFraction,
                        streetName, streetType, streetDirection, apartmentNumber,
                        dr.MemoText
                        );
                }

                if (processedRows == 0)
                {
                    string emailBody = string.Format("InsertDispatchComplaint or  InsertDispatchMemo failed to insert DispatchRequest row for {0}",
                        dr.AuditLogLinkId);
                    Email.SendEmail("MailTo", "MailFrom", "Dispatch Synchronizer Failure", emailBody, "SMTPMailServer");
                    return false;
                }
            }

            return true; 
        }

        /// <summary>
        /// Will update the Dispatch and Delivered dates in up to 10 rows in SIS dispatch table. The complaints that have
        /// been dispatched and delivered will have the DispatchDate and DeliveredDate columns
        /// in the Dispatch database changed from null to a date. After that happens, the Dispatch and delivered dates in 
        /// the SIS database must be also updated.
        /// </summary>
        public static bool ReconcileDispatchedDispatchRowsIntoSIS(DSDispatch.DispatchRequestDataTable inDispatchRequestTable, 
                                            SqlTransaction inTransaction)
        {
            SISDispatchRequestTableAdapter forUpdate = new SISDispatchRequestTableAdapter();
            forUpdate.SetTransaction(inTransaction);

            System.Nullable<System.DateTime> dispatchDate = new System.Nullable<DateTime>();
            System.Nullable<System.DateTime> deliveredDate = new System.Nullable<DateTime>();

            int processedRows = 0;
            foreach (DSDispatch.DispatchRequestRow dr in inDispatchRequestTable)
            {
                if (!dr.IsDispatchDateNull())
                    dispatchDate = dr.DispatchDate;

                if (!dr.IsDeliveredDateNull())
                    deliveredDate = dr.DeliveredDate;

                processedRows = forUpdate.SetDispatchRequestDelivered(
                    dispatchDate,
                    deliveredDate,
                    dr.AuditLogLinkId,
                    dr.PubCode,
                    dr.OperatorId
                    );

                if (processedRows == 0)
                {
                    string emailBody = string.Format("UpdateDispatchRequestReconcileWithDispatch failed to update DispatchDate and DeliveredDate for {0}", 
                        dr.AuditLogLinkId);
                    Email.SendEmail("MailSourceAddress", "MailFrom", "Dispatch Synchronizer Failed", emailBody, "SMTPMailServer");
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Will update all (up to 10) rows from the SIS database DispatchRequest table
        /// that have been transferred to the Dispatch database and 'mark' them as 'transferred'. 
        /// </summary>
        public static bool SetSISDispatchRequestToTransferredToDispatch(DSSIS.SISDispatchRequestDataTable inDispatchRequestTable,
                                            SISDispatchRequestTableAdapter inForLoad)
        {
            int processedRows = 0;

            foreach (DSSIS.SISDispatchRequestRow dr in inDispatchRequestTable)
            {
                int tryNum = 0; 
                for (tryNum = 0; tryNum < 5; tryNum++)
                {
                    try
                    {
                        processedRows = inForLoad.SetDispatchRequestToTransferredToDispatch(
                           dr.Id,
                           dr.LastUpdate,
                           synchronizer);
                        break; 
                    }
                    catch (Exception ex)
                    {
                    }
                }
                if (tryNum >= 5) 
                {
                                        string emailBody = string.Format("SetSISDispatchRequestToTransferredToDispatch failed for AuditLogLinkId {0}, OperatorId {1}, LastUpdate {2}",
                        dr.AuditLogLinkId, dr.OperatorId, dr.LastUpdate);
                    Email.SendEmail("MailSourceAddress", "MailFrom", "Dispatch Synchronizer Failed", emailBody, "SMTPMailServer");
                }
            }

            return true;
        }


        /// <summary>
        /// Will update all (up to 10) rows from the SIS database DispatchRequest table
        /// that have been transferred to the Dispatch database and 'mark' them as 'transferred'. 
        /// </summary>
        public static bool SetDispatchDispatchRequestToTransferredToSIS(DSDispatch.DispatchRequestDataTable inDispatchRequestTable,
                                                DispatchRequestTableAdapter inForUpdate)
        {

            int processedRows = 0;
            foreach (DSDispatch.DispatchRequestRow dr in inDispatchRequestTable)
            {
                processedRows = inForUpdate.SetDispatchRequestToTransferredToSIS(dr.AuditLogLinkId,
                    dr.PubCode,
                    dr.LastUpdate);

                if (processedRows == 0)
                {
                    string emailBody = string.Format("SetDispatchRequestToTransferredToSIS failed for Id {0}, LastUpdate {1}",
                        dr.Id, dr.LastUpdate);
                    Email.SendEmail("MailSourceAddress", "MailFrom", "Dispatch Synchronizer Failed", emailBody, "SMTPMailServer");
//                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Attempts to set the "Dispatched" DisptachTime for the CJ Service History records for all rows in the DispatchRequest table.  
        /// </summary>
        public static bool SetCJServiceHistoryToDispatched(DSDispatch.DispatchRequestDataTable inDispatchRequestTable, 
            OdbcTransaction inCJTransaction)
        {
            DSSIS dsSIS = new DSSIS(); 
            ComplaintHistoryTableAdapter taSISComplaints = new ComplaintHistoryTableAdapter();
            MemoHistoryTableAdapter taSISMemos = new MemoHistoryTableAdapter();
            SERVICE_HISTORYTableAdapter forSave = new SERVICE_HISTORYTableAdapter(); 
            forSave.SetTransaction(inCJTransaction); 

            foreach (DSDispatch.DispatchRequestRow dr in inDispatchRequestTable)
            {
                // Try to find a complaint associated with the Audit Log Id in the Dispatch Request
                taSISComplaints.GetComplaintHistoryByAuditLogLinkId(dsSIS.ComplaintHistory, dr.AuditLogLinkId.ToString());
                if (dsSIS.ComplaintHistory.Count > 0)
                {
                    int cjRecsUpdated = 0;

                    // There may be more than one dispatch request tied to an Audit Log item (complaint may be for missing 
                    // DP + TV) so match complaints with dispatch request via Pub Code
                    foreach (DSSIS.ComplaintHistoryRow complaintRow in dsSIS.ComplaintHistory)
                    {
                        if (complaintRow.PublicationCode == dr.PubCode)
                        {
                            string fromHours, toHours;
                            GetTimeRange(Utility.FormatTimetoCJTimeString(complaintRow.EnteredDate), out fromHours, out toHours); 

                            // Try to update the CJ Service History record for the given Complaint
                            // with the dispatched time.
                            cjRecsUpdated =
                            forSave.SetDispatchTimeOfDisptachComplaint(
                                Utility.FormatTimetoCJTimeString(dr.DispatchDate),
                                complaintRow.HouseholdAddressKey,
                                complaintRow.SubscriptionNumber,
                                complaintRow.PublicationCode,
                                complaintRow.ServiceCode,
                                Utility.FormatDatetoCJDateString(complaintRow.EffectiveDate),
                                Utility.FormatDatetoCJDateString(complaintRow.EnteredDate),
                                fromHours, toHours,
                                complaintRow.CJComplaintCode);
                            break;
                        }
                    }
                    // If we couldn't find the complaint, something's wrong so throw an exception
                    if (cjRecsUpdated != 1)
                    {
                        string emailBody = string.Format("SetCJServiceHistoryToDispatched Could Not find the CJ Service History for the Complaint associated with AuditLogLinkId {0}", dr.AuditLogLinkId);
                        Email.SendEmail("MailSourceAddress", "MailFrom", "Dispatch Synchronizer Failed", emailBody, "SMTPMailServer");
                        //throw new Exception(emailBody);
                    }
                }
                // If we didn't find a Complaint for the audit log id, look for a Memo with that complaint
                else
                {
                    taSISMemos.GetMemoHistoryByAuditLogLinkId(dsSIS.MemoHistory, dr.AuditLogLinkId);

                    // If we found a memo, there should be only 1 memo, so use it..
                    if (dsSIS.MemoHistory.Count > 0)
                    {
                        DSSIS.MemoHistoryRow memoRow = dsSIS.MemoHistory[0];

                        string fromHours, toHours;
                        GetTimeRange(Utility.FormatTimetoCJTimeString(memoRow.CreateDate), out fromHours, out toHours); 

                        // Try to update the CJ Service History record for the given Memo
                        // with the dispatched time.
                        int cjRecsUpdated = forSave.SetDisptachTimeOfDispatchMemo(
                            Utility.FormatTimetoCJTimeString(dr.DispatchDate), 
                            memoRow.HouseholdAddressKey, 
                            Utility.FormatDatetoCJDateString(memoRow.EffectiveDate),
                            Utility.FormatDatetoCJDateString(memoRow.CreateDate),
                            fromHours, toHours,
                            memoRow.OperatorInitials, 
                            memoRow.MemoText.ToUpper());

                        // If we didn't update CJ history record, something's wrong, so throw an exception
                        if (cjRecsUpdated == 0)
                        {
                            string emailBody = string.Format("SetCJServiceHistoryToDispatched Could Not find CJ Service History for the Memo associated with AuditLogLinkId {0}", dr.AuditLogLinkId);
                            Email.SendEmail("MailSourceAddress", "MailFrom", "Dispatch Synchronizer Failed", emailBody, "SMTPMailServer");
//                            throw new Exception(emailBody);

                        }
                    }
                    // If we didn't find a Complaint or a Memo for the Audit Log Id then something is really wrong, 
                    // throw an exception.
                    else
                    {
                        string emailBody = string.Format("SetCJServiceHistoryToDispatched Could Not find SIS Complaint or Memo tied to the AuditLogLinkId {0}", dr.AuditLogLinkId);
                        Email.SendEmail("MailSourceAddress", "MailFrom", "Dispatch Synchronizer Failed", emailBody, "SMTPMailServer");
//                        throw new Exception(emailBody); 
                    }
                }
            }
            return true; 
        }

        /// <summary>
        /// The TIME_ENTERED value in CJ's Service History record can be off by several minutes (compared to 
        /// SIS and Dispatch times), so when looking for a matching Service History record in CJ 
        /// we need to look for time within a range.  This method takes an input time 
        /// and returns a "From Time" and a "To Time" - the "From Time" will be x minutes before the input time
        /// and the "To Time" will be x minutes after the input time.
        /// </summary>
        /// <param name="inTime"></param>
        /// <param name="outFromTime"></param>
        /// <param name="outToTime"></param>
        private static void GetTimeRange(string inTime, out string outFromTime, out string outToTime)
        {
            const int minutesToAddSubtract = 5; 

            int hours = Int32.Parse(inTime.Substring(0, 2));
            int minutes = Int32.Parse(inTime.Substring(2, 2));

            int fromHours = 0;
            int fromMinutes = 0;
            int toHours = 0;
            int toMinutes = 0; 

            // Calculate the From Minutes
            if (minutes < minutesToAddSubtract)
            {
                if (hours == 0)
                {
                    fromHours = 0;
                    fromMinutes = 0; 
                }
                else
                {
                    fromHours = hours - 1;
                    fromMinutes = 60 - minutesToAddSubtract; 
                }
            }
            else 
            {
                fromHours = hours;
                fromMinutes = minutes - minutesToAddSubtract;
            }

            // Calculate To Minutes
            if (minutes + minutesToAddSubtract >= 60)
            {
                toMinutes = (minutes + minutesToAddSubtract) % 60;

                if (hours == 23)
                {
                    toHours = 0;
                }
                else
                {
                    toHours = hours + 1;
                }
            }
            else
            {
                toMinutes = minutes + minutesToAddSubtract;
                toHours = hours; 
            }
            outFromTime = string.Format("{0:00}{1:00}", fromHours, fromMinutes);
            outToTime = string.Format("{0:00}{1:00}", toHours, toMinutes);
        }
    }
}
