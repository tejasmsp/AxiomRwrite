using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.IO;
using System.Reflection;

namespace AxiomAutomation
{
    public class Log
    {
        public Log()
        {
        }
        
        public static string EchoPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "\\log.txt";
        public static string ServiceLogPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "\\ServiceLog.txt";

        public static bool EchoEnabled = Boolean(System.Configuration.ConfigurationManager.AppSettings["LogEnabled"]);
        public static bool ServiceLogEnabled = Boolean(System.Configuration.ConfigurationManager.AppSettings["ServiceLogEnabled"]);

        public static bool Boolean(object readField)
        {
            if ((readField != null))
            {
                if (readField.GetType() != typeof(System.DBNull))
                {
                    bool x;
                    bool.TryParse(Convert.ToString(readField), out x);
                    return x;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Echo(object obj)
        {
            if (obj == null)
                return;

            string msg = obj.ToString();

            if (EchoPath == null)
                return;

            if (!EchoEnabled)
                return;

            using (StreamWriter sw = new StreamWriter(EchoPath, true))
            {
                sw.WriteLine(string.Format("{0:d}  {0:hh:mm:ss}    {1}", DateTime.Now, msg));
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void ServicLog(object obj)
        {
            if (obj == null)
                return;

            string msg = obj.ToString();

            if (ServiceLogPath == null)
                return;

            if (!ServiceLogEnabled)
                return;
            using (StreamWriter sw = new StreamWriter(ServiceLogPath, true))
            {
                sw.WriteLine(string.Format("{0:d}  {0:hh:mm:ss}  {1} ", DateTime.Now, msg));
            }
        }
    }
    
}
