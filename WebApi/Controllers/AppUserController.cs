using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DATA.EF;
using WebApi.DTO;
using WebApi.Models;
using System.Web.Http.Cors;


namespace WebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AppUserController : ApiController
    {
        [HttpGet]
        [Route("api/AppUser/{email}/{password}")]
        public HttpResponseMessage Get(string email, string password)
        {
            try
            {
                horseClubDbContext db = new horseClubDbContext();
                UserDTO user = db.users.Where(x => x.email == email).Select(u => new UserDTO()
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
                }).FirstOrDefault();

                if (user != null)
                {
                    if (!user.isAllowed || user.user_type == "secretary" || user.user_type == "manager")
                    {
                        return Request.CreateResponse(HttpStatusCode.Unauthorized, "משתמש לא מורשה");
                    }

                    if (user.password == password)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, user);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.Unauthorized, "סיסמה שגויה");
                    }

                }
                return Request.CreateResponse(HttpStatusCode.NotFound, "המשתמש לא נמצא");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }

        [HttpGet]
        [Route("api/AppUser/Token/{id}")]
        public HttpResponseMessage GetToken(string id)
        {
            horseClubDbContext db = new horseClubDbContext();
            user u = db.users.SingleOrDefault(x => x.id == id);
            return Request.CreateResponse(HttpStatusCode.OK, u.token);
        }

        [HttpGet]
        [Route("api/AppUser/Lessons/{id}")]
        public HttpResponseMessage GetLesssons(string id)
        {
            try
            {
                horseClubDbContext db = new horseClubDbContext();
                user u = db.users.SingleOrDefault(x => x.id == id);

                if (u != null)
                {
                    if (u.user_type=="rider")
                    {
                        List<RiderLessonDTO> riderLessons = db.lessons.Where(x => x.rider_id == id).Select(l => new RiderLessonDTO() {

                            lesson_id = l.lesson_id,
                            date = l.date.ToString(),
                            start_time = l.start_time.ToString().Substring(0, 5),
                            end_time = l.end_time.ToString().Substring(0, 5),
                            rider_id = l.rider_id,
                            horse_name = l.horse.name,
                            instructor_fullName = l.worker.user.first_name + " " + l.worker.user.last_name,
                            field = l.field,
                            lesson_type = l.lesson_type

                        }).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, riderLessons);
                    }
                    else if (u.user_type=="instructor")
                    {
                        List<InstructorLessonDTO> instructorLessons = db.lessons.Where(x => x.instructor_id == id).Select(l => new InstructorLessonDTO()
                        {
                            lesson_id = l.lesson_id,
                            date = l.date.ToString(),
                            start_time = l.start_time.ToString().Substring(0, 5),
                            end_time = l.end_time.ToString().Substring(0, 5),
                            rider_id = l.rider_id,
                            rider_fullName = l.rider.user.first_name + " " + l.rider.user.last_name,
                            horse_id = l.horse_id,
                            horse_name = l.horse.name,
                            field = l.field,
                            lesson_type = l.lesson_type,
                            was_present = l.was_present,
                            comments = l.comments,
                            match_rank = l.match_rank

                        }).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, instructorLessons);
                    }
                }

                return Request.CreateResponse(HttpStatusCode.NotFound, "המשתמש לא נמצא");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }

        [HttpGet]
        [Route("api/AppUser/Lesson/{id}/{lesson_id}")]
        public HttpResponseMessage GetLessson(string id, int lesson_id)
        {
            try
            {
                horseClubDbContext db = new horseClubDbContext();
                user u = db.users.SingleOrDefault(x => x.id == id);

                if (u != null)
                {
                    if (u.user_type == "rider")
                    {
                        RiderLessonDTO riderLesson = db.lessons.Where(x => x.rider_id == id && x.lesson_id==lesson_id).Select(l => new RiderLessonDTO()
                        {
                            lesson_id = l.lesson_id,
                            date = l.date.ToString(),
                            start_time = l.start_time.ToString().Substring(0, 5),
                            end_time = l.end_time.ToString().Substring(0, 5),
                            rider_id = l.rider_id,
                            horse_name = l.horse.name,
                            instructor_fullName = l.worker.user.first_name + " " + l.worker.user.last_name,
                            field = l.field,
                            lesson_type = l.lesson_type

                        }).SingleOrDefault();

                        if (riderLesson == null)
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "השיעור לא נמצא");
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, riderLesson);
                    }
                    else if (u.user_type == "instructor")
                    {
                        InstructorLessonDTO instructorLesson = db.lessons.Where(x => x.instructor_id == id && x.lesson_id==lesson_id).Select(l => new InstructorLessonDTO()
                        {
                            lesson_id = l.lesson_id,
                            date = l.date.ToString(),
                            start_time = l.start_time.ToString().Substring(0, 5),
                            end_time = l.end_time.ToString().Substring(0, 5),
                            rider_id = l.rider_id,
                            rider_fullName = l.rider.user.first_name + " " + l.rider.user.last_name,
                            horse_id = l.horse_id,
                            horse_name = l.horse.name,
                            field = l.field,
                            lesson_type = l.lesson_type,
                            was_present = l.was_present,
                            comments = l.comments,
                            match_rank = l.match_rank

                        }).SingleOrDefault();

                        if (instructorLesson==null)
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "השיעור לא נמצא");
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, instructorLesson);
                    }
                }

                return Request.CreateResponse(HttpStatusCode.NotFound, "המשתמש לא נמצא");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }

        [HttpGet]
        [Route("api/AppUser/Chats/{id}")]
        public HttpResponseMessage GetChats(string id)
        {
            try
            {
                horseClubDbContext db = new horseClubDbContext();
                user u = db.users.SingleOrDefault(x => x.id == id);

                if (u != null)
                {
                    List<ChatDTO> chats = db.chats.Where(x => (x.user_id1 == u.id || x.user_id2 == u.id) && x.last_message!="").OrderByDescending(x=>x.dateTime).Select(c => new ChatDTO()
                    {
                        chat_num=c.chat_num,
                        last_message =c.last_message,
                        user_id1=c.user_id1,
                        user_name1 = c.user.first_name+" "+c.user.last_name,
                        user_profile1 = c.user.profileImg,
                        user_id2=c.user_id2,
                        user_name2 = c.user1.first_name + " " + c.user1.last_name,
                        user_profile2 = c.user1.profileImg,
                        dateTime = c.dateTime,
                        dateStr = c.dateStr,
                        timeStr = c.timeStr
                    }).ToList();

                    return Request.CreateResponse(HttpStatusCode.OK, chats);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "המשתמש לא נמצא");
                }
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }

        [HttpPost]
        [Route("api/AppUser/Chats")]
        public HttpResponseMessage PostChat([FromBody]ChatDTO chat)
        {
            horseClubDbContext db = new horseClubDbContext();

            chat c = db.chats.SingleOrDefault(x => (x.user_id1 == chat.user_id1 || x.user_id1 == chat.user_id2) && (x.user_id2 == chat.user_id1 || x.user_id2 == chat.user_id2));

            if (c != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, c.chat_num);
            }
            else
            {
                c = new chat
                {
                    last_message = chat.last_message,
                    dateTime = DateTime.Now,
                    user_id1 = chat.user_id1,
                    user_id2 = chat.user_id2
                };
                db.chats.Add(c);
            }
            try
            {
                db.SaveChanges();
                chat newChat = db.chats.OrderByDescending(x => x.dateTime).First();
                return Request.CreateResponse(HttpStatusCode.OK, newChat.chat_num);
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }

        [HttpPut]
        [Route("api/AppUser/Chats")]
        public HttpResponseMessage PutChat([FromBody]ChatDTO chat)
        {
            horseClubDbContext db = new horseClubDbContext();
            chat c = db.chats.SingleOrDefault(x => x.chat_num == chat.chat_num);
            if (c != null)
            {
                c.dateTime = chat.dateTime;
                c.dateStr = chat.dateStr;
                c.timeStr = chat.timeStr;
                c.last_message = chat.last_message;
                c.last_message_sent_by = chat.last_message_sent_by;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "ההודעה לא נמצאה");
            }
            try
            {
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }

        [HttpPost]
        [Route("api/AppUser/PushNotification")]
        public string PostPushNot([FromBody]PushNotDataDTO pnd)
        {
            var objectToSend = new
            {
                to = pnd.to,
                title = pnd.title,
                body = pnd.body,
                badge = pnd.badge,
                data = new { chat_num = pnd.data.chat_num , from_id = pnd.data.from_id, action=pnd.data.action}
            };

            string res = notificationServices.PushNotificationSend(objectToSend);
            return res;
        }

        [HttpGet]
        [Route("api/AppUser/Notifications/{id}")]
        public HttpResponseMessage GetNotifications(string id)
        {
            try
            {
                horseClubDbContext db = new horseClubDbContext();
                user u = db.users.SingleOrDefault(x => x.id == id);

                if (u != null)
                {
                    List<NotificationDTO> notifications = db.notifications.Where(x => x.user_id == id && x.was_sent == true).OrderByDescending(x => x.dateTime).Select(n => new NotificationDTO()
                    {
                        notification_id = n.notification_id,
                        user_id = n.user_id,
                        title = n.title,
                        text = n.text,
                        dateTime = n.dateTime,
                        dateStr = n.dateStr,
                        timeStr = n.timeStr,
                        lesson_id = n.lesson_id
                    }).ToList();

                    return Request.CreateResponse(HttpStatusCode.OK, notifications);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "המשתמש לא נמצא");
                }
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }

        [HttpPut]
        [Route("api/AppUser/Token/{id}")]
        public HttpResponseMessage PutToken(string id, [FromBody]string token)
        {
            horseClubDbContext db = new horseClubDbContext();
            user u = db.users.SingleOrDefault(x => x.id == id);
            if (u != null)
            {
                u.token = token;
            }

            try
            {
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK,"עודכן בהצלחה");
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, e.Message);
            }
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}