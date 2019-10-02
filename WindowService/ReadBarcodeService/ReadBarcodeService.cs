using Aspose.Words;
using Aspose.Words.Drawing;
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
using Aspose.BarCode.BarCodeRecognition;
using System.Drawing;

namespace ReadBarcodeService
{
    public partial class ReadBarcodeService : ServiceBase
    {
        Timer serviceTimer = null;
        public ReadBarcodeService()
        {
            Log.ServicLog("======================================================================");
            Log.ServicLog("Initialize");
            Log.ServicLog("Duration Time :  " + ConfigurationManager.AppSettings["Duration"].ToString() + " min");
            serviceTimer = new Timer();
            serviceTimer.Elapsed += serviceTimer_Elapsed;
            serviceTimer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["Duration"].ToString()) * 60 * 1000;
            serviceTimer.Enabled = true;
            serviceTimer.Start();
            InitializeComponent();
        }
        private void serviceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Log.ServicLog(DateTime.Now.ToString() + " TIMER ELAPSED");
            ReadBarCode();
        }

        public void ReadBarCode()
        {
            Log.ServicLog("---------- SERVICE EXECUTED " + DateTime.Now.ToString() + "---------------");
            string sourceRoot = ConfigurationManager.AppSettings["ReadBarCodeSourceRoot"];
            string DestinationRoot = ConfigurationManager.AppSettings["ReadBarCodeDestination"];
            DirectoryInfo di = new DirectoryInfo(sourceRoot);
            DirectoryInfo directoryDestination = new DirectoryInfo(DestinationRoot);
            try
            {
                if (di.Exists && directoryDestination.Exists)
                {
                    FileInfo[] filesInDir = di.GetFiles().ToArray();
                    if (filesInDir != null && filesInDir.Count() > 0)
                    {
                        foreach (FileInfo item in filesInDir)
                        {
                            string dataDir = Path.Combine(sourceRoot + item.Name);

                            Aspose.BarCode.License licence = new Aspose.BarCode.License();
                            licence.SetLicense("Aspose.BarCode.lic");

                            Aspose.Pdf.License lic = new Aspose.Pdf.License();
                            lic.SetLicense("Aspose.Pdf.lic");

                            if (!string.IsNullOrEmpty(item.Name) && Path.GetExtension(item.Name.ToLower()) == ".doc")
                            {
                                Document doc = new Document(dataDir);
                                NodeCollection shapes = doc.GetChildNodes(NodeType.Shape, true, false);
                                foreach (Shape shape in shapes)
                                {
                                    if (shape.HasImage)
                                    {
                                        MemoryStream imgStream = new MemoryStream();
                                        shape.ImageData.Save(imgStream);
                                        BarCodeReader barCodeReader = new BarCodeReader(new Bitmap(imgStream), BarCodeReadType.QR);
                                        Log.ServicLog("---------- Recognizing barcode ----------" + DateTime.Now.ToString() + "----------");
                                        while (barCodeReader.Read())
                                        {
                                            CopyFile(barCodeReader, DestinationRoot, dataDir, item.Name);
                                        }
                                        barCodeReader.Close();
                                    }
                                }
                            }
                            else if (!string.IsNullOrEmpty(item.Name) && Path.GetExtension(item.Name.ToLower()) == ".pdf")
                            {
                                Aspose.Pdf.Facades.PdfExtractor pdfExtractor = new Aspose.Pdf.Facades.PdfExtractor();
                                pdfExtractor.BindPdf(dataDir);
                                pdfExtractor.StartPage = 1;
                                pdfExtractor.EndPage = 1;
                                pdfExtractor.ExtractImage();
                                while (pdfExtractor.HasNextImage())
                                {
                                    MemoryStream imageStream = new MemoryStream();
                                    pdfExtractor.GetNextImage(imageStream);
                                    imageStream.Position = 0;
                                    Log.ServicLog("---------- Recognizing barcode ----------" + DateTime.Now.ToString() + "----------");
                                    BarCodeReader barcodeReader = new BarCodeReader(imageStream, BarCodeReadType.QR);
                                    while (barcodeReader.Read())
                                    {
                                        CopyFile(barcodeReader, DestinationRoot, dataDir, item.Name);
                                    }
                                    barcodeReader.Close();
                                }
                            }
                        }
                    }
                    else
                    {
                        Log.ServicLog("---------- There is no files in the directory  " + DateTime.Now.ToString() + "---------------");
                    }
                }
                else
                {
                    Log.ServicLog("Source file path OR Destination file path doesn't exists.");
                }

            }
            catch (Exception ex)
            {
                Log.ServicLog(ex.ToString());
            }
        }
        private void CopyFile(BarCodeReader barcodeReader, string DestinationRoot, string dataDir, string Name)
        {
            try
            {
                string[] CreateDirectory_OrderNo_PartNo = barcodeReader.GetCodeText().Split('-');
                string CreateDirectory_OrderNo = CreateDirectory_OrderNo_PartNo[0];
                string CreateDirectory_PartNo = CreateDirectory_OrderNo_PartNo[1];
                Log.ServicLog("---------- Order No =>" + CreateDirectory_OrderNo + " Part No=>" + CreateDirectory_PartNo + "----------" + DateTime.Now.ToString() + "----------");
                string destination_with_orderNo_partNo = string.Format("{0}{1}\\{2}", DestinationRoot, CreateDirectory_OrderNo, CreateDirectory_PartNo);
                if (!Directory.Exists(destination_with_orderNo_partNo))
                    Directory.CreateDirectory(destination_with_orderNo_partNo);

                File.Copy(dataDir, destination_with_orderNo_partNo + @"\" + Name, true);
            }
            catch (Exception ex)
            {
                Log.ServicLog("---------- Exception" + ex.Message + "  ----------" + DateTime.Now.ToString() + "----------");
            }
        }

        protected override void OnStart(string[] args)
        {
            Log.ServicLog("Service Started and Executed");
            ReadBarCode();
        }

        protected override void OnStop()
        {
            Log.ServicLog("Service Stopped");
        }
    }
}
