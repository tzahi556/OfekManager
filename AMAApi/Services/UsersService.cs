using FarmsApi.DataModels;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Web;
using System.Web.UI.WebControls;


namespace FarmsApi.Services
{
    public class UsersService
    {


        private static double? CheckifExistDouble(JToken jToken)
        {
            if (jToken == null) return 0;
            double res;
            bool Ok = Double.TryParse(jToken.ToString(), out res);
            if (Ok) return res;
            else return 0;
        }

        private static bool CheckifExistBool(JToken jToken)
        {
            if (jToken == null) return false;
            Boolean res = new Boolean();
            bool Ok = Boolean.TryParse(jToken.ToString(), out res);
            if (Ok)
                return res;
            else
                return false;
        }

        private static DateTime? CheckifExistDate(JToken jToken)
        {

            if (jToken == null) return null;
            DateTime res = new DateTime();
            bool Ok = DateTime.TryParse(jToken.ToString(), out res);
            if (Ok && res.Year > 1960) return res;

            else return null;
        }

        private static int CheckifExistInt(JToken jToken)
        {
            if (jToken == null) return 0;
            int res;
            bool Ok = Int32.TryParse(jToken.ToString(), out res);
            if (Ok) return res;
            else return 0;
        }

        private static string CheckifExistStr(JToken jToken)
        {

            if (jToken != null)

                return jToken.ToString();

            return "";

        }




        //*****************************************************************************

        public static List<User> GetUsers(string Role, bool IncludeDeleted = false)
        {
            using (var Context = new Context())
            {
                var CurrentUserFarmId = GetCurrentUser().Farm_Id;



                Context.Configuration.ProxyCreationEnabled = false;
                Context.Configuration.LazyLoadingEnabled = false;

                var Users = Context.Users.Where(u => u.Farm_Id == CurrentUserFarmId).OrderBy(x => x.FirstName).ToList();

                if (CurrentUserFarmId == 0)
                {
                    Users = Context.Users.ToList();
                }

                Users = FilterByUser(Users);
                Users = FilterRole(Users, Role);
                Users = FilterDeleted(Users, IncludeDeleted);

                return RemovePassword(Users);
            }
        }

        private static List<User> FilterByUser(List<User> Users)
        {
            var CurrentUser = GetCurrentUser();

            if (CurrentUser.Role == "instructor" || CurrentUser.Role == "profAdmin")
                return Users.Where(u => u.Role == "student" || u.Id == CurrentUser.Id).ToList();

            return Users;
        }

        private static List<User> FilterByFarm(List<User> Users)
        {
            var CurrentUserFarmId = GetCurrentUser().Farm_Id;
            if (CurrentUserFarmId != 0)
                return Users.Where(u => u.Farm_Id == CurrentUserFarmId).ToList();
            else
                return Users;
        }

        private static List<User> FilterDeleted(List<User> Users, bool IncludeDeleted)
        {
            if (IncludeDeleted)
            {
                return Users;
            }

            return Users.Where(u => !u.Deleted).ToList();
        }

        private static List<User> FilterRole(List<User> Users, string Role)
        {
            if (!string.IsNullOrEmpty(Role))
            {
                var Roles = Role.Split(',');
                if (Roles.Length > 1)
                {
                    List<User> ReturnUsers = new List<User>();
                    foreach (var role in Roles)
                    {
                        ReturnUsers.AddRange(Users.Where(u => u.Role == role).ToList());
                    }
                    return ReturnUsers;
                }
                else
                {
                    return Users.Where(u => u.Role == Role).ToList();
                }
            }
            return Users;
        }

        public static User GetUser(int? Id)
        {
            using (var Context = new Context())
            {
                if (Id.HasValue)
                {
                    return Context.Users.SingleOrDefault(u => u.Id == Id.Value && !u.Deleted);
                }
                else
                {
                    var CurrentUserId = GetCurrentUser().Id;
                    return Context.Users.SingleOrDefault(u => u.Id == CurrentUserId);
                }

            }
        }

        public static User GetSetUserEnter(int? Id, bool isForCartis = false)
        {


            using (var Context = new Context())
            {
                if (Id.HasValue)
                {
                    User us = Context.Users.SingleOrDefault(u => u.Id == Id.Value && !u.Deleted);
                    if (isForCartis)
                    {

                        if (us != null) //&& us.CurrentUserId != GetCurrentUser().Id)
                        {

                            //var users = Context.Users.Where(x => x.CurrentUserId == user.Id).ToList();

                            //users.ForEach(a =>
                            //{
                            //    a.IsTafus = false;
                            //    a.CurrentUserId = null;
                            //});



                            //Context.SaveChanges();

                            //// אם לא תפוס  תתפוס
                            //if (!us.IsTafus)
                            //{
                            //    User u = GetCurrentUser();
                            //    us.IsTafus = true;
                            //    us.CurrentUserId = u.Id;
                            //    us.TofesName = u.FirstName + " " + u.LastName;
                            //    Context.Entry(us).State = System.Data.Entity.EntityState.Modified;
                            //    Context.SaveChanges();

                            //    us.IsTafus = false;

                            //}
                            //else
                            //{
                            //    // אם תפוס אבל אותו משתמש תחזיר שלא תפוס
                            //    if (us.CurrentUserId == GetCurrentUser().Id)
                            //        us.IsTafus = false;

                            //}

                        }


                    }
                    return us;
                }
                else
                {
                    var CurrentUserId = GetCurrentUser().Id;
                    return Context.Users.SingleOrDefault(u => u.Id == CurrentUserId);
                }

            }
        }

        public static List<Testpdfs> GetPortfolios(int llx, int lly, int urx, int ury, string text, int font, int space, int id, int pagenumber)
        {


            using (var Context = new Context())
            {

                if (id == 0)
                {
                    var PdfScena = new Testpdfs();//Context.Testpdfs.Where(x => x.Id == id).FirstOrDefault();
                    PdfScena.llx = llx;
                    PdfScena.lly = lly;
                    PdfScena.urx = urx;
                    PdfScena.ury = ury;
                    PdfScena.Word = text;
                    PdfScena.Font = font;
                    PdfScena.Space = space;
                    PdfScena.PageNumber = pagenumber;

                    Context.Testpdfs.Add(PdfScena);
                    Context.SaveChanges();

                }


                if (id > 0)
                {
                    var PdfScena = Context.Testpdfs.Where(x => x.Id == id).FirstOrDefault();
                    PdfScena.llx = llx;
                    PdfScena.lly = lly;
                    PdfScena.urx = urx;
                    PdfScena.ury = ury;
                    PdfScena.Word = text;
                    PdfScena.Font = font;
                    PdfScena.Space = space;
                    PdfScena.PageNumber = pagenumber;

                    Context.Entry(PdfScena).State = System.Data.Entity.EntityState.Modified;
                    Context.SaveChanges();


                }

                PdfAPI pa = new PdfAPI();
                pa.TestPdfNewFromDB(llx, lly, urx, ury, text);



                //כאשר אני משתמש על הpdf 
                //var TestList = Context.Testpdfs.Where(x => x.PageNumber == pagenumber).OrderByDescending(x => x.Id).ToList();

                //return TestList;

                // כאשר אני עושה התאמה אז אני משתמש כאן

                var TestList = Context.Testpdfs.Where(x => x.PageNumber == pagenumber).OrderBy(x => x.Id).ToList();

                return TestList;

            }


        }

        public static List<Testpdfs> BindData(int id, string Comment, int pagenumber, string Value)
        {


            using (var Context = new Context())
            {


                if (Comment == "null") Comment = null;
                if (Value == "null") Value = null;

                if (id > 0)
                {
                    var PdfScena = Context.Testpdfs.Where(x => x.Id == id).FirstOrDefault();
                    PdfScena.Comment = Comment;
                    PdfScena.Value = Value;

                    Context.Entry(PdfScena).State = System.Data.Entity.EntityState.Modified;
                    Context.SaveChanges();


                }




                var TestList = Context.Testpdfs.Where(x => x.PageNumber == pagenumber).OrderBy(x => x.Id).ToList();

                return TestList;

            }


        }


        //****************************************** Workers
        public static List<Files> GetFiles(int Workerid)
        {


            using (var Context = new Context())
            {


                var WorkersFilesList = Context.Files.Where(x => x.WorkerId == Workerid).ToList();

                return WorkersFilesList;


            }
        }

        public static List<Workers> GetWorkers()
        {


            using (var Context = new Context())
            {

                //if (id >= 0)
                //{

                //    var WorkersList = Context.Workers.Where(x => x.Id == id).ToList();
                //    return WorkersList;

                //}



                var CurrentUserId = GetCurrentUser().Id;
                var CurrentRole = GetCurrentUser().Role;

                if (CurrentRole == "instructor")
                {
                    var WorkersListToRemove = Context.Workers.Where(x => x.UserId == CurrentUserId && (string.IsNullOrEmpty(x.FirstName) && string.IsNullOrEmpty(x.LastName) && string.IsNullOrEmpty(x.Taz))).ToList();

                    Context.Workers.RemoveRange(WorkersListToRemove);
                    Context.SaveChanges();


                    var WorkersList = Context.Workers.Where(x => x.UserId == CurrentUserId).ToList();

                    return WorkersList;
                }
                else
                {
                    return Context.Workers.ToList();

                }

            }
        }




      
        public static Workers GetWorker(int id)
        {


            using (var Context = new Context())
            {

                if (id >= 0)
                {
                    if (id == 0)
                    {

                        Workers newWork = new Workers();
                        newWork.UserId = GetCurrentUser().Id;

                        newWork.DateRigster = DateTime.Now;
                        Context.Workers.Add(newWork);
                        Context.SaveChanges();
                        id = newWork.Id;
                        // return newWork;


                    }
                    var Worker = Context.Workers.Where(x => x.Id == id).FirstOrDefault();
                    return Worker;

                }
                else
                {
                    var CurrentUserId = GetCurrentUser().Id;
                    return Context.Workers.SingleOrDefault(u => u.Id == CurrentUserId);

                }




            }
        }

        public static List<Workers> DeleteWorker(int Id)
        {
            using (var Context = new Context())
            {
                var Worker = Context.Workers.SingleOrDefault(u => u.Id == Id);

                Context.Workers.Remove(Worker);

                Context.SaveChanges();

                return GetWorkers();
            }
        }


        public static List<WorkerChilds> GetWorkerChilds(int Id)
        {
            using (var Context = new Context())
            {
                var WorkerChilds = Context.WorkerChilds.Where(x => x.WorkerId == Id).ToList();

                return WorkerChilds;


            }
        }


        public static Workers UpdateWorkerAndFiles(JArray dataObj, int type)
        {

            Workers w = UpdateWorker(dataObj[0].ToObject<Workers>());

            List<Files> f = dataObj[1].ToObject<List<Files>>();
            if (f != null) UpdateFilesObject(f, w);

            List<WorkerChilds> wc = dataObj[2].ToObject<List<WorkerChilds>>();
            if (wc != null) UpdateWorkerChildsObject(wc, w);


            if (type == 2 || type == 3)
            {
                PdfAPI pa = new PdfAPI();
                pa.CreatePDF(w);
            }

            //אם זה שמירה ושליחה למשרד

           
            if (type == 2)
            {

                try
                {

                    var CurrentUser = GetCurrentUser();
                    
                    string MailTo = ConfigurationSettings.AppSettings["MailTo"].ToString();


                    SmtpClient client = new SmtpClient("82.166.0.201", 25);
                    client.Credentials = new System.Net.NetworkCredential("office@ofekmanage.com", "jadekia556"); //
                    client.EnableSsl = false;

                    string Body = "<html dir='rtl'><div style='text-align:right'><b>שלום רב,</b>" + "<br/>" + "מצ''ב קובץ עובדת חדשה.</div><br/>";// </html>";

                    Body += " מנהל אזור -  " + CurrentUser.FirstName + " " + CurrentUser.LastName + "</html>";

                    string Title = "עובדת חדשה - " + w.FirstName + " " + w.LastName + " - " + w.Taz;

                    MailMessage actMSG = new MailMessage(
                                            "office@ofekmanage.com",
                                             MailTo,
                                            Title,
                                             Body);
                    
                    
                    actMSG.IsBodyHtml = true;
                    var BaseLinkSite = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/" + w.Id);
                    Attachment attachment = new Attachment(BaseLinkSite + "/OfekAllPdf.pdf");

                    actMSG.Attachments.Add(attachment);
                    client.Send(actMSG);

                    w.Status = "נשלח למשרד";


                }
                catch (Exception ex)
                {
                    w.Status = "תקלה שליחת נתונים";

                   // w.Status = ex.InnerException.ToString();
                }
                finally
                {
                    using (var Context = new Context())
                    {
                       
                        Context.Entry(w).State = System.Data.Entity.EntityState.Modified;
                        Context.SaveChanges();

                    }


                }

            }


            return w;
        }

        private static void CreatePDF(Workers w)
        {
            throw new NotImplementedException();
        }

        private static void UpdateWorkerChildsObject(List<WorkerChilds> objList, Workers w)
        {
            using (var Context = new Context())
            {

                foreach (WorkerChilds item in objList)
                {

                    item.WorkerId = w.Id;

                    if (item.Id == 0)
                    {
                        Context.WorkerChilds.Add(item);

                    }
                    else
                    {

                        Context.Entry(item).State = System.Data.Entity.EntityState.Modified;


                    }

                }

                try
                {

                    var result = Context.WorkerChilds.Where(p => p.WorkerId == w.Id).ToList();
                    IEnumerable<WorkerChilds> differenceQuery = result.Except(objList);

                    foreach (WorkerChilds item in differenceQuery)
                    {
                        Context.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    }



                }
                catch (Exception ex)
                {


                }

                Context.SaveChanges();


            }
        }



        private static void UpdateFilesObject(List<Files> objList, Workers w)
        {
            using (var Context = new Context())
            {

                foreach (Files item in objList)
                {

                    item.WorkerId = w.Id;

                    if (item.Id == 0)
                    {
                        Context.Files.Add(item);

                    }
                    else
                    {

                        Context.Entry(item).State = System.Data.Entity.EntityState.Modified;


                    }

                }

                try
                {

                    var result = Context.Files.Where(p => p.WorkerId == w.Id).ToList();
                    IEnumerable<Files> differenceQuery = result.Except(objList);

                    foreach (Files item in differenceQuery)
                    {
                        Context.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    }



                }
                catch (Exception ex)
                {


                }

                Context.SaveChanges();


            }
        }


        public static Workers UpdateWorker(Workers Worker)
        {
            //  System.Threading.Thread.Sleep(5000);

            using (var Context = new Context())
            {


                Worker.Status = "נתונים נשמרו";
                Context.Entry(Worker).State = System.Data.Entity.EntityState.Modified;

                Context.SaveChanges();


                if (!string.IsNullOrEmpty(Worker.ImgData))
                {

                    string root = HttpContext.Current.Server.MapPath("~/Uploads/");

                    string WorkerPath = root + Worker.Id.ToString();

                    if (!Directory.Exists(WorkerPath))
                    {
                        Directory.CreateDirectory(WorkerPath);

                    }
                    string filePath = WorkerPath + "\\Signature.png";
                    //if (File.Exists(filePath))
                    //{
                    //    File.Delete(filePath);
                    //}



                    File.WriteAllBytes(filePath, GetValidString(Worker.ImgData));
                }

                return Worker;
            }
        }

        public static byte[] GetValidString(string s)
        {
            s = s.Replace("data:image/png;base64,", "");
            s = s.Replace('-', '+').Replace('_', '/').PadRight(4 * ((s.Length + 3) / 4), '=');
            return Convert.FromBase64String(s);
        }


        //********************* End Workers ***************************
        //********************* Master Table ***************************

        public static List<Cities> GetCitiesList()
        {


            using (var Context = new Context())
            {

                return Context.Cities.ToList();

            }
        }

        public static List<Banks> GetBanksList()
        {


            using (var Context = new Context())
            {

                return Context.Banks.ToList();

            }
        }

        public static List<BanksBrunchs> GetBanksBrunchsList()
        {


            using (var Context = new Context())
            {

                return Context.BanksBrunchs.ToList();

            }
        }


        //********************* End Master Table ***************************


        public static User UpdateUser(User User)
        {
            using (var Context = new Context())
            {
                var CurrentUserFarmId = GetCurrentUser().Farm_Id;
                User.Farm_Id = CurrentUserFarmId != 0 ? CurrentUserFarmId : User.Farm_Id;
                if (User.Role == "sysAdmin")
                {
                    User.Farm_Id = 0;
                }
                var UserIdByEmail = GetUserIdByEmail(User.Email, CurrentUserFarmId);
                if (User.Id == 0 && UserIdByEmail == 0)
                {
                    Context.Users.Add(User);
                    Context.SaveChanges();
                }

                // צחי הוסיף בכדי למנוע עדכון של ת"ז קיים לחווה מסויימת
                if (User.Id != UserIdByEmail && UserIdByEmail != 0)
                {
                    User.FirstName = "Error";
                    return User;
                }



                //// צחי שינה
                //var Meta = JObject.Parse(User.Meta);
                //if (Meta["AvailableHours"] != null)
                //{
                //    foreach (var Item in Meta["AvailableHours"])
                //    {
                //        Item["resourceId"] = User.Id;
                //    }
                //}
                //User.Meta = Meta.ToString(Formatting.None);

                Context.Entry(User).State = System.Data.Entity.EntityState.Modified;

                try
                {
                    Context.SaveChanges();

                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // צחי הוסיף כדי לבדוק אם קיים כבר תלמיד כזה להחזיר שגיאה למשתמש
                    User.FirstName = "Error";
                }

                return User;
            }
        }

        public static void DeleteUser(int Id)
        {
            using (var Context = new Context())
            {
                var User = Context.Users.SingleOrDefault(u => u.Id == Id);
                if (User != null)
                {
                    User.Deleted = true;
                }
                //  Context.Notifications.RemoveRange(Context.Notifications.Where(n => n.EntityId == Id && n.EntityType == "student"));
                Context.SaveChanges();
            }
        }

        public static void DestroyUser(string email)
        {
            using (var Context = new Context())
            {
                var User = Context.Users.SingleOrDefault(u => u.Email == email);
                if (User != null)
                {

                    Context.Users.Remove(User);
                }
                //  Context.Notifications.RemoveRange(Context.Notifications.Where(n => n.EntityId == User.Id && n.EntityType == "student"));
                Context.SaveChanges();
            }
        }

        public static int GetUserIdByEmail(string Email, int CurrentUserFarmId = 0)
        {
            using (var Context = new Context())
            {

                var User = Context.Users.SingleOrDefault(u => u.Email.ToLower() == Email.ToLower() && (CurrentUserFarmId == 0 || u.Farm_Id == CurrentUserFarmId));
                if (User != null)
                    return User.Id;
                return 0;
            }
        }

        public static User GetCurrentUser()
        {
            var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
            var Email = identity.Claims.SingleOrDefault(c => c.Type == "sub").Value;

            return GetUser(GetUserIdByEmail(Email));
        }

        public static void RegisterDevice(string token)
        {
            using (var Context = new Context())
            {
                try
                {
                    var CurrentUserId = GetCurrentUser().Id;
                    //if (Context.UserDevices.SingleOrDefault(ud => ud.DeviceToken == token && ud.User_Id == CurrentUserId) == null)
                    //{
                    //    Context.UserDevices.Add(new UserDevices() { User_Id = CurrentUserId, DeviceToken = token });
                    //    Context.SaveChanges();
                    //}
                }
                catch (Exception ex) { }
            }
        }

        public static void UnregisterDevice(string token)
        {
            using (var Context = new Context())
            {
                try
                {
                    var CurrentUserId = GetCurrentUser().Id;
                    //var UserDevice = Context.UserDevices.Where(ud => ud.DeviceToken == token);
                    //Context.UserDevices.RemoveRange(UserDevice);
                    Context.SaveChanges();
                }
                catch (Exception ex) { }
            }
        }

        public static List<string> GetDevices(string UserId)
        {
            using (var Context = new Context())
            {
                try
                {
                    return null;
                    //  return Context.UserDevices.Where(ud => ud.User_Id == int.Parse(UserId)).Select(ud => ud.DeviceToken).ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }









        #region Helpers

        public static List<User> RemovePassword(List<User> Users)
        {
            foreach (var User in Users)
                User.Password = null;

            return Users;
        }

        public static User RemovePassword(User User)
        {
            User.Password = null;
            return User;
        }


        #endregion
    }
}