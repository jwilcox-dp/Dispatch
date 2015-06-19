using System;
using System.Configuration;
using System.Configuration.Install; 
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using DNACircSynchronizer.Processes;
using System.Threading;

namespace DNACircSynchronizer.Services
{
    public partial class DNASynchronizer : ServiceBase
    {
		private static EventLog _eventLog = new EventLog("Application");

        public DNASynchronizer()
        {
            try
            {
                _eventLog = new EventLog("Application");
                _eventLog.Source = "DNASynchronizer 1.0.0.0"; 
                _eventLog.WriteEntry("DNASynchronizer Constructor", EventLogEntryType.Information);
                ServiceName = "DNASynchronizer 1.0.0.0";
            }
            catch (Exception ex)
            { }

            InitializeComponent();

#if DEBUG
            OnStart(null);
#endif 
        }

        // The main entry point for the process
#if (!DEBUG)
        static void Main()
        {
            System.ServiceProcess.ServiceBase[] ServicesToRun;

            ServicesToRun = new System.ServiceProcess.ServiceBase[] { new DNASynchronizer() };
            System.ServiceProcess.ServiceBase.Run(ServicesToRun);
        }
#endif

        protected override void OnStart(string[] args)
        {
            _eventLog.WriteEntry("DNASynchronizer Starting Up", EventLogEntryType.Information);

            Thread SISToDispatch = new Thread(new ThreadStart(Synchronizer.SynchronizeSISToDispatchTables));
            SISToDispatch.Start();
            _eventLog.WriteEntry("SIS to Dispatch Started", EventLogEntryType.Information);

            Thread DispatchToSis = new Thread(new ThreadStart(Synchronizer.SynchronizeDispatchToSISTables));
            DispatchToSis.Start();
            _eventLog.WriteEntry("Dispatch to SIS Started", EventLogEntryType.Information);

            Thread CJToSIS = new Thread(new ThreadStart(Synchronizer.SynchronizeCJToSISTables));
            CJToSIS.Start();
            _eventLog.WriteEntry("CJ To SIS Started Started", EventLogEntryType.Information);
        }

        protected override void OnStop()
        {
            _eventLog.WriteEntry("Service Stopped", EventLogEntryType.Information);
        }
    }

    [RunInstallerAttribute(true)]
    public class DNASynchronizerInstaller : Installer
    {
        private ServiceInstaller serviceInstaller;
        private ServiceProcessInstaller processInstaller;

        public DNASynchronizerInstaller()
        {
            processInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            serviceInstaller.ServiceName = "DNASynchronizer 1.0.0.0";

            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
        }
    }
}
