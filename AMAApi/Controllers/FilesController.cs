using FarmsApi.Services;
using System;
using System.Configuration;
using System.IO;
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

            string WorkerPath = root + workerid.ToString();

            if (!Directory.Exists(WorkerPath))
            {
                Directory.CreateDirectory(WorkerPath);

            }

            root = WorkerPath;

            var provider = new MultipartFormDataStreamProvider(root);

            var file = await Request.Content.ReadAsMultipartAsync(provider);


            string fileList = "";
            for (int i = 0; i < file.FileData.Count; i++)
            {
                var source = file.FileData[i].LocalFileName;
                var dest = root + "/" + file.FileData[i].Headers.ContentDisposition.FileName.Replace("\"", "");
                //dest = filterFilename(dest);

                if (File.Exists(dest))
                {
                    File.Delete(dest);
                }


                File.Move(source, dest);
                if (i == 0)
                {
                    fileList += Path.GetFileName(dest);

                }
                else
                {
                    fileList += "," + Path.GetFileName(dest);
                }

            }


            return Ok(fileList);
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


                SmtpClient client = new SmtpClient("82.166.0.201", 25);
                client.Credentials = new System.Net.NetworkCredential("office@ofekmanage.com", "jadekia556"); //
                client.EnableSsl = false;

                string Body = "<html dir='rtl'><div style='text-align:right'><b>שלום רב,</b>" + "<br/>" + "מצ''ב קבצים חדשים.</div><br/>";// </html>";

                Body += text + "</html>";

                string Title = "הודעה חדשה - " +CurrentUser.FirstName + " " + CurrentUser.LastName;

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
        public IHttpActionResult Delete(string filename)
        {
            string root = HttpContext.Current.Server.MapPath("~/Uploads/");
            File.Delete(root + filename);
            return Ok();
        }




    }
}
