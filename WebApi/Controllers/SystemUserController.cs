using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DATA.EF;
using WebApi.DTO;
using System.Web.Http.Cors;


namespace WebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SystemUserController : ApiController
    {
        // GET api/<controller>
        [Route("api/SystemUser/{email}/{password}")]
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
                    if (!user.isAllowed|| user.user_type=="rider"|| user.user_type=="instructor")
                    {
                        return Request.CreateResponse(HttpStatusCode.Unauthorized, "משתמש לא מורשה");
                    }

                    if (user.password == null || user.password == "")
                    {
                        return Request.CreateResponse(HttpStatusCode.Unauthorized, "יש להזין סיסמה");
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

        public List<UserDTO> Get()
        {
            horseClubDbContext db = new horseClubDbContext();
            List<UserDTO> users = db.users.Where(x=>x.isAllowed).Select(u=> new UserDTO()
            {
                first_name = u.first_name,
                last_name = u.last_name,
                id = u.id,
                profileImg = u.profileImg,
                email = u.email,
                password = u.password,
                isAllowed = u.isAllowed,
                phone_number = u.phone_number,
                user_type = u.user_type
            }).ToList();

            return users;
        }

        [HttpGet]
        [Route("api/SystemUser/All")]
        public List<UserDTO> GetAllUsers()
        {
            horseClubDbContext db = new horseClubDbContext();
            List<UserDTO> users = db.users.Select(u => new UserDTO()
            {
                first_name = u.first_name,
                last_name = u.last_name,
                id = u.id,
                profileImg = u.profileImg,
                email = u.email,
                password = u.password,
                isAllowed = u.isAllowed,
                phone_number = u.phone_number,
                user_type = u.user_type
            }).ToList();

            return users;
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]UserDTO new_user)
        {
            horseClubDbContext db = new horseClubDbContext();

            user u = db.users.SingleOrDefault(x => x.id == new_user.id);

            if (u != null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "משתמש בעל תעודת זהות זהה קיים במערכת");
            }
            else
            {
                if (string.IsNullOrEmpty(new_user.gender) || (new_user.gender != "זכר" && new_user.gender != "נקבה"))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין את מגדר המשתמש (זכר או נקבה)");
                }

                if (string.IsNullOrEmpty(new_user.first_name) || string.IsNullOrEmpty(new_user.last_name))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין שם פרטי ושם משפחה של המשתמש");
                }

                if (string.IsNullOrEmpty(new_user.date_of_birth))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין תאריך לידה של המשתמש");
                }

                if (DateTime.Now < DateTime.Parse(new_user.date_of_birth))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין תאריך לידה תקין");
                }

                user user = db.users.SingleOrDefault(x => x.email == new_user.email);

                if (user != null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין כתובת דואר אלקטורוני ייחודית שלא קיימת במערכת");
                }

                if (string.IsNullOrEmpty(new_user.email))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין כתובת דואר אלקטרוני");
                }

                if (string.IsNullOrEmpty(new_user.password) || new_user.password.Length < 6)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין סיסמה תקינה (באורך של 6 תווים ומעלה)");
                }

                u = new user
                {
                    id = new_user.id,
                    first_name = new_user.first_name,
                    last_name = new_user.last_name,
                    gender = new_user.gender,
                    date_of_birth = DateTime.Parse(new_user.date_of_birth),
                    city = new_user.city,
                    address = new_user.address,
                    phone_number = new_user.phone_number,
                    email = new_user.email,
                    password = new_user.password,
                    isAllowed = true,
                    profileImg = "profile.png",
                    user_type = new_user.user_type,
                };

                db.users.Add(u);

                worker w = new worker
                {
                    id = new_user.id,
                    starting_date = new_user.starting_date == "" ? DateTime.Now : DateTime.Parse(new_user.starting_date)
                };

                db.workers.Add(w);
            }
            try
            {
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "המשתמש התווסף בהצלחה");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put(string id)
        {
            horseClubDbContext db = new horseClubDbContext();

            user u = db.users.SingleOrDefault(x => x.id == id);

            if (u != null)
            {
                u.isAllowed = true;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "פרטי המשתמש לא נמצאו במערכת");
            }
            try
            {
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "המתשמש שוחזר בהצלחה");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }

        // DELETE api/<controller>/5
        public HttpResponseMessage Delete(string id)
        {
            horseClubDbContext db = new horseClubDbContext();

            user u = db.users.SingleOrDefault(x => x.id == id);

            if (u != null)
            {
                u.isAllowed = false;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "פרטי המשתמש לא נמצאו במערכת");
            }
            try
            {
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "המתשמש נמחק בהצלחה");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }
    }
}