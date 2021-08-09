using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using DATA.EF;
using WebApi.DTO;
using System.Web;
using System.Threading.Tasks;
using System.IO;

namespace WebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ProfileController : ApiController
    {
        [HttpGet]
        [Route("api/Profile/{id}")]
        public HttpResponseMessage Get(string id)
        {
            try
            {
                horseClubDbContext db = new horseClubDbContext();
                UserDTO user = db.users.Where(x => x.id == id).Select(u => new UserDTO()
                {
                    id = u.id,
                    first_name = u.first_name,
                    last_name = u.last_name,
                    email = u.email,
                    profileImg = u.profileImg,
                    password = u.password,
                    phone_number = u.phone_number,
                    isAllowed = u.isAllowed,
                    user_type = u.user_type
                }).SingleOrDefault();

                if (user != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, user);
                }

                return Request.CreateResponse(HttpStatusCode.NotFound, "המשתמש לא נמצא");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }

        [HttpGet]
        [Route("api/Profile/Message/{id}")]
        public HttpResponseMessage GetLastMessage(string id)
        {
            try
            {
                horseClubDbContext db = new horseClubDbContext();
                user u = db.users.SingleOrDefault(x => x.id == id);

                if (u != null)
                {
                    ChatDTO last_chat = db.chats.Where(x => (x.user_id1 == u.id || x.user_id2 == u.id) && x.last_message != "" && x.last_message_sent_by!=id).OrderByDescending(x => x.dateTime).Select(c => new ChatDTO()
                    {
                        chat_num = c.chat_num,
                        last_message = c.last_message,
                        user_id1 = c.user_id1,
                        user_name1 = c.user.first_name + " " + c.user.last_name,
                        user_profile1 = c.user.profileImg,
                        user_id2 = c.user_id2,
                        user_name2 = c.user1.first_name + " " + c.user1.last_name,
                        user_profile2 = c.user1.profileImg,
                        dateTime = c.dateTime,
                        dateStr = c.dateStr,
                        timeStr = c.timeStr
                    }).FirstOrDefault();

                    return Request.CreateResponse(HttpStatusCode.OK, last_chat);
                }
                //if the code is 404 - there is no chats...
                return Request.CreateResponse(HttpStatusCode.NotFound, "אין הודעות חדשות");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }

        [HttpGet]
        [Route("api/Profile/Notifications/{id}")]
        public HttpResponseMessage GetLastNotification(string id)
        {
            try
            {
                horseClubDbContext db = new horseClubDbContext();
                user u = db.users.SingleOrDefault(x => x.id == id);

                if (u != null)
                {
                    NotificationDTO last_notification = db.notifications.Where(x => x.user_id == id && x.was_sent == true).OrderByDescending(x => x.dateTime).Select(n => new NotificationDTO()
                    {
                        notification_id = n.notification_id,
                        user_id = n.user_id,
                        title = n.title,
                        text = n.text,
                        dateTime = n.dateTime,
                        dateStr = n.dateStr,
                        timeStr = n.timeStr,
                        lesson_id = n.lesson_id
                    }).FirstOrDefault();

                    //if the code is 404 - there is no notifications...
                    return Request.CreateResponse(HttpStatusCode.OK, last_notification);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "אין התראות");
                }
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }


        [HttpPut]
        [Route("api/Profile/{id}")]
        public HttpResponseMessage Put(string id, [FromBody] UserProfileDTO userProfileDTO)
        {
            horseClubDbContext db = new horseClubDbContext();

            user u = db.users.SingleOrDefault(x => x.id == id);

            if (u.email != userProfileDTO.email)
            {
                user chackUser = db.users.SingleOrDefault(x => x.email == userProfileDTO.email);
                if (chackUser != null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "קיים משתמש עם כתובת מייל זהה, לא ניתן להשתמש בכתובת זו.");
                }
                else
                {
                    u.email = userProfileDTO.email;
                }
            }

            if (!string.IsNullOrEmpty(userProfileDTO.password) && userProfileDTO.password !="")
            {
                if (userProfileDTO.password.Length < 6)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין סיסמה תקינה (באורך של 6 תווים ומעלה)");
                }
                u.password = userProfileDTO.password;
            }

            if (!string.IsNullOrEmpty(userProfileDTO.phone_number) && userProfileDTO.phone_number != "")
            {
                u.phone_number = userProfileDTO.phone_number;
            }

            try
            {
                db.SaveChanges();

                UserDTO newUser = db.users.Where(x => x.id == id).Select(x => new UserDTO()
                {
                    id = x.id,
                    first_name = x.first_name,
                    last_name = x.last_name,
                    email = x.email,
                    profileImg = x.profileImg,
                    password = x.password,
                    phone_number = x.phone_number,
                    isAllowed = x.isAllowed,
                    user_type = x.user_type

                }).SingleOrDefault();

                return Request.CreateResponse(HttpStatusCode.OK, newUser);
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }

        [HttpPut]
        [Route("api/Profile/Pic/{id}")]
        public HttpResponseMessage PutPic(string id,[FromBody]string profile_Img)
        {
            horseClubDbContext db = new horseClubDbContext();
            user u = db.users.SingleOrDefault(x => x.id == id);
            u.profileImg = profile_Img;
            try
            {
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "התמונה נשמרה בהצלחה");
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, e);
            }
        }

        [Route("api/Profile/PostPic")]
        public Task<HttpResponseMessage> Post()
        {
            string outputForNir = "start---";
            List<string> savedFilePath = new List<string>();

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            //Where to put the picture on server  ...MapPath("~/TargetDir")
            string rootPath = HttpContext.Current.Server.MapPath("~/uploadFiles");
            var provider = new MultipartFileStreamProvider(rootPath);
            var task = Request.Content.ReadAsMultipartAsync(provider).
                ContinueWith<HttpResponseMessage>(t =>
                {
                    if (t.IsCanceled || t.IsFaulted)
                    {
                        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                    }
                    foreach (MultipartFileData item in provider.FileData)
                    {
                        try
                        {
                            outputForNir += " ---here";
                            string name = item.Headers.ContentDisposition.FileName.Replace("\"", "");
                            outputForNir += " ---here2=" + name;

                            //need the guid because in react native in order to refresh an inamge it has to have a new name
                            string newFileName = Path.GetFileNameWithoutExtension(name) + "_" + CreateDateTimeWithValidChars() + Path.GetExtension(name);
                            //string newFileName = Path.GetFileNameWithoutExtension(name) + "_" + Guid.NewGuid() + Path.GetExtension(name);
                            //string newFileName = name + "" + Guid.NewGuid();
                            outputForNir += " ---here3" + newFileName;


                            //delete all files begining with the same name
                            string[] names = Directory.GetFiles(rootPath);
                            foreach (var fileName in names)
                            {
                                if (Path.GetFileNameWithoutExtension(fileName).IndexOf(Path.GetFileNameWithoutExtension(name)) != -1)
                                {
                                    File.Delete(fileName);
                                }
                            }

                            //File.Move(item.LocalFileName, Path.Combine(rootPath, newFileName));
                            File.Copy(item.LocalFileName, Path.Combine(rootPath, newFileName), true);
                            File.Delete(item.LocalFileName);
                            outputForNir += " ---here4";

                            Uri baseuri = new Uri(Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery, string.Empty));
                            outputForNir += " ---here5";
                            string fileRelativePath = "~/uploadFiles/" + newFileName;
                            outputForNir += " ---here6 imageName=" + fileRelativePath;
                            Uri fileFullPath = new Uri(baseuri, VirtualPathUtility.ToAbsolute(fileRelativePath));
                            outputForNir += " ---here7" + fileFullPath.ToString();
                            savedFilePath.Add(fileFullPath.ToString());
                        }
                        catch (Exception ex)
                        {
                            outputForNir += " ---excption=" + ex.Message;
                            string message = ex.Message;
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.Created, "nirchen " + savedFilePath[0] + "!" + provider.FileData.Count + "!" + outputForNir + ":)");
                });
            return task;
        }

        private string CreateDateTimeWithValidChars()
        {
            return DateTime.Now.ToString().Replace('/', '_').Replace(':', '-').Replace(' ', '_');
        }

    }
}