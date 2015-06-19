using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Odbc; 
using System.Threading; 
using DNACircSynchronizer.Processes.DSSISTableAdapters;
using DNACircSynchronizer.Processes.DSDispatchTableAdapters;


namespace DNACircSynchronizer.Processes
{
    /// <summary>
    /// This class provides data access tracking for ODBC connections and transactions to CJ.  
    /// 
    /// This class tracks when a connection is opened or transction is started and when that connection 
    /// is closed or the transaction is committed or rolledback.  Time limits can be set on the open connections
    /// and then the time limit is reached an email can be sent to the developer staff to warn them of 
    /// a possible problem.  Data access times can also be sent to a database table.  This class is used 
    /// to diagnose data access problems.  
    /// </summary>
    internal class CJUtility
    {
        #region TransactionUtilities

        /// <summary>
        /// Type of data access - Read or Write?
        /// </summary>
        public enum AccessType { Read, Write };

        /// <summary>
        /// Thread to track access times
        /// </summary>
        private Thread _accessTimer = null;

        /// <summary>
        /// Information about the data access being performed - the value of this field will be sent 
        /// to the developers when data access time is over the limit and/or stored in the database.  
        /// In essence this information should be used to help pinpoint a location in code that may 
        /// have problematic data access handling.  At the very least the class name and method 
        /// containing the data access code should be provided for this field. 
        /// </summary>
        private string _accessInformation; 
        private AccessType _accessType = AccessType.Read;


        /// <summary>
        /// Start time of the data access.
        /// </summary>
        private DateTime _startTime;

        /// <summary>
        /// Stop time of the data access.
        /// </summary>
        private DateTime _stopTime;

        /// <summary>
        /// Whether or not the timer was started.
        /// </summary>
        private bool _timerStarted = false;

        /// <summary>
        /// Time limit for this data access, in milliseconds.  Default to 1000.
        /// </summary>
        private int _timeLimit = 1000;

        /// <summary>
        /// Whether or not the time limit was exceeded.
        /// </summary>
        private bool _timeLimitExceeded = false;

        /// <summary>
        /// When database logging is enabled, a database record is created at the start of data access, 
        /// that record is then updated when data access completes - this field holds the Id of the 
        /// record that was created so that the record can be updated when data access is complete.
        /// </summary>
        private Nullable<int> _dbRecordId = null;

        /// <summary>
        /// Should data access times be monitored?
        /// </summary>
        static private bool _monitorAccessTime = false;

        /// <summary>
        /// Should data access times be monitored and the results saved to the database?
        /// </summary>
        static private bool _monitorAccessTimeToDB = false;

        /// <summary>
        /// Should data access times be monitored and when a data access limit is reached, should 
        /// the developers be emailed about that limit breach?
        /// </summary>
        static private bool _monitorAccessTimeToEmail = false;

        /// <summary>
        /// Default number of milliseconds for the time limit for Read access
        /// </summary>
        static private int _readTimeLimit = 1000;

        /// <summary>
        /// Default number of milliseconds for the time limit for Write access
        /// </summary>
        static private int _writeTimeLimit = 3000;

        /// <summary>
        /// Maximum time limit - if this limit is reached, the data access is assumed to be blocked, an 
        /// additional email will be sent out to the developers when this time limit is reached.  
        /// </summary>
        static private int _maxTimeLimit = 40000;


        /// <summary>
        /// Static constructor to read settings from the Application Configuration file and set 
        /// flags in the class.  
        /// </summary>
        static CJUtility()
        {
            string readTimeLimit = System.Configuration.ConfigurationManager.AppSettings["CJReadTimeLimit"];
            string writeTimeLimit = System.Configuration.ConfigurationManager.AppSettings["CJWriteTimeLimit"];
            string monitorAccessToDB = System.Configuration.ConfigurationManager.AppSettings["MonitorDataAccessToDB"];
            string sendMonitorEmails = System.Configuration.ConfigurationManager.AppSettings["MonitorDataAccessToEmail"];

            _monitorAccessTimeToDB = Utility.ParseBooleanValue(monitorAccessToDB);
            _monitorAccessTimeToEmail = Utility.ParseBooleanValue(sendMonitorEmails);
            if (_monitorAccessTimeToDB || _monitorAccessTimeToEmail)
                _monitorAccessTime = true; 

            Int32.TryParse(readTimeLimit, out _readTimeLimit);
            Int32.TryParse(writeTimeLimit, out _writeTimeLimit);
        }

        /// <summary>
        /// This method is called on a separate thread when monitoring is turned on and a connection 
        /// is opened or a transaction started. This thread sleeps for the specific time limit.  
        /// If data access completed before that sleep period ends, this thread is aborted (via 
        /// the StopTimer method).  If data access does not complete before the sleep period ends, 
        /// an email will be sent to the developers (if emailing is turned on) to warn developers 
        /// of a long-running data access call.  This method will then sleep for a longer period of time - 
        /// if the timer thread is not aborted by the end of the second sleep period, there is 
        /// probably something drastically wrong with the data access call (probably a deadlock), 
        /// the developers are emailed (this option cannot be turned off).  
        /// </summary>
        private void AccessTimer()
        {
            try
            {
                // Sleep for the time limit for this data access.  This method runs on a different thread
                // from the data access functionality.  When the data access functionality completes, this 
                // thread is aborted.  
                Thread.Sleep(_timeLimit);

                _timeLimitExceeded = true;

                string body = string.Empty;

                // If this thread isn't aborted during the sleep time above, then data access exceeded the
                // time limit - if we're configured to email the developers on this occurrence, then 
                // send the email.
                if (_monitorAccessTimeToEmail)
                {
                    body = string.Format("ODBC Data Access Limit Reached.  {3} Access Information: {0} {3} Access Type: {1} {3} Time Limit Exceeded: {2}",
                        _accessInformation,
                        _accessType.ToString(),
                        _timeLimit,
                        "\n\r");

                    Email.ReportErrorToProgrammers("Synchronizer: (ODBC) Data Access Limit Reached", body);
                }


                // Sleep a longer period of time.  If this thread isn't aborted, then send another message as the 
                // request tied to this data access probably never completed.  (Data access might be deadlocked).
                Thread.Sleep(_maxTimeLimit);

                int maxSeconds = _maxTimeLimit / 1000;
                body = string.Format("The thread for this data access didn't complete within {0} seconds - it is probably deadlocked. {3} Access Information: {1} {3} Access Type: {2} {3}",
                    maxSeconds,
                    _accessInformation,
                    _accessType.ToString(),
                    "\n\r");

                Email.ReportErrorToProgrammers("Synchronizer: (ODBC) Data Access Thread Probably Never Completed", body);

            }
            // The data access thread will abort this thread when data access is complete.  When 
            // a thread is aborted, a ThreadAbortException is raised, so catch that exception here
            // and do nothing.  
            catch (ThreadAbortException ex)
            {
                // Even though you're not supposed to use exceptions for process flow, this is
                // the only way to kill a thread.
            }
        }


        /// <summary>
        /// Instance constructor for the CJUtility.
        /// </summary>
        /// <param name="inAccessInformation">What type of data access was being performed?  This 
        /// information is stored in the database and/or sent to the user when a data access problem is 
        /// found, so this information should be able to uniquely identify the caller - at the very least
        /// the Class Name and Method Name should be sent.  
        /// </param>
        /// <param name="inAccessType">Read or Write data access?</param>
        public CJUtility(string inAccessInformation, AccessType inAccessType)
        {
            // If configured to NOT monitor data access, leave.  
            if (!_monitorAccessTime)
                return; 

            _accessInformation = inAccessInformation;
            _accessType = inAccessType;

            // Set the time limit based on the type of data access.  
            if (inAccessType == AccessType.Write)
                _timeLimit = _writeTimeLimit;
            else
                _timeLimit = _readTimeLimit; 
        }


        /// <summary>
        /// Start the data access timer.  This method is called when a transaction is strated but 
        /// may also be called by the data access code when starting a Read.  
        /// </summary>
        public void StartTimer()
        {
            // If the timer is already started or we're not monitoring time, leave.
            if (_timerStarted || !_monitorAccessTime)
                return;

            _timerStarted = true;
            _startTime = DateTime.Now;

            // Create and start the separate thread to "monitor" access times.  
            _accessTimer = new Thread(new ThreadStart(AccessTimer));
            _accessTimer.Start();

            // If we're saving monitor data to the database, create the record in the database - this 
            // record will the program name, access information, start time and the limit.  If/when 
            // data access completes this record will be updated with end time and number of milliseconds spent
            // during data access.
            if (_monitorAccessTimeToDB)
            {
                try
                {
                    new DSDataAccessLogTableAdapters.DataAccessLogTableAdapter().InsertDataAccessLog(
                        "Synchronizer",
                        _accessInformation,
                        _startTime, _timeLimit, ref _dbRecordId);
                }
                catch (Exception ex)
                {
                    Email.ReportErrorToProgrammers("Synchronizer: Data Access Logging Problem (ODBC)", "The following exception was thrown when attempting to insert to the DataAccessLog table: " + ex.ToString());
                }
            }
        }

        /// <summary>
        /// Stop the data access monitor.  This is called from Commit and Rollback but can also be called
        /// directly from the data access code for a Read access.
        /// </summary>
        public void StopTimer()
        {
            // If the timer isn't running or we're not monitoring access time, exit.
            if (!_timerStarted || !_monitorAccessTime)
                return;

            _timerStarted = false;

            // If we're monitoring access time, then abort the "monitor" thread 'cause we're 
            // done with the data access.
            if (_accessTimer != null &&
                (_accessTimer.ThreadState == System.Threading.ThreadState.WaitSleepJoin ||
                 _accessTimer.ThreadState == System.Threading.ThreadState.Running))
            {
                _accessTimer.Abort();
            }
            else
            {
                string stuff = "Found a thread I didn't abort";
            }


            _stopTime = DateTime.Now;
            TimeSpan ts = new TimeSpan(_stopTime.Ticks - _startTime.Ticks);

            int seconds = ts.Seconds;
            int milliseconds = ts.Milliseconds;

            // If the time limit was exceeded and Email's have been enabled, 
            // send the programmer notification that the limit was exceeded.  
            if (_timeLimitExceeded && _monitorAccessTimeToEmail)
            {
                string body = string.Format("ODBC Time spent on {0} ({1}) was {2}:{3}",
                    _accessInformation,
                    _accessType.ToString(),
                    ts.Seconds, ts.Milliseconds);

                Email.ReportErrorToProgrammers("Synchronizer: (ODBC) Data Access Time", body);
            }

            // If we're storing access times in the database, update the DB access record 
            // that was created in the StartTimer() method.  
            if (_monitorAccessTimeToDB)
            {
                int totalMilliseconds = (ts.Minutes * 60000) + (ts.Seconds * 1000) + ts.Milliseconds;
                try
                {
                    new DSDataAccessLogTableAdapters.DataAccessLogTableAdapter().UpdateDataAccessLog(
                        _dbRecordId.Value,
                        _stopTime,
                        totalMilliseconds);
                }
                catch (Exception ex)
                {
                    Email.ReportErrorToProgrammers("Synchronizer: Data Access Logging Problem (ODBC)", "The following exception was thrown when attempting to update the DataAccessLog table: " + ex.ToString());
                }
            }
        }


        /// <summary>
        /// Start a database transaction to CJ's DCISDB database.  If monitoring is enabled, monitoring will also be started.
        /// </summary>
        /// <returns>Started transaction</returns>
        public OdbcTransaction StartTransaction()
        {
            StartTimer(); 

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DNACircSynchronizer.Processes.Properties.Settings.CJConnectionString"].ConnectionString;
            OdbcConnection forTransaction = new OdbcConnection(connectionString);
            forTransaction.Open(); 
            OdbcTransaction toReturn = forTransaction.BeginTransaction(System.Data.IsolationLevel.Serializable);
            return toReturn;
        }



        /// <summary>
        /// Commit a CJ transaction.  If data monitor is enabled, this method will also stop 
        /// monitoring for this instance.  
        /// </summary>
        /// <param name="inTransaction">Transaction to Commit.</param>
        public void CommitTransaction(OdbcTransaction inTransaction)
        {
            inTransaction.Commit();
            if (inTransaction != null && inTransaction.Connection != null &&
                inTransaction.Connection.State == ConnectionState.Open)
            {
                inTransaction.Connection.Close();
            }

            StopTimer(); 
        }

        /// <summary>
        /// Rollback a CJ transaction.  If data monitor is enabled, this method will also stop 
        /// monitoring for this instance.  
        /// </summary>
        /// <param name="inTransaction">Transaction to rollback</param>
        public void RollbackTransaction(OdbcTransaction inTransaction)
        {
            inTransaction.Rollback();
            if (inTransaction != null && inTransaction.Connection != null &&
                inTransaction.Connection.State == ConnectionState.Open)
            {
                inTransaction.Connection.Close();
            }

            StopTimer(); 
        }
        #endregion TransactionUtilities
    }


    internal class TAHelper
    {
        /// <summary>
        /// Set the Connection and Transaction properties of the Insert, Update and Delete 
        /// commands (if not null) for a given Data Adapter.
        /// </summary>
        /// <param name="inTransaction">SQL Transaction</param>
        /// <param name="inDataAdapter">Data Adapter in which to set the command's Transaction and Connections properties</param>
        public static void SetTransaction(SqlTransaction inTransaction, SqlDataAdapter inDataAdapter)
        {
            if (inDataAdapter.InsertCommand != null)
            {
                inDataAdapter.InsertCommand.Connection = inTransaction.Connection;
                inDataAdapter.InsertCommand.Transaction = inTransaction;
            }

            if (inDataAdapter.UpdateCommand != null)
            {
                inDataAdapter.UpdateCommand.Connection = inTransaction.Connection;
                inDataAdapter.UpdateCommand.Transaction = inTransaction;
            }

            if (inDataAdapter.DeleteCommand != null)
            {
                inDataAdapter.DeleteCommand.Connection = inTransaction.Connection;
                inDataAdapter.DeleteCommand.Transaction = inTransaction;
            }
        }

        /// <summary>
        /// Set the Connection properties of the Select, Insert, Update and Delete 
        /// commands (if not null) for a given Data Adapter.
        /// </summary>
        /// <param name="inConnection">SQL Connection</param>
        /// <param name="inDataAdapter">Data Adapter in which to set the command's Connections properties</param>
        public static void SetConnection(SqlConnection inConnection, SqlDataAdapter inDataAdapter)
        {
            if (inDataAdapter.InsertCommand != null)
            {
                inDataAdapter.InsertCommand.Connection = inConnection;
            }

            if (inDataAdapter.UpdateCommand != null)
            {
                inDataAdapter.UpdateCommand.Connection = inConnection;
            }

            if (inDataAdapter.DeleteCommand != null)
            {
                inDataAdapter.DeleteCommand.Connection = inConnection;
            }
            if (inDataAdapter.SelectCommand != null)
            {
                inDataAdapter.SelectCommand.Connection = inConnection;
            }
        }
    }

    /// <summary>
    /// This class provides data access tracking for SQL connections and transactions.  
    /// 
    /// This class tracks when a connection is opened or transction is started and when that connection 
    /// is closed or the transaction is committed or rolledback.  Time limits can be set on the open connections
    /// and then the time limit is reached an email can be sent to the developer staff to warn them of 
    /// a possible problem.  Data access times can also be sent to a database table.  This class is used 
    /// to diagnose data access problems.  
    /// </summary>
    public class DataSQLUtility
    {
        /// <summary>
        /// Type of data access - Read or Write?
        /// </summary>
        public enum AccessType { Read, Write }; 

        /// <summary>
        /// Thread to track access times
        /// </summary>
        private Thread _accessTimer = null;

        /// <summary>
        /// Information about the data access being performed - the value of this field will be sent 
        /// to the developers when data access time is over the limit and/or stored in the database.  
        /// In essence this information should be used to help pinpoint a location in code that may 
        /// have problematic data access handling.  At the very least the class name and method 
        /// containing the data access code should be provided for this field. 
        /// </summary>
        private string _accessInformation; 
        private AccessType _accessType = AccessType.Read;

        /// <summary>
        /// Start time of the data access.
        /// </summary>
        private DateTime _startTime;

        /// <summary>
        /// Stop time of the data access.
        /// </summary>
        private DateTime _stopTime;

        /// <summary>
        /// Whether or not the timer was started.
        /// </summary>
        private bool _timerStarted = false;

        /// <summary>
        /// Time limit for this data access, in milliseconds.  Default to 1000.
        /// </summary>
        private int _timeLimit = 1000;

        /// <summary>
        /// Whether or not the time limit was exceeded.
        /// </summary>
        private bool _timeLimitExceeded = false;

        /// <summary>
        /// When database logging is enabled, a database record is created at the start of data access, 
        /// that record is then updated when data access completes - this field holds the Id of the 
        /// record that was created so that the record can be updated when data access is complete.
        /// </summary>
        private Nullable<int> _dbRecordId = null;

        /// <summary>
        /// Should data access times be monitored?
        /// </summary>
        static private bool _monitorAccessTime = false;

        /// <summary>
        /// Should data access times be monitored and the results saved to the database?
        /// </summary>
        static private bool _monitorAccessTimeToDB = false;

        /// <summary>
        /// Should data access times be monitored and when a data access limit is reached, should 
        /// the developers be emailed about that limit breach?
        /// </summary>
        static private bool _monitorAccessTimeToEmail = false;

        /// <summary>
        /// Default number of milliseconds for the time limit for Read access
        /// </summary>
        static private int _defaultReadTimeLimit = 1000;

        /// <summary>
        /// Default number of milliseconds for the time limit for Write access
        /// </summary>
        static private int _defaultWriteTimeLimit = 3000;

        /// <summary>
        /// Maximum time limit - if this limit is reached, the data access is assumed to be blocked, an 
        /// additional email will be sent out to the developers when this time limit is reached.  
        /// </summary>
        static private int _maxTimeLimit = 40000; 

        /// <summary>
        /// Static constructor to read settings from the Application Configuration file and set 
        /// flags in the class.  
        /// </summary>
        static DataSQLUtility()
        {
            string readTimeLimit = System.Configuration.ConfigurationManager.AppSettings["SQLReadTimeLimit"];
            string writeTimeLimit = System.Configuration.ConfigurationManager.AppSettings["SQLWriteTimeLimit"];

            string monitorAccessToDB = System.Configuration.ConfigurationManager.AppSettings["MonitorDataAccessToDB"];
            string sendMonitorEmails = System.Configuration.ConfigurationManager.AppSettings["MonitorDataAccessToEmail"];

            _monitorAccessTimeToDB = Utility.ParseBooleanValue(monitorAccessToDB);
            _monitorAccessTimeToEmail = Utility.ParseBooleanValue(sendMonitorEmails);
            if (_monitorAccessTimeToDB || _monitorAccessTimeToEmail)
                _monitorAccessTime = true; 

            Int32.TryParse(readTimeLimit, out _defaultReadTimeLimit);
            Int32.TryParse(writeTimeLimit, out _defaultWriteTimeLimit);
        }

        /// <summary>
        /// This method is called on a separate thread when monitoring is turned on and a connection 
        /// is opened or a transaction started. This thread sleeps for the specific time limit.  
        /// If data access completed before that sleep period ends, this thread is aborted (via 
        /// the StopTimer method).  If data access does not complete before the sleep period ends, 
        /// an email will be sent to the developers (if emailing is turned on) to warn developers 
        /// of a long-running data access call.  This method will then sleep for a longer period of time - 
        /// if the timer thread is not aborted by the end of the second sleep period, there is 
        /// probably something drastically wrong with the data access call (probably a deadlock), 
        /// the developers are emailed (this option cannot be turned off).  
        /// </summary>
        private void AccessTimer()
        {
            try
            {
                // Sleep for the time limit for this data access.  This method runs on a different thread
                // from the data access functionality.  When the data access functionality completes, this 
                // thread is aborted.  
                Thread.Sleep(_timeLimit);

                if (!_timerStarted)
                    return; 

                _timeLimitExceeded = true;
                string body = string.Empty;

                // If this thread isn't aborted during the sleep time above, then data access exceeded the
                // time limit - if we're configured to email the developers on this occurrence, then 
                // send the email.
                if (_monitorAccessTimeToEmail)
                {
                    body = string.Format("SQL Data Access Limit Reached.  {3} Access Information: {0} {3} Access Type: {1} {3} Time Limit Exceeded: {2}",
                        _accessInformation,
                        _accessType.ToString(),
                        _timeLimit,
                        "\n\r");

                    Email.ReportErrorToProgrammers("Synchronizer: (SQL) Data Access Limit Reached", body);
                }

                // Sleep a longer period of time.  If this thread isn't aborted, then send another message as the 
                // request tied to this data access probably never completed.  (Data access might be deadlocked).
                Thread.Sleep(_maxTimeLimit);

                int maxSeconds = _maxTimeLimit / 1000;
                body = string.Format("The thread for this data access didn't complete within {0} seconds - it is probably deadlocked. {3} Access Information: {1} {3} Access Type: {2} {3}",
                    maxSeconds,
                    _accessInformation,
                    _accessType.ToString(),
                    "\n\r");

                Email.ReportErrorToProgrammers("Synchronizer: (SQL) Data Access Thread Probably Never Completed", body);

            }
            // The data access thread will abort this thread when data access is complete.  When 
            // a thread is aborted, a ThreadAbortException is raised, so catch that exception here
            // and do nothing.  
            catch (ThreadAbortException ex)
            {
                // Even though you're not supposed to use exceptions for process flow, this is
                // the only way to kill a thread.
            }
        }

        /// <summary>
        /// Instance constructor for the DataSQLUtility.
        /// </summary>
        /// <param name="inAccessInformation">What type of data access was being performed?  This 
        /// information is stored in the database and/or sent to the user when a data access problem is 
        /// found, so this information should be able to uniquely identify the caller - at the very least
        /// the Class Name and Method Name should be sent.  
        /// </param>
        /// <param name="inAccessType">Read or Write data access?</param>
        public DataSQLUtility(string inAccessInformation, AccessType inAccessType)
        {
            // If configured to NOT monitor data access, leave.  
            if (!_monitorAccessTime)
                return; 

            _accessInformation = inAccessInformation;
            _accessType = inAccessType;

            // Set the time limit based on the type of data access.  
            if (inAccessType == AccessType.Write)
                _timeLimit = _defaultWriteTimeLimit;
            else
                _timeLimit = _defaultReadTimeLimit; 
        }


        /// <summary>
        /// Start the data access timer.  This method is called when a transaction is strated but 
        /// may also be called by the data access code when starting a Read.  
        /// </summary>
        public void StartTimer()
        {
            // If the timer is already started or we're not monitoring time, leave.
            if (_timerStarted || !_monitorAccessTime)
                return;

            _timerStarted = true;
            _startTime = DateTime.Now;

            // Create and start the separate thread to "monitor" access times.  
            _accessTimer = new Thread(new ThreadStart(AccessTimer));
            _accessTimer.Start();

            // If we're saving monitor data to the database, create the record in the database - this 
            // record will the program name, access information, start time and the limit.  If/when 
            // data access completes this record will be updated with end time and number of milliseconds spent
            // during data access.
            if (_monitorAccessTimeToDB)
            {
                try
                {
                    new DSDataAccessLogTableAdapters.DataAccessLogTableAdapter().InsertDataAccessLog(
                        "Synchronizer - SQL",
                        _accessInformation,
                        _startTime, _timeLimit, ref _dbRecordId);
                }
                catch (Exception ex)
                {
                    Email.ReportErrorToProgrammers("Synchronizer: Data Access Logging Problem (SQL)", "The following exception was thrown when attempting to insert to the DataAccessLog table: " + ex.ToString());
                }
            }
        }

        /// <summary>
        /// Stop the data access monitor.  This is called from Commit and Rollback but can also be called
        /// directly from the data access code for a Read access.
        /// </summary>
        public void StopTimer()
        {
            // If the time isn't running or we're not monitoring access time, exit.
            if (!_timerStarted || !_monitorAccessTime)
                return;

            _timerStarted = false;

            // If we're monitoring access time, then abort the "monitor" thread 'cause we're 
            // done with the data access.
            if (_accessTimer != null &&
                (_accessTimer.ThreadState == System.Threading.ThreadState.WaitSleepJoin ||
                 _accessTimer.ThreadState == System.Threading.ThreadState.Running))
            {
                _accessTimer.Abort();
            }
            else
            {
                string stuff = "Found a thread I didn't abort";
            }


            _stopTime = DateTime.Now;
            TimeSpan ts = new TimeSpan(_stopTime.Ticks - _startTime.Ticks);

            int seconds = ts.Seconds;
            int milliseconds = ts.Milliseconds;

            // If the time limit was exceeded and Email's have been enabled, 
            // send the programmer notification that the limit was exceeded.  
            if (_timeLimitExceeded && _monitorAccessTimeToEmail)
            {
                string body = string.Format("SQL Time spent on {0} ({1}) was {2}:{3}",
                    _accessInformation,
                    _accessType.ToString(),
                    ts.Seconds, ts.Milliseconds);

                Email.ReportErrorToProgrammers("Synchronizer: (SQL) Data Access Time", body);
            }

            // If we're storing access times in the database, update the DB access record 
            // that was created in the StartTimer() method.  
            if (_monitorAccessTimeToDB)
            {
                int totalMilliseconds = (ts.Minutes * 60000) + (ts.Seconds * 1000) + ts.Milliseconds;
                try
                {
                    new DSDataAccessLogTableAdapters.DataAccessLogTableAdapter().UpdateDataAccessLog(
                        _dbRecordId.Value,
                        _stopTime,
                        totalMilliseconds);
                }
                catch (Exception ex)
                {
                    Email.ReportErrorToProgrammers("SIS: Data Access Logging Problem (SQL)", "The following exception was thrown when attempting to update to the DataAccessLog table: " + ex.ToString());
                }
            }
        }

        /// <summary>
        /// Stop the data access monitor.  This is called from Commit and Rollback but can also be called
        /// directly from the data access code for a Read access.  This method allows additional information 
        /// to be saved in the log table.
        /// </summary>
        public void StopTimer(string inAdditionalInformation)
        {
            // If the time isn't running or we're not monitoring access time, exit.
            if (!_timerStarted || !_monitorAccessTime)
                return;

            _timerStarted = false;

            // If we're monitoring access time, then abort the "monitor" thread 'cause we're 
            // done with the data access.
            if (_accessTimer != null &&
                (_accessTimer.ThreadState == System.Threading.ThreadState.WaitSleepJoin ||
                 _accessTimer.ThreadState == System.Threading.ThreadState.Running))
            {
                _accessTimer.Abort();
            }
            else
            {
                string stuff = "Found a thread I didn't abort";
            }


            _stopTime = DateTime.Now;
            TimeSpan ts = new TimeSpan(_stopTime.Ticks - _startTime.Ticks);

            int seconds = ts.Seconds;
            int milliseconds = ts.Milliseconds;

            // If the time limit was exceeded and Email's have been enabled, 
            // send the programmer notification that the limit was exceeded.  
            if (_timeLimitExceeded && _monitorAccessTimeToEmail)
            {
                string body = string.Format("SQL Time spent on {0} ({1}) was {2}:{3}",
                    _accessInformation,
                    _accessType.ToString(),
                    ts.Seconds, ts.Milliseconds);

                Email.ReportErrorToProgrammers("Synchronizer: (SQL) Data Access Time", body);
            }

            // If we're storing access times in the database, update the DB access record 
            // that was created in the StartTimer() method.  
            if (_monitorAccessTimeToDB)
            {
                int totalMilliseconds = (ts.Minutes * 60000) + (ts.Seconds * 1000) + ts.Milliseconds;
                try
                {
                    new DSDataAccessLogTableAdapters.DataAccessLogTableAdapter().UpdateDataAccessLogWithAdditionalInfo(
                        _dbRecordId.Value,
                        _stopTime,
                        totalMilliseconds, 
                        inAdditionalInformation);
                }
                catch (Exception ex)
                {
                    Email.ReportErrorToProgrammers("SIS: Data Access Logging Problem (SQL)", "The following exception was thrown when attempting to update to the DataAccessLog table: " + ex.ToString());
                }
            }
        }



        /// <summary>
        /// Start a SQL transaction on the Dispatch database.  If monitoring is enabled, monitoring will also be started.
        /// </summary>
        /// <param name="inTransactionName">Name of the transaction to start.</param>
        /// <returns>SQL transaction that was started.</returns>
        public SqlTransaction StartDispatchTransaction(string inTransactionName)
        {
            StartTimer(); 

            DispatchRequestTableAdapter forTransaction = new DispatchRequestTableAdapter();
            SqlConnection toOpen = forTransaction.GetConnection();
            toOpen.Open();
            return toOpen.BeginTransaction(inTransactionName);

        }

        /// <summary>
        /// Start a SQL transaction on the SIS database.  If monitoring is enabled, monitoring will also be started.
        /// </summary>
        /// <param name="inTransactionName">Name of the transaction to start.</param>
        /// <returns>SQL transaction that was started.</returns>
        public SqlTransaction StartSISTransaction(string inTransactionName)
        {
            StartTimer(); 

            SISDispatchRequestTableAdapter forTransaction = new SISDispatchRequestTableAdapter();
            SqlConnection toOpen = forTransaction.GetConnection();
            toOpen.Open();
            return toOpen.BeginTransaction(inTransactionName);

        }

        /// <summary>
        /// Start a SQL transaction with the given transaction name and with the specified connection string name.
        /// 
        /// If data access monitoring is enabled, monitoring is also started for this connection/transaction.
        /// </summary>
        /// <param name="inTransactionName">Name of the transaction.</param>
        /// <param name="inConnectionStringName">Connection String Name, refers to an entry in the Connection String section of the
        /// applicatinon configuration file.</param>
        /// <returns></returns>
        public SqlTransaction StartSQLTransaction(string inTransactionName, string inConnectionStringName)
        {
            StartTimer(); 

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[inConnectionStringName].ConnectionString;

            SqlConnection forTransaction = new SqlConnection(connectionString);
            forTransaction.Open();
            return forTransaction.BeginTransaction(inTransactionName);
        }


        /// <summary>
        /// Commit the specified transaction.  If data monitor is enabled, this method will also stop 
        /// monitoring for this instance.  
        /// </summary>
        /// <param name="inToCommit">Transaction to commit.</param>
        public void CommitTransaction(SqlTransaction inToCommit)
        {
            inToCommit.Commit();

            if (inToCommit.Connection != null
                && inToCommit.Connection.State != ConnectionState.Closed)
                inToCommit.Connection.Close();

            StopTimer(); 
        }

        /// <summary>
        /// Commit the specified transaction.  If data monitor is enabled, this method will also stop 
        /// monitoring for this instance.  
        /// This method allows specification of additional logging information.
        /// </summary>
        /// <param name="inToCommit">Transaction to commit.</param>
        public void CommitTransaction(SqlTransaction inToCommit, string inAdditionalLogInformation)
        {
            inToCommit.Commit();

            if (inToCommit.Connection != null
                && inToCommit.Connection.State != ConnectionState.Closed)
                inToCommit.Connection.Close();

            StopTimer(inAdditionalLogInformation);
        }


        /// <summary>
        /// Rollback the specified transaction.  If data monitoring is enabled, this method will also stop 
        /// monitoring of this instance. 
        /// </summary>
        /// <param name="inToRollback"></param>
        public void RollbackTransaction(SqlTransaction inToRollback)
        {
            inToRollback.Rollback();

            if (inToRollback.Connection != null
                && inToRollback.Connection.State != ConnectionState.Closed)
                inToRollback.Connection.Close();

            StopTimer(); 
        }

        /// <summary>
        /// Rollback the specified transaction.  If data monitoring is enabled, this method will also stop 
        /// monitoring of this instance. 
        /// This method allows specification of additional logging information. 
        /// </summary>
        /// <param name="inToRollback"></param>
        public void RollbackTransaction(SqlTransaction inToRollback, string inAdditionalLogInformation)
        {
            try
            {
                inToRollback.Rollback();
            }
            catch (Exception ex)
            {
            }

            if (inToRollback.Connection != null
                && inToRollback.Connection.State != ConnectionState.Closed)
                inToRollback.Connection.Close();

            StopTimer(inAdditionalLogInformation);
        }


    }

    public sealed class SmartDataReader
    {
        SqlDataReader _reader = null;
        public SmartDataReader(SqlDataReader inReader)
        {
            _reader = inReader;
        }

        public Nullable<int> GetInt32(String column)
        {
            Nullable<int> data = null;

            if (!_reader.IsDBNull(_reader.GetOrdinal(column)))
                data = (int)_reader[column];
            return data;
        }

        public Nullable<short> GetInt16(String column)
        {
            Nullable<short> data = null;

            if (!_reader.IsDBNull(_reader.GetOrdinal(column)))
                data = (short)_reader[column];
            return data;
        }

        public Nullable<float> GetFloat(String column)
        {
            Nullable<float> data = null;

            if (!_reader.IsDBNull(_reader.GetOrdinal(column)))
                data = (float)_reader[column];
            return data;
        }

        public Nullable<Guid> GetGuid(String column)
        {
            Nullable<Guid> data = null;
            if (!_reader.IsDBNull(_reader.GetOrdinal(column)))
                data = (Guid)_reader[column];
            return data;
        }


        public Nullable<bool> GetBoolean(String column)
        {
            Nullable<bool> data = null;

            if (!_reader.IsDBNull(_reader.GetOrdinal(column)))
                data = (bool)_reader[column];
            return data;
        }

        public String GetString(String column)
        {
            String data = null;

            if (!_reader.IsDBNull(_reader.GetOrdinal(column)))
                data = (string)_reader[column];
            return data;
        }

        public Nullable<DateTime> GetDateTime(String column)
        {
            Nullable<DateTime> data = null;

            if (!_reader.IsDBNull(_reader.GetOrdinal(column)))
                data = (DateTime)_reader[column];
            return data;
        }
    }
}
