using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

namespace Axiom.Web.API
{
    public class Document
    {
        public string MoveLocalToServerFile(string fileName, string batchId, string localStoragePath, string serverStoragePath, int orderNo, string partNo)
        {
            string fileDiskNameResult = "";
            DirectoryInfo Localdirectory = new DirectoryInfo(localStoragePath);
            if (!Localdirectory.Exists)
            {
                Localdirectory = Directory.CreateDirectory(localStoragePath);
            }
            if (!Directory.Exists(serverStoragePath))
            {
                Directory.CreateDirectory(serverStoragePath);
            }
            //Move Step 6 file to Upload Batch folder
            string UploadBatchPath = ConfigurationManager.AppSettings["TempStorageDirectory"].ToString() + "/" + orderNo + "/" + partNo.Replace(",", "-");
            if (!Directory.Exists(UploadBatchPath))
            {
                Directory.CreateDirectory(UploadBatchPath);
            }
            foreach (var file in Localdirectory.GetFiles("*" + batchId + "_" + fileName + "*"))
            {

                var objFile = file.Name.Split('_');
                if (objFile.Length > 0 && objFile[1] == batchId && objFile[2] == fileName)
                {

                    string fileType = file.Extension;
                    Aspose.Pdf.License license = new Aspose.Pdf.License();
                    license.SetLicense("Aspose.Pdf.lic");

                    Aspose.Words.License license1 = new Aspose.Words.License();
                    license1.SetLicense("Aspose.Words.lic");

                    string fileDiskName = Guid.NewGuid().ToString();
                    if (fileType.ToUpper() == ".DOC" || fileType.ToUpper() == ".DOCX")
                    {
                        fileDiskName += ".pdf";
                        Aspose.Words.Document doc = new Aspose.Words.Document(file.FullName);
                        doc.Save(Path.Combine(serverStoragePath, fileDiskName));
                        doc.Save(Path.Combine(UploadBatchPath, fileDiskName));
                    }
                    else if (fileType.ToUpper() == ".PDF")
                    {
                        fileDiskName += ".pdf";
                        Aspose.Pdf.Document doc = new Aspose.Pdf.Document(file.FullName);
                        doc.Save(Path.Combine(serverStoragePath, fileDiskName));
                        doc.Save(Path.Combine(UploadBatchPath, fileDiskName));
                    }
                    else
                    {
                        fileDiskName += file.Extension;
                        file.CopyTo(Path.Combine(serverStoragePath, fileDiskName));
                        file.CopyTo(Path.Combine(UploadBatchPath, fileDiskName));
                    }
                    fileDiskNameResult = fileDiskName;
                    //file.Delete();
                }
            }
            return fileDiskNameResult;
        }

        public void DeleteAttchFile(string fileName, string batchId, string localStoragePath)
        {
            DirectoryInfo Localdirectory = new DirectoryInfo(localStoragePath);
            if (!Localdirectory.Exists)
            {
                Localdirectory = Directory.CreateDirectory(localStoragePath);
            }
            foreach (var file in Localdirectory.GetFiles("*" + batchId + "_" + fileName + "*"))
            {
                file.Delete();
            }

        }
        public HttpResponseMessage DownloadFileFromServer(string fileDiskName, string originalFileName, int orderNo, int partNo = 0)
        {
            string fileType = Path.GetExtension(originalFileName);

            if (!string.IsNullOrEmpty(fileType) && (fileType.ToUpper() == ".DOC" || fileType.ToUpper() == ".DOCX"))
            {
                originalFileName = Path.GetFileNameWithoutExtension(originalFileName) + ".pdf";
            }
            string filePath = ConfigurationManager.AppSettings["UploadRoot"].ToString();
            if (partNo > 0)
            {
                filePath += "/" + orderNo + "/" + partNo + "/" + fileDiskName;

            }
            else
            {
                filePath += "/" + orderNo + "/" + fileDiskName;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);
                    ms.Write(bytes, 0, (int)file.Length);

                    HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                    httpResponseMessage.Content = new ByteArrayContent(bytes.ToArray());
                    httpResponseMessage.Content.Headers.Add("x-filename", originalFileName);
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    httpResponseMessage.Content.Headers.ContentDisposition.FileName = originalFileName;
                    httpResponseMessage.StatusCode = HttpStatusCode.OK;
                    return httpResponseMessage;
                }
            }

        }
    }
}