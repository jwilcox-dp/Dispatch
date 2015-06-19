using System;
using System.Data.SqlClient;
using System.Data;


namespace DNACircSynchronizer.Processes.DSSISTableAdapters 
{
    public partial class SISDispatchRequestTableAdapter : global::System.ComponentModel.Component
    {
        public SISDispatchRequestTableAdapter(SqlTransaction inTransaction)
            : base()
        {
            if (inTransaction == null)
                return;

            foreach (SqlCommand command in CommandCollection)
            {
                command.Connection = inTransaction.Connection;
                command.Transaction = inTransaction;
            }
            TAHelper.SetTransaction(inTransaction, Adapter);
        }


        public SqlConnection GetConnection()
        {
            return Connection;
        }

        
        public void SetConnection(SqlConnection inConnection)
        {
            foreach (SqlCommand command in CommandCollection)
            {
                command.Connection = inConnection;
            }
            TAHelper.SetConnection(inConnection, Adapter); 
        }
        

        public void SetTransaction(SqlTransaction inTransaction)
        {
            foreach (SqlCommand command in CommandCollection)
            {
                command.Connection = inTransaction.Connection;
                command.Transaction = inTransaction;
            }
            TAHelper.SetTransaction(inTransaction, Adapter);
        }
    }

    public partial class ComplaintHistoryTableAdapter : global::System.ComponentModel.Component
    {
        public ComplaintHistoryTableAdapter(SqlTransaction inTransaction)
            : base()
        {
            if (inTransaction == null)
                return;

            foreach (SqlCommand command in CommandCollection)
            {
                command.Connection = inTransaction.Connection;
                command.Transaction = inTransaction;
            }
            TAHelper.SetTransaction(inTransaction, Adapter);
        }


        public SqlConnection GetConnection()
        {
            return Connection;
        }


        public void SetConnection(SqlConnection inConnection)
        {
            foreach (SqlCommand command in CommandCollection)
            {
                command.Connection = inConnection;
            }
            TAHelper.SetConnection(inConnection, Adapter); 
        }


        public void SetTransaction(SqlTransaction inTransaction)
        {
            foreach (SqlCommand command in CommandCollection)
            {
                command.Connection = inTransaction.Connection;
                command.Transaction = inTransaction;
            }
            TAHelper.SetTransaction(inTransaction, Adapter);
        }
    }

    public partial class MemoHistoryTableAdapter : global::System.ComponentModel.Component
    {
        public MemoHistoryTableAdapter(SqlTransaction inTransaction)
            : base()
        {
            if (inTransaction == null)
                return;

            foreach (SqlCommand command in CommandCollection)
            {
                command.Connection = inTransaction.Connection;
                command.Transaction = inTransaction;
            }
            TAHelper.SetTransaction(inTransaction, Adapter);
        }


        public SqlConnection GetConnection()
        {
            return Connection;
        }


        public void SetConnection(SqlConnection inConnection)
        {
            foreach (SqlCommand command in CommandCollection)
            {
                command.Connection = inConnection;
            }
            TAHelper.SetConnection(inConnection, Adapter); 
        }


        public void SetTransaction(SqlTransaction inTransaction)
        {
            foreach (SqlCommand command in CommandCollection)
            {
                command.Connection = inTransaction.Connection;
                command.Transaction = inTransaction;
            }
            TAHelper.SetTransaction(inTransaction, Adapter);
        }
    }
}



namespace DNACircSynchronizer.Processes {
    
    
    public partial class DSSIS {
    }
}
