using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AxiomAutomation
{
    static class Program
    {
        static bool isDebug = false;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            if (isDebug == false)
            {
                try
                {
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[]
                    {
                    new AxiomAutomation()
                    };
                    ServiceBase.Run(ServicesToRun);
                }
                catch (Exception ex)
                {
                    Log.ServicLog("----------- EXCEPTION APP.CONFIG -------------------");
                    Log.ServicLog(Convert.ToString(ex.Message));
                    Log.ServicLog(Convert.ToString(ex.StackTrace));
                    Log.ServicLog(Convert.ToString(ex.InnerException));
                    Log.ServicLog("--------------------------------------------");
                }
            }
            else
            {
                try
                {
                  AxiomAutomation obj = new AxiomAutomation();
                  obj.AutomationMain();
                }
                catch (Exception ex)
                {
                    Log.ServicLog("----------- EXCEPTION APP.CONFIG -------------------");
                    Log.ServicLog(Convert.ToString(ex.Message));
                    Log.ServicLog(Convert.ToString(ex.StackTrace));
                    Log.ServicLog(Convert.ToString(ex.InnerException));
                    Log.ServicLog("--------------------------------------------");
                }
            }
            
        }

    }
}
