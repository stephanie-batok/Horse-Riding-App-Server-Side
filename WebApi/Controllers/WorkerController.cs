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
    public class WorkerController : ApiController
    {
        public List<WorkerDTO> Get()
        {
            horseClubDbContext db = new horseClubDbContext();

            return db.workers.Select(w => new WorkerDTO()
            {
                id = w.id,
                first_name = w.user.first_name,
                last_name = w.user.last_name,
                gender = w.user.gender,
                email = w.user.email,
                date_of_birth = w.user.date_of_birth.ToString(),
                phone_number = w.user.phone_number,
                city = w.user.city,
                address = w.user.address,
                password = w.user.password,
                starting_date = w.starting_date.ToString(),
            }).ToList();
        }

        [HttpGet]
        [Route("api/Worker/Instructor")]
        public List<InstructorDTO> GetInstructors()
        {
            horseClubDbContext db = new horseClubDbContext();

            return db.users.Where(u => u.user_type == "instructor").Select(i => new InstructorDTO()
            {
                id = i.id,
                first_name = i.first_name,
                last_name = i.last_name,
                gender = i.gender,
                email = i.email,
                date_of_birth = i.date_of_birth.ToString(),
                phone_number = i.phone_number,
                city = i.city,
                address = i.address,
                password = i.password,
                isAllowed = i.isAllowed,
                starting_date = i.worker.starting_date.ToString()
            }).ToList();
        }

        // GET api/<controller>/5
        public WorkerDTO Get(string id)
        {
            horseClubDbContext db = new horseClubDbContext();

            return db.workers.Where(x => x.id == id).Select(w => new WorkerDTO()
            {
                id = w.id,
                first_name = w.user.first_name,
                last_name = w.user.last_name,
                gender = w.user.gender,
                email = w.user.email,
                date_of_birth = w.user.date_of_birth.ToString(),
                phone_number = w.user.phone_number,
                city = w.user.city,
                address = w.user.address,
                password = w.user.password,
                isAllowed = w.user.isAllowed,
                starting_date = w.starting_date.ToString()
            }).SingleOrDefault();
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]WorkerDTO worker)
        {
            horseClubDbContext db = new horseClubDbContext();

            user u = db.users.SingleOrDefault(x => x.id == worker.id);

            if (u != null )
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "מדריך בעל תעודת זהות זהה קיים במערכת");
            }
            else
            {
                if (string.IsNullOrEmpty(worker.gender) || (worker.gender != "זכר" && worker.gender != "נקבה"))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין את מגדר המדריך (זכר או נקבה)");
                }

                if (string.IsNullOrEmpty(worker.first_name) || string.IsNullOrEmpty(worker.last_name))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין שם פרטי ושם משפחה של המדריך");
                }

                if (string.IsNullOrEmpty(worker.date_of_birth))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין תאריך לידה של המדריך");
                }

                if (DateTime.Now < DateTime.Parse(worker.date_of_birth))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין תאריך לידה תקין");
                }

                user user = db.users.SingleOrDefault(x => x.email == worker.email);

                if (user != null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין כתובת דואר אלקטורוני ייחודית שלא קיימת במערכת");
                }

                if (string.IsNullOrEmpty(worker.email))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין כתובת דואר אלקטרוני");
                }

                if (string.IsNullOrEmpty(worker.password) || worker.password.Length < 6)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין סיסמה תקינה (באורך של 6 תווים ומעלה)");
                }

                u = new user
                {
                    id = worker.id,
                    first_name = worker.first_name,
                    last_name = worker.last_name,
                    gender = worker.gender,
                    date_of_birth = DateTime.Parse(worker.date_of_birth),
                    city = worker.city,
                    address = worker.address,
                    phone_number = worker.phone_number,
                    email = worker.email,
                    password = worker.password,
                    isAllowed = true,
                    profileImg = "profile.png",
                    user_type = "instructor",
                };

                db.users.Add(u);

                worker w = new worker
                {
                    id = worker.id,
                    starting_date = worker.starting_date==""?DateTime.Now:DateTime.Parse(worker.starting_date)
                };

                db.workers.Add(w);
            }
            try
            {
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "המדריך התווסף בהצלחה");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put(string id, [FromBody]WorkerDTO worker)
        {
            horseClubDbContext db = new horseClubDbContext();
            user u = db.users.SingleOrDefault(x => x.id == id);
            worker w = db.workers.SingleOrDefault(x => x.id == id);

            if (u != null && w!=null)
            {
                if (string.IsNullOrEmpty(worker.gender) || (worker.gender != "זכר" && worker.gender != "נקבה"))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין את מגדר המדריך (זכר או נקבה)");
                }

                if (string.IsNullOrEmpty(worker.first_name) || string.IsNullOrEmpty(worker.last_name))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין שם פרטי ושם משפחה של המדריך");
                }

                if (string.IsNullOrEmpty(worker.date_of_birth))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין תאריך לידה של המדריך");
                }

                if (DateTime.Now < DateTime.Parse(worker.date_of_birth))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין תאריך לידה תקין");
                }

                user user = db.users.SingleOrDefault(x => x.email == worker.email);

                if (user != null && user.email!=u.email)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין כתובת דואר אלקטורוני ייחודית שלא קיימת במערכת");
                }

                if (string.IsNullOrEmpty(worker.email))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין כתובת דואר אלקטרוני");
                }

                if (string.IsNullOrEmpty(worker.password) || worker.password.Length < 6)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין סיסמה תקינה (באורך של 6 תווים ומעלה)");
                }

                u.first_name = worker.first_name;
                u.last_name = worker.last_name;
                u.gender = worker.gender;
                u.date_of_birth = DateTime.Parse(worker.date_of_birth);
                u.phone_number = worker.phone_number;
                u.email = worker.email;
                u.password = worker.password;
                u.address = worker.address;
                u.city = worker.city;
                u.isAllowed = worker.isAllowed;
                w.starting_date = DateTime.Parse(worker.starting_date);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "פרטי המדריך לא נמצאו במערכת");
            }            

            try
            {
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "פרטי המדריך עודכנו בהצלחה");
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
                return Request.CreateResponse(HttpStatusCode.NotFound, "פרטי המדריך לא נמצאו במערכת");
            }
            try
            {
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "המדריך נמחק בהצלחה");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }
    }
}