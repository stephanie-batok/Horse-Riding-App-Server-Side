using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using DATA.EF;
using WebApi.DTO;
using System.Web.Http.Cors;



namespace WebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RiderController : ApiController
    {
        // GET api/<controller>
        public List<RiderDTO> Get()                      //שליפת פרטי כל הרוכבים מתוך בסיס הנתונים
        {
            horseClubDbContext db = new horseClubDbContext();

            var riders = db.riders.Select(r => new RiderDTO()
            {
                id = r.id,
                weight = r.weight,
                height = r.height,
                riding_rank = r.riding_rank,
                riding_type = r.riding_type,
                starting_date = r.starting_date.ToString(),
                isActive = r.isActive,
                first_name = r.user.first_name,
                last_name = r.user.last_name,
                gender = r.user.gender,
                email = r.user.email,
                date_of_birth = r.user.date_of_birth.ToString(),
                phone_number = r.user.phone_number,
                city = r.user.city,
                address = r.user.address,
                password = r.user.password,

                regular_lessons = r.regular_lesson.Select(l => new RegularLessonDTO()
                {
                    day = l.day,
                    start_time = l.start_time.ToString().Substring(0, 5),
                    end_time = l.end_time.ToString().Substring(0, 5),
                    lesson_type = l.lesson_type,
                    price = l.price,
                    funding_source = l.funding_source
                }).ToList(),

                parents = r.parents.Select(p => new ParentDTO()
                {
                    id = p.id,
                    first_name = p.first_name,
                    last_name = p.last_name,
                    gender = p.gender,
                    phone_number = p.phone_number,
                    email = p.email,
                }).ToList(),

                instructor_full_name = r.worker.user.first_name + " " + r.worker.user.last_name,
                instructor_id = r.instructor_id

            }).ToList();

            return riders;
        }

        // GET api/<controller>/5
        public RiderDTO Get(string id)                      //שליפת פרטי רוכב ספציפי מתוך בסיס הנתונים
        {
            horseClubDbContext db = new horseClubDbContext();

            return db.riders.Where(x => x.id == id).Select(r => new RiderDTO()
            {
                id = r.id,
                weight = r.weight,
                height = r.height,
                riding_rank = r.riding_rank,
                riding_type = r.riding_type,
                starting_date = r.starting_date.ToString(),
                isActive = r.isActive,
                first_name = r.user.first_name,
                last_name = r.user.last_name,
                gender = r.user.gender,
                email = r.user.email,
                date_of_birth = r.user.date_of_birth.ToString(),
                phone_number = r.user.phone_number,
                city = r.user.city,
                address = r.user.address,
                password = r.user.password,

                regular_lessons = r.regular_lesson.Select(l => new RegularLessonDTO()
                {
                    lesson_id=l.lesson_id,
                    day = l.day,
                    start_time = l.start_time.ToString().Substring(0, 5),
                    end_time = l.end_time.ToString().Substring(0, 5),
                    lesson_type = l.lesson_type,
                    price = l.price,
                    funding_source = l.funding_source

                }).ToList(),

                parents = r.parents.Select(p => new ParentDTO()
                {
                    id = p.id,
                    first_name = p.first_name,
                    last_name = p.last_name,
                    gender = p.gender,
                    phone_number = p.phone_number,
                    email = p.email,
                }).ToList(),

                instructor_full_name = r.worker.user.first_name + " " + r.worker.user.last_name,
                instructor_id = r.instructor_id

            }).SingleOrDefault();
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]RiderDTO rider)           //הכנסת רוכב חדש לתוך בסיס הנתונים
        {
            horseClubDbContext db = new horseClubDbContext();

            user u = db.users.SingleOrDefault(x => x.id == rider.id);
            rider r = db.riders.SingleOrDefault(x => x.id == rider.id);

            if (u != null||r!=null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "תלמיד בעל תעודת זהות זהה קיים במערכת");
            }
            else
            {
                if (string.IsNullOrEmpty(rider.id) || rider.id.Length != 9)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להקליד מספר תעודת זהות תקין בעל 9 ספרות)");
                }

                if (string.IsNullOrEmpty(rider.gender) || (rider.gender != "זכר" && rider.gender != "נקבה"))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "פרטים אישיים - יש להזין את מגדר הרוכב (זכר או נקבה)");
                }

                if (string.IsNullOrEmpty(rider.first_name) || string.IsNullOrEmpty(rider.last_name))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "פרטים אישיים - יש להזין שם פרטי ושם משפחה של הרוכב");
                }

                if (string.IsNullOrEmpty(rider.date_of_birth))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין תאריך לידה של הרוכב");
                }

                if (DateTime.Now < DateTime.Parse(rider.date_of_birth))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "פרטים אישיים - יש להזין תאריך לידה תקין");
                }

                user user = db.users.SingleOrDefault(x => x.email == rider.email);

                if (user!=null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "פרטים אישיים - יש להזין כתובת דואר אלקטורוני ייחודית שלא קיימת במערכת");
                }

                if (string.IsNullOrEmpty(rider.email) || rider.email == "")
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "פרטים אישיים - יש להזין כתובת דואר אלקטרוני");
                }

                if (string.IsNullOrEmpty(rider.password) || rider.password == "" || rider.password.Length<6)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "פרטים אישיים - יש להזין סיסמה תקינה (באורך של 6 תווים ומעלה)");
                }

                u = new user
                {
                    id = rider.id,
                    first_name = rider.first_name,
                    last_name = rider.last_name,
                    gender = rider.gender,
                    date_of_birth = DateTime.Parse(rider.date_of_birth),
                    phone_number = rider.phone_number,
                    email = rider.email,
                    profileImg = "profile.png",
                    password = rider.password,
                    isAllowed = true,
                    user_type = "rider"
                };

                db.users.Add(u);

                if (rider.riding_type == "")
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "פרטים אישיים - יש להזין את מגזר הרכיבה (רכיבה טיפולית או ספורטיבית)");
                }

                if (rider.weight!=null)
                {
                    if (rider.weight<0)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "פרטים אישיים - יש להזין משקל תקין)");
                    }
                }

                if (rider.height != null)
                {
                    if (rider.height < 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "פרטים אישיים - יש להזין גובה תקין)");
                    }
                }

                if (rider.instructor_id == "" || string.IsNullOrEmpty(rider.instructor_id))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "פרטי שיעור - יש לבחור מדריך קבוע לרוכב");
                }

                r = new rider
                {
                    riding_type = rider.riding_type,
                    weight = rider.weight,
                    height = rider.height,
                    starting_date = rider.starting_date ==""?DateTime.Now:DateTime.Parse(rider.starting_date),
                    instructor_id = rider.instructor_id,
                    isActive = true,
                    riding_rank = 0,
                    user = u
                };

                if (rider.horse_id != 0 )
                {
                    horse h = db.horses.SingleOrDefault(x => x.id == rider.horse_id);
                    if (h!=null)
                    {
                        r.horse_id = h.id;
                        r.horse = h;
                    }
                }

                foreach (ParentDTO parent in rider.parents)
                {
                    if (parent.id != "" && parent.id != null)
                    {
                        parent p = db.parents.SingleOrDefault(x => x.id == parent.id);

                        if (p == null)
                        {
                            if (parent.id.Length != 9)
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להקליד עבור ההורה מספר תעודת זהות תקין בעל 9 ספרות)");
                            }

                            p = new parent
                            {
                                id = parent.id,
                                first_name = parent.first_name,
                                last_name = parent.last_name,
                                gender = parent.gender,
                                phone_number = parent.phone_number,
                                email = parent.email,
                            };
                            db.parents.Add(p);
                        }
                        r.parents.Add(p);
                    }
                }
                db.riders.Add(r);

                foreach (RegularLessonDTO regularLesson in rider.regular_lessons)
                {

                    if (regularLesson.day!=null && regularLesson.day != "")
                    {
                        if (string.IsNullOrEmpty(regularLesson.start_time) || string.IsNullOrEmpty(regularLesson.end_time))
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין שעת התחלה ושעת סיום שיעור קבוע");
                        }

                        if (TimeSpan.Parse(regularLesson.start_time) > TimeSpan.Parse(regularLesson.end_time))
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין שעת התחלה ושעת סיום שיעור תקינה");
                        }

                        regular_lesson rl = new regular_lesson
                        {
                            day = regularLesson.day,
                            start_time = TimeSpan.Parse(regularLesson.start_time),
                            end_time = TimeSpan.Parse(regularLesson.end_time),
                            lesson_type = regularLesson.lesson_type,
                            price = regularLesson.price,
                            funding_source = regularLesson.funding_source,
                            rider_id = rider.id,
                        };

                        r.regular_lesson.Add(rl);
                        db.regular_lesson.Add(rl);
                    }
                }
            }
            try
            {
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "התלמיד התווסף בהצלחה");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }

        }

        // PUT api/<controller>/5
        [HttpPut]
        [Route("api/Rider/PersonalDetails/{id}")]
        public HttpResponseMessage PutPersonalDetails(string id, [FromBody]RiderDTO rider)             //עריכת פרטי רוכב ספציפי בבסיס הנתונים
        {
            horseClubDbContext db = new horseClubDbContext();

            user u = db.users.SingleOrDefault(x => x.id == id);

            user user = db.users.SingleOrDefault(x => x.email == rider.email);

            if (u != null)
            {
                if (string.IsNullOrEmpty(rider.first_name) || string.IsNullOrEmpty(rider.last_name))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "פרטים אישיים - יש להזין שם פרטי ושם משפחה של הרוכב");
                }

                if (string.IsNullOrEmpty(rider.gender) || (rider.gender != "זכר" && rider.gender != "נקבה"))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "פרטים אישיים - יש להזין את מגדר הרוכב (זכר או נקבה)");
                }

                if (string.IsNullOrEmpty(rider.date_of_birth))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין תאריך לידה של הרוכב");
                }

                if (DateTime.Now < DateTime.Parse(rider.date_of_birth))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "פרטים אישיים - יש להזין תאריך לידה תקין");
                }

                if (user != null && user.email != u.email)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין כתובת דואר אלקטורוני ייחודית שלא קיימת במערכת");
                }

                if (string.IsNullOrEmpty(rider.password) || rider.password.Length < 6)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "פרטים אישיים - יש להזין סיסמה תקינה (באורך של 6 תווים ומעלה)");
                }

                u.first_name = rider.first_name;
                u.last_name = rider.last_name;
                u.gender = rider.gender;
                u.date_of_birth = DateTime.Parse(rider.date_of_birth);
                u.phone_number = rider.phone_number;
                u.email = rider.email;
                u.password = rider.password;
                u.address = rider.address;
                u.city = rider.city;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "פרטי המשתמש לא נמצאו במערכת");
            }

            rider r = db.riders.SingleOrDefault(x => x.id == rider.id);

            if(r != null)
            {
                if (rider.starting_date != "")
                {
                    r.starting_date = DateTime.Parse(rider.starting_date);
                }

                if (rider.weight != null)
                {
                    if (rider.weight < 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "פרטים אישיים - יש להזין משקל תקין)");
                    }
                }

                if (rider.height != null)
                {
                    if (rider.height < 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "פרטים אישיים - יש להזין גובה תקין)");
                    }
                }

                r.riding_type = rider.riding_type;
                r.weight = rider.weight;
                r.height = rider.height;
                r.isActive = rider.isActive;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "פרטי התלמיד לא נמצאו במערכת");
            }
            try
            {
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "הנתונים עודכנו בהצלחה");

            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }

        [HttpPut]
        [Route("api/Rider/ParentDetails/{id}")]
        public HttpResponseMessage PutParentDetails(string id, [FromBody]RiderDTO rider)             //עריכת פרטי הורים של רוכב ספציפי בבסיס הנתונים
        {
            horseClubDbContext db = new horseClubDbContext();

            rider r = db.riders.SingleOrDefault(x => x.id == rider.id);


            if (r != null)
            {
                if (rider.parents.Count==0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "לא הוקלדו פרטי הורים");
                }
                foreach (ParentDTO parent in rider.parents)
                {
                    if (parent.id != "" && parent.id != null)
                    {
                        parent p = db.parents.SingleOrDefault(x => x.id == parent.id);

                        if (p != null)
                        {
                            p.first_name = parent.first_name;
                            p.last_name = parent.last_name;
                            p.gender = parent.gender;
                            p.phone_number = parent.phone_number;
                            p.email = parent.email;
                        }
                        else
                        {
                            if (parent.id.Length!=9)
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להקליד מספר תעודת זהות תקין בעל 9 ספרות)");
                            }

                            p = new parent
                            {
                                id = parent.id,
                                first_name = parent.first_name,
                                last_name = parent.last_name,
                                gender = parent.gender,
                                phone_number = parent.phone_number,
                                email = parent.email,
                            };

                            db.parents.Add(p);
                            r.parents.Add(p);
                        }
                    }
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "פרטי התלמיד לא נמצאו במערכת");
            }

            try
            {
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "הנתונים עודכנו בהצלחה");

            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }

        // PUT api/<controller>/5
        [HttpPut]
        [Route("api/Rider/LessonDetails/{id}")]
        public HttpResponseMessage PutLessonDetails(string id, [FromBody]RiderDTO rider)             //עריכת פרטי שיעור קבוע של רוכב ספציפי בבסיס הנתונים
        {
            horseClubDbContext db = new horseClubDbContext();

            rider r = db.riders.SingleOrDefault(x => x.id == rider.id);

            if (r != null)
            {
                r.instructor_id = rider.instructor_id;
                if (rider.horse_id != 0)
                {
                    r.horse_id = rider.horse_id;
                }

                foreach (RegularLessonDTO regularLesson in rider.regular_lessons)
                {

                    if (regularLesson.day != null && regularLesson.day != "")
                    {
                        if (string.IsNullOrEmpty(regularLesson.start_time) || string.IsNullOrEmpty(regularLesson.end_time))
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין שעת התחלה ושעת סיום שיעור קבוע");
                        }

                        if (TimeSpan.Parse(regularLesson.start_time) > TimeSpan.Parse(regularLesson.end_time))
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין שעת התחלה ושעת סיום שיעור תקינה");
                        }

                        regular_lesson rl = db.regular_lesson.SingleOrDefault(x => x.lesson_id == regularLesson.lesson_id);

                        if (rl != null)
                        {
                            rl.day = regularLesson.day;
                            rl.start_time = TimeSpan.Parse(regularLesson.start_time);
                            rl.end_time = TimeSpan.Parse(regularLesson.end_time);
                            rl.lesson_type = regularLesson.lesson_type;
                            rl.price = regularLesson.price;
                            rl.funding_source = regularLesson.funding_source;
                        }
                        else
                        {
                            rl = new regular_lesson
                            {
                                day = regularLesson.day,
                                start_time = TimeSpan.Parse(regularLesson.start_time),
                                end_time = TimeSpan.Parse(regularLesson.end_time),
                                lesson_type = regularLesson.lesson_type,
                                price = regularLesson.price,
                                funding_source = regularLesson.funding_source,
                            };

                            r.regular_lesson.Add(rl);
                            db.regular_lesson.Add(rl);
                        }
                    }
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "פרטי התלמיד לא נמצאו במערכת");
            }
            try
            {
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "הנתונים עודכנו בהצלחה");
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

            rider r = db.riders.SingleOrDefault(x => x.id == id);

            if (r != null)
            {
                r.isActive = false;            
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "פרטי התלמיד לא נמצאו במערכת");
            }
            try
            {
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "התלמיד נמחק בהצלחה");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented,"שגיאה");
            }
        }

        //public HttpResponseMessage Delete(string id)
        //{
        //    horseClubDbContext db = new horseClubDbContext();
        //    rider r = db.riders
        //                .Include(x => x.parents)
        //                .Include(x => x.lessons)
        //                .Include(x => x.regular_lesson)
        //                .SingleOrDefault(x => x.id == id);

        //    if (r != null)
        //    {
        //        List<parent> parents = r.parents.ToList();
        //        for (int i = 0; i < parents.Count; i++)
        //        {
        //            r.parents.Remove(parents[i]);
        //            db.parents.Remove(parents[i]);
        //        }

        //        List<regular_lesson> regular_lessons = r.regular_lesson.ToList();
        //        for (int i = 0; i < regular_lessons.Count; i++)
        //        {
        //            r.regular_lesson.Remove(regular_lessons[i]);
        //            db.regular_lesson.Remove(regular_lessons[i]);
        //        }

        //        List<lesson> lessons = r.lessons.ToList();
        //        for (int i = 0; i < lessons.Count; i++)
        //        {
        //            r.lessons.Remove(lessons[i]);
        //            db.lessons.Remove(lessons[i]);
        //        }

        //        db.riders.Remove(r);

        //        user u = db.users.SingleOrDefault(x => x.id == id);
        //        if (u != null)
        //        {
        //            db.users.Remove(u);
        //        }
        //    }
        //    else
        //    {
        //        return Request.CreateResponse(HttpStatusCode.NotFound, "פרטי התלמיד לא נמצאו במערכת");
        //    }
        //    try
        //    {
        //        db.SaveChanges();
        //        return Request.CreateResponse(HttpStatusCode.OK, "התלמיד נמחק בהצלחה");
        //    }
        //    catch (Exception e)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.NotImplemented, e);
        //    }
        //}
    }
}