using System;
using System.ComponentModel; 
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using DNACircSynchronizer.Processes;
using System.Configuration; 


namespace DNACircSynchronizer.Services
{
    static class Program 
    {
       
#if ( DEBUG )
        static void Main(string[] args)
        {
            new ServiceProxy(); 
        }
#endif
    }

    public class ServiceProxy
    {
        public ServiceProxy()
        {
            DNASynchronizer toStart = new DNASynchronizer();

            System.Threading.Thread.Sleep(5000000);
        }
    }
}
