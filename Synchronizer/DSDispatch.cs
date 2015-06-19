using System;
using System.Data.SqlClient;
using System.Data;

namespace DNACircSynchronizer.Processes.DSDispatchTableAdapters
{
    public partial class DispatchRequestTableAdapter : global::System.ComponentModel.Component
    {
        public DispatchRequestTableAdapter(SqlTransaction inTransaction)
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
    
    
    public partial class DSDispatch {
    }
}
