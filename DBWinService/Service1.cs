using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DBWinService
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["TimeIntervalinNilliSeconds"]); //number in milisecinds  
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            new DataAccess().GetResults();
        }
    }
}
