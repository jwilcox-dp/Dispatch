using System;
using System.Data.Odbc;
using System.Data;

namespace DNACircSynchronizer.Processes {
    
    
    public partial class DSCJ {
    }
}

namespace DNACircSynchronizer.Processes.DSCJTableAdapters {
    
    
    public partial class SERVICE_HISTORYTableAdapter 
    
    {
        public OdbcConnection GetConnection()
        {
            return Connection;
        }


        public void SetConnection(OdbcConnection inConnection)
        {
            foreach (OdbcCommand command in CommandCollection)
            {
                command.Connection = inConnection;
            }
        }

        public void SetTransaction(OdbcTransaction inTransaction)
        {
            foreach (OdbcCommand command in CommandCollection)
            {
                command.Connection = inTransaction.Connection;
                command.Transaction = inTransaction;
            }
        }
    }
}
