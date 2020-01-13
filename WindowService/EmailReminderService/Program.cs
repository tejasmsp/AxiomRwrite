using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace EmailReminderService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static bool isDebug = false;
        static void Main()
        {
        #if DEBUG
                    isDebug = true;
        #endif

            if (isDebug == false)
            {
                try
                {
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[]
                    {
                new EmailReminderService()
                    };
                    ServiceBase.Run(ServicesToRun);
                }
                catch (Exception ex)
                {
                    Log.Echo("----------- EXCEPTION APP.CONFIG -------------------");
                    Log.Echo(Convert.ToString(ex.Message));
                    Log.Echo(Convert.ToString(ex.StackTrace));
                    Log.Echo(Convert.ToString(ex.InnerException));
                    Log.Echo("--------------------------------------------");
                }

            }
            else
            {
                try
                {
                    EmailReminderService obj = new EmailReminderService();
                    // obj.ServiceExecution(); //Email Reminder Service
                    // obj.SendMonthlyEmail(); //Axiom Service
                    obj.AxiomServiceExecution(); //Axiom Service
                }
                catch (Exception ex)
                {
                    Log.Echo("----------- EXCEPTION APP.CONFIG -------------------");
                    Log.Echo(Convert.ToString(ex.Message));
                    Log.Echo(Convert.ToString(ex.StackTrace));
                    Log.Echo(Convert.ToString(ex.InnerException));
                    Log.Echo("--------------------------------------------");
                }
            }

        }
    }
}
