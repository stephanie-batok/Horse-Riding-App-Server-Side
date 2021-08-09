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
    public class TrialLessonController : ApiController
    {
        // GET api/<controller>
        public List<TrialLessonDTO> Get()
        {
            horseClubDbContext db = new horseClubDbContext();

            List<TrialLessonDTO> trial_lessons = db.trial_lesson.Select(l => new TrialLessonDTO()
            {
                lesson_id = l.lesson_id,
                date = l.date.ToString(),
                start_time = l.start_time.ToString().Substring(0, 5),
                end_time = l.end_time.ToString().Substring(0, 5),
                visitor_id = l.visitor.id,
                visitor_fullName = l.visitor.first_name + " " + l.visitor.last_name,
                horse_id = l.horse_id,
                horse_name = l.horse.name,
                instructor_id = l.instructor_id,
                instructor_fullName = l.worker.user.first_name + " " + l.worker.user.last_name,
                field = l.field,
                price = l.price,
                funding_source = l.funding_source,
                charge_type = l.charge_type,
                was_present = l.was_present,
                comments = l.comments,
                lesson_type = l.lesson_type
            }).ToList();

            return trial_lessons;
        }

        // GET api/<controller>/5
        [Route("api/TrialLesson/{lesson_id}")]
        public TrialLessonDTO Get(int lesson_id)
        {
            horseClubDbContext db = new horseClubDbContext();

            TrialLessonDTO trial_lesson = db.trial_lesson.Where(x => x.lesson_id == lesson_id).Select(l => new TrialLessonDTO()
            {
                lesson_id = l.lesson_id,
                date = l.date.ToString(),
                start_time = l.start_time.ToString().Substring(0, 5),
                end_time = l.end_time.ToString().Substring(0, 5),
                visitor_id = l.visitor.id,
                visitor_fullName = l.visitor.first_name + " " + l.visitor.last_name,
                horse_id = l.horse_id,
                horse_name = l.horse.name,
                instructor_id = l.instructor_id,
                instructor_fullName = l.worker.user.first_name + " " + l.worker.user.last_name,
                field = l.field,
                price = l.price,
                funding_source = l.funding_source,
                charge_type = l.charge_type,
                was_present = l.was_present,
                comments = l.comments,
                lesson_type = l.lesson_type
            }).SingleOrDefault();

            return trial_lesson;
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]TrialLessonDTO lesson)
        {
            horseClubDbContext db = new horseClubDbContext();

            visitor v = db.visitors.SingleOrDefault(x => x.id == lesson.visitor_id);
            if (v == null)
            {
                v = new visitor
                {
                    id = lesson.visitor.id,
                    first_name = lesson.visitor.first_name,
                    last_name = lesson.visitor.last_name,
                    gender = lesson.visitor.gender,
                    date_of_birth = DateTime.Parse(lesson.visitor.date_of_birth),
                    phone_number = lesson.visitor.phone_number,
                    address = lesson.visitor.address,
                    city = lesson.visitor.city,
                    height = lesson.visitor.height,
                    weight = lesson.visitor.weight,
                    comments = lesson.visitor.comments
                };
                db.visitors.Add(v);
            }

            DateTime date = DateTime.Parse(lesson.date);
            TimeSpan time = TimeSpan.Parse(lesson.start_time);

            trial_lesson l = db.trial_lesson.FirstOrDefault(x => x.visitor_id == lesson.visitor_id && x.date == date && x.start_time == time);
            if (l != null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "לתלמיד זה כבר קיים שיעור באותו המועד");
            }
            else
            {
                List<lesson> lessons = db.lessons.Where(x => x.date == date && x.start_time == time).ToList();
                List<trial_lesson> trial_lessons = db.trial_lesson.Where(x => x.date == date && x.start_time == time).ToList();

                foreach (lesson less in lessons)
                {
                    if (less.instructor_id == lesson.instructor_id)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "למדריך שנבחר כבר קיים שיעור באותו המועד");
                    }

                    if (less.horse_id == lesson.horse_id && less.horse_id!=null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "הסוס שנבחר משובץ לשיעור אחר באותו המועד");
                    }
                }

                foreach (trial_lesson less in trial_lessons)
                {
                    if (less.instructor_id == lesson.instructor_id)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "למדריך שנבחר כבר קיים שיעור באותו המועד");
                    }

                    if (less.horse_id == lesson.horse_id && less.horse_id != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "הסוס שנבחר משובץ לשיעור אחר באותו המועד");
                    }
                }

                worker w = db.workers.SingleOrDefault(x => x.id == lesson.instructor_id);
                horse h = db.horses.SingleOrDefault(x => x.id == lesson.horse_id);

                trial_lesson new_lesson = new trial_lesson
                {
                    visitor_id = lesson.visitor_id,
                    visitor = v,
                    date = DateTime.Parse(lesson.date),
                    start_time = TimeSpan.Parse(lesson.start_time),
                    end_time = TimeSpan.Parse(lesson.end_time),
                    field = lesson.field,
                    instructor_id = lesson.instructor_id,
                    worker = w,
                    funding_source = lesson.funding_source,
                    price = lesson.price,
                    charge_type = lesson.charge_type,
                    comments = lesson.comments,
                    lesson_type= "שיעור ניסיון"
                };

                if (h != null)
                {
                    new_lesson.horse_id = lesson.horse_id;
                    new_lesson.horse = h;
                }
                db.trial_lesson.Add(new_lesson);
            }
            try
            {
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "השיעור התווסף בהצלחה");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }

        // PUT api/<controller>/5
        [HttpPut]
        [Route("api/TrialLesson/{id}")]
        public HttpResponseMessage Put(int id, [FromBody]TrialLessonDTO lesson)
        {
            horseClubDbContext db = new horseClubDbContext();

            visitor v = db.visitors.SingleOrDefault(x => x.id == lesson.visitor_id);
            if (v != null)
            {
                v.id = lesson.visitor.id;
                v.first_name = lesson.visitor.first_name;
                v.last_name = lesson.visitor.last_name;
                v.gender = lesson.visitor.gender;
                v.date_of_birth = DateTime.Parse(lesson.visitor.date_of_birth);
                v.phone_number = lesson.visitor.phone_number;
                v.address = lesson.visitor.address;
                v.city = lesson.visitor.city;
                v.height = lesson.visitor.height;
                v.weight = lesson.visitor.weight;
                v.comments = lesson.visitor.comments;            
            }

            trial_lesson l = db.trial_lesson.SingleOrDefault(x => x.lesson_id == id);
            DateTime date = DateTime.Parse(lesson.date);
            TimeSpan time = TimeSpan.Parse(lesson.start_time);

            if (time > TimeSpan.Parse(lesson.end_time))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין שעת התחלה ושעת סיום תקינה");
            }

            if (l != null)
            {
                List<lesson> lessons = db.lessons.Where(x => x.date == date && x.start_time == time).ToList();
                List<trial_lesson> trial_lessons = db.trial_lesson.Where(x => x.date == date && x.start_time == time).ToList();

                foreach (lesson less in lessons)
                {
                    if (less.instructor_id == lesson.instructor_id)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "למדריך שנבחר כבר קיים שיעור באותו המועד");
                    }

                    if (less.horse_id == lesson.horse_id)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "הסוס שנבחר משובץ לשיעור אחר באותו המועד");
                    }
                }

                foreach (trial_lesson less in trial_lessons)
                {
                    if (less.lesson_id != l.lesson_id)
                    {
                        if (less.instructor_id == lesson.instructor_id)
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "למדריך שנבחר כבר קיים שיעור באותו המועד");
                        }

                        if (less.horse_id == lesson.horse_id)
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "הסוס שנבחר משובץ לשיעור אחר באותו המועד");
                        }
                    }
                }

                worker w = db.workers.SingleOrDefault(x => x.id == lesson.instructor_id);
                horse h = db.horses.SingleOrDefault(x => x.id == lesson.horse_id);

                l.field = lesson.field;
                if (h != null)
                {
                    l.horse_id = lesson.horse_id;
                    l.horse = h;
                }
                if (w != null)
                {
                    l.instructor_id = lesson.instructor_id;
                    l.worker = w;
                }
                l.funding_source = lesson.funding_source;
                l.price = lesson.price;
                l.charge_type = lesson.charge_type;
                l.comments = lesson.comments;
                l.was_present = lesson.was_present;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "פרטי השיעור לא נמצאו במערכת");
            }
            try
            {
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "השיעור עודכן בהצלחה");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }

        // DELETE api/<controller>/5
        [HttpDelete]
        [Route("api/TrialLesson/{lesson_id}")]
        public HttpResponseMessage Delete(int lesson_id)
        {
            horseClubDbContext db = new horseClubDbContext();
            trial_lesson l = db.trial_lesson.SingleOrDefault(x => x.lesson_id == lesson_id);

            if (l != null)
            {
                db.trial_lesson.Remove(l);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, " השיעור לא נמצא במערכת");
            }
            try
            {
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "השיעור נמחק בהצלחה");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }
    }
}