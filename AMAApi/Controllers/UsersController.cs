using FarmsApi.DataModels;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Web.Http;

namespace FarmsApi.Services
{
    [RoutePrefix("users")]
    public class UsersController : ApiController
    {

        [Route("UpdateUsersLessons")]
        [HttpGet]
        public string UpdateUsersLessons()
        {


            



            //UploadFromAccess uac = new UploadFromAccess();
            //uac.UpdateUsersLessons();
            return "sdsdsdsd";


        }


      




      


       



        [Authorize]
        [Route("getUsers/{role?}/{includeDeleted?}")]
        [HttpGet]
        public IHttpActionResult GetUsers(string role = null, bool includeDeleted = false)
        {
            return Ok(UsersService.GetUsers(role, includeDeleted));
        }

        [Authorize]
        [Route("getUser/{id?}")]
        [HttpGet]
        public IHttpActionResult GetUser(int? id = null)
        {
            return Ok(UsersService.GetUser(id));
        }

        [Authorize]
        [Route("getsetUserEnter/{isForCartis}/{id?}")]
        [HttpGet]
        public IHttpActionResult GetSetUserEnter(int? id = null, bool isForCartis = false)
        {
            return Ok(UsersService.GetSetUserEnter(id, isForCartis));
        }



        [Authorize]
        [Route("newUser")]
        [HttpGet]
        public IHttpActionResult NewUser()
        {
            return Ok(new User());
        }

        [Authorize]
        [Route("getUserIdByEmail/{email}")]
        [HttpGet]
        public IHttpActionResult GetUserIdByEmail(string email)
        {
            return Ok(UsersService.GetUserIdByEmail(email));
        }

        [Authorize(Roles = "farmAdmin,farmAdminHorse,sysAdmin,vetrinar,shoeing")]
        [Route("deleteUser/{id}")]
        [HttpGet]
        public IHttpActionResult DeleteUser(int id)
        {
            UsersService.DeleteUser(id);
            return Ok();
        }

        [Authorize(Roles = "farmAdmin,farmAdminHorse,sysAdmin,vetrinar,shoeing")]
        [Route("destroyUser")]
        [HttpGet]
        public IHttpActionResult DestroyUser([FromUri] string email)
        {
            UsersService.DestroyUser(email);
            return Ok();
        }

        [Authorize]
        [Route("updateUser")]
        [HttpPost]
        public IHttpActionResult UpdateUser(DataModels.User user)
        {
            return Ok(UsersService.UpdateUser(user));
        }


        [Authorize]
        [Route("getPortfolios/{llx}/{lly}/{urx}/{ury}/{text}/{font}/{space}/{id}/{pagenumber}")]
        [HttpGet]
        public IHttpActionResult GetPortfolios(int llx, int lly, int urx, int ury, string text, int font, int space, int id, int pagenumber)
        {
            return Ok(UsersService.GetPortfolios( llx,  lly,  urx,  ury,  text, font, space, id, pagenumber));
        }
       
        [Authorize]
        [Route("bindData/{id}/{comment}/{pagenumber}/{value}")]
        [HttpGet]
        public IHttpActionResult BindData(int id,  string comment, int pagenumber, string value)
        {
            return Ok(UsersService.BindData(id, comment, pagenumber, value));
        }


        //******************************************** Workers *****************************
        [Authorize]
        [Route("getFiles/{workerid}")]
        [HttpGet]
        public IHttpActionResult GetFiles(int Workerid)
        {
            return Ok(UsersService.GetFiles(Workerid));
        }

        [Authorize]
        [Route("getWorkers")]
        [HttpGet]
        public IHttpActionResult GetWorkers()
        {
            return Ok(UsersService.GetWorkers());
        }

        [Authorize]
        [Route("getWorker/{id}")]
        [HttpGet]
        public IHttpActionResult GetWorker(int id)
        {
            return Ok(UsersService.GetWorker(id));
        }

        [Authorize]
        [Route("deleteWorker/{id}")]
        [HttpGet]
        public IHttpActionResult DeleteWorker(int id)
        {
           
            return Ok(UsersService.DeleteWorker(id));
        }


        [Authorize]
        [Route("getWorkerChilds/{id}")]
        [HttpGet]
        public IHttpActionResult GetWorkerChilds(int id)
        {

            return Ok(UsersService.GetWorkerChilds(id));
        }


        [Authorize]
        [Route("updateWorker/{type}")]
        [HttpPost]
        public IHttpActionResult UpdateWorkerAndFiles(JArray dataobj, int type)
        {
            return Ok(UsersService.UpdateWorkerAndFiles(dataobj,type));
        }
        //******************************************** End Workers *****************************
        //******************************************** Master Table *****************************
        [Authorize]
        [Route("getMasterTable/{type}")]
        [HttpGet]
        public IHttpActionResult GetMasterTable(int type)
        {

            switch (type)
            {
                case 1:
                    return Ok(UsersService.GetCitiesList());

                case 2:
                    return Ok(UsersService.GetBanksList());
                case 3:
                    return Ok(UsersService.GetBanksBrunchsList());


                default:
                    return null;
            }

            
        }




        

        //******************************************** End Master Table *****************************
    }
}
