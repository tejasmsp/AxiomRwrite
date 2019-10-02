using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace QuickFormService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        static bool isDebug = true;
        static void Main()
        {

            if (isDebug == false)
            {
                try
                {
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[]
                    {
                new QuickFormService()
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
                    QuickFormService obj = new QuickFormService();
                    obj.GenerateDocuments();
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
