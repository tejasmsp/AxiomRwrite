using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace UploadSweepService
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

            if (isDebug == true)
            {
                try
                {
                    UploadSweepService obj = new UploadSweepService();
                    obj.UploadOtherRecords();
                    obj.CreateOrder();

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
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[]
                    {
                        new UploadSweepService()
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


        }
    }
}
