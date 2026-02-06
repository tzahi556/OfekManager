using FarmsApi.Services;
using Google.Cloud.Vision.V1;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace FarmsApi.Controllers
{
    [RoutePrefix("files")]
    public class FilesController : ApiController
    {


        [Route("upload/{folder}/{workerid}")]
        [HttpPost]
        public async Task<IHttpActionResult> Upload(string folder, int workerid)
        {

            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);






            string root = HttpContext.Current.Server.MapPath("~/Uploads/");

            string tempRoot = root;

            string WorkerPath = root + workerid.ToString();

            if (!Directory.Exists(WorkerPath))
            {
                Directory.CreateDirectory(WorkerPath);

            }

            root = WorkerPath;

            var provider = new MultipartFormDataStreamProvider(root);

            var file = await Request.Content.ReadAsMultipartAsync(provider);

            if (folder.Contains("taz_"))
            {




                //string credential_path = tempRoot + "CrediJson/cerdi.json";   //HttpContext.Current.Server.MapPath("~/Uploads/CrediJson/cerdi.json");
                //// יצירת סרביס אקאונט ואז יצירת מפתח ולהוריד את הגייסון
                //System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credential_path);


                //bool IsImageTazOk = false;
                bool IsImageTazOk = true;



                //for (int i = 0; i < file.FileData.Count; i++)
                //{
                //    var source = file.FileData[i].LocalFileName;

                //    var client = ImageAnnotatorClient.Create();
                //    var image = Image.FromFile(source); //FromUri("https://ofekmanage.com/api/uploads/5397/%D7%9E%D7%99%D7%A8%D7%91%20%D7%9E%D7%98%D7%A8%D7%99%20%D7%AA.%D7%96.jpg");
                //    var labels = client.DetectText(image);

                //    string Taz = folder.Replace("taz_", "");

                //    if (string.IsNullOrEmpty(Taz) || Taz == "null") return Ok("NoTaz");

                //    var LabelContainsTaz = labels.Where(x => x.Description.Length >= 7 && Taz.Contains(x.Description)).ToList();

                //    if (LabelContainsTaz.Count > 0)
                //    {
                //        IsImageTazOk = true;
                //    }

                //}

                if (!IsImageTazOk)
                {

                    for (int i = 0; i < file.FileData.Count; i++)
                    {
                        var source = file.FileData[i].LocalFileName;
                        if (File.Exists(source))
                        {
                            File.Delete(source);
                        }

                    }



                    return Ok(false);
                }
                ;


            }





            //string fileList = "";
            //for (int i = 0; i < file.FileData.Count; i++)
            //{
            //    var source = file.FileData[i].LocalFileName;

            //    var dest = root + "/" + file.FileData[i].Headers.ContentDisposition.FileName.Replace("\"", "");
            //    //dest = filterFilename(dest);

            //    if (File.Exists(dest))
            //    {
            //        try
            //        {
            //            File.Delete(dest);
            //        }
            //        catch (Exception ex) { }
            //    }

            //    try
            //    {
            //        File.Move(source, dest);
            //    }
            //    catch (Exception ex)
            //    {

            //        //File.Delete(dest);
            //    }





            //    if (i == 0)
            //    {
            //        fileList += Path.GetFileName(dest);

            //    }
            //    else
            //    {
            //        fileList += "," + Path.GetFileName(dest);
            //    }

            //}
            //return Ok(fileList);

            string fileList = "";

            for (int i = 0; i < file.FileData.Count; i++)
            {
                var source = file.FileData[i].LocalFileName;

                var fileName = file.FileData[i].Headers.ContentDisposition.FileName.Trim('"');
                var dest = Path.Combine(root, fileName);

                // שם זמני ייחודי באותה תיקייה
                var tempDest = Path.Combine(root, Guid.NewGuid().ToString("N") + "_" + fileName);

                // 1) קודם כל מעבירים מה-temp של Multipart ל-tempDest (כדי לא לאבד את source)
                TryMoveWithRetry(source, tempDest);

                // 2) עכשיו מחליפים את הקובץ הסופי
                ReplaceWithRetry(tempDest, dest);

                fileList += (i == 0) ? Path.GetFileName(dest) : "," + Path.GetFileName(dest);
            }

            return Ok(fileList);

        }

        static void TryMoveWithRetry(string from, string to)
        {
            for (int i = 0; i < 6; i++)
            {
                try { File.Move(from, to); return; }
                catch (IOException) { System.Threading.Thread.Sleep(150); }
                catch (UnauthorizedAccessException) { System.Threading.Thread.Sleep(150); }
            }

            // fallback
            File.Copy(from, to, true);
            File.Delete(from);
        }

        static void ReplaceWithRetry(string from, string to)
        {
            for (int i = 0; i < 8; i++)
            {
                try
                {
                    // אם קיים יעד – מוחקים (עם ניסיון חוזר)
                    if (File.Exists(to))
                    {
                        TryDeleteWithRetry(to);
                    }

                    File.Move(from, to);
                    return;
                }
                catch (IOException) { System.Threading.Thread.Sleep(200); }
                catch (UnauthorizedAccessException) { System.Threading.Thread.Sleep(200); }
            }

            // אם עדיין נעול: נשאיר את הקובץ החדש בשם אחר ולא נאבד אותו
            var dir = Path.GetDirectoryName(to);
            if (string.IsNullOrEmpty(dir))
                dir = Directory.GetCurrentDirectory(); // או Directory.GetCurrentDirectory()

            var fallback = Path.Combine(dir, "NEW_" + Path.GetFileName(to));
            File.Move(from, fallback);
        }

        static void TryDeleteWithRetry(string path)
        {
            for (int i = 0; i < 8; i++)
            {
                try { File.Delete(path); return; }
                catch (IOException) { System.Threading.Thread.Sleep(200); }
                catch (UnauthorizedAccessException) { System.Threading.Thread.Sleep(200); }
            }
        }





        [Route("uploadformail/{folder}/{workerid}/{text}")]
        [HttpPost]
        public async Task<IHttpActionResult> UploadForMail(string folder, int workerid, string text)
        {

            if (folder == "send") return Ok("true");


            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            string root = HttpContext.Current.Server.MapPath("~/Uploads/");

            string WorkerPath = root + workerid.ToString();

            if (!Directory.Exists(WorkerPath))
            {
                Directory.CreateDirectory(WorkerPath);

            }

            root = WorkerPath;

            var provider = new MultipartFormDataStreamProvider(root);

            var file = await Request.Content.ReadAsMultipartAsync(provider);


            string fileList = "";


            try
            {

                var CurrentUser = UsersService.GetCurrentUser();

                string MailTo = ConfigurationSettings.AppSettings["MailTo"].ToString();
                string SmtpHost = ConfigurationSettings.AppSettings["SmtpHost"].ToString();
                string MailUser = ConfigurationSettings.AppSettings["MailUser"].ToString();
                string MailPassword = ConfigurationSettings.AppSettings["MailPassword"].ToString();

                SmtpClient client = new SmtpClient(SmtpHost, 25);
                client.Credentials = new System.Net.NetworkCredential(MailUser, MailPassword); //
                client.EnableSsl = false;

                string Body = "<html dir='rtl'><div style='text-align:right'><b>שלום רב,</b>" + "<br/>" + "מצ''ב קבצים חדשים.</div><br/>";// </html>";

                Body += text + "</html>";

                string Title = "הודעה חדשה - " + CurrentUser.FirstName + " " + CurrentUser.LastName;

                MailMessage actMSG = new MailMessage(
                                        "office@ofekmanage.com",
                                         MailTo,
                                         Title,
                                         Body);


                actMSG.IsBodyHtml = true;
                for (int i = 0; i < file.FileData.Count; i++)
                {
                    FileStream fStream = new FileStream(file.FileData[i].LocalFileName, FileMode.Open);

                    Attachment attachment = new Attachment(fStream, file.FileData[i].Headers.ContentDisposition.FileName.Replace("\"", ""), file.FileData[i].Headers.ContentType.MediaType);

                    actMSG.Attachments.Add(attachment);

                    // fStream.Close();

                }
                client.Send(actMSG);




            }
            catch (Exception ex)
            {
                return Ok("false");
            }
            finally
            {


            }





            return Ok(fileList);
        }




        public string filterFilename(string filename)
        {
            var suffix = 0;

            while (true)
            {
                suffix++;
                var NewFileName = Path.GetFileNameWithoutExtension(filename) + "_" + suffix;
                var OldFileName = Path.GetFileNameWithoutExtension(filename);
                var TempFilePath = filename.Replace(OldFileName, NewFileName);

                if (!File.Exists(TempFilePath))
                {
                    filename = TempFilePath;
                    break;
                }

            }
            return filename;
        }

        [Route("delete")]
        [HttpGet]
        public IHttpActionResult Delete(string filename, string workerid)
        {
            string root = HttpContext.Current.Server.MapPath("~/Uploads/");
            try
            {
                File.Delete(root + "/" + workerid + "/" + filename);
            }
            catch (Exception ex)
            {

            }
            return Ok();
        }




    }
}
