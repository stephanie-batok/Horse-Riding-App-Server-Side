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
    public class LessonController : ApiController
    {
        // GET api/<controller>
        public List<LessonDTO> Get()
        {
            horseClubDbContext db = new horseClubDbContext();

            List<LessonDTO> lessons = db.lessons.Select(l => new LessonDTO()
            {
                lesson_id=l.lesson_id,
                date = l.date.ToString(),
                start_time = l.start_time.ToString().Substring(0, 5),
                end_time = l.end_time.ToString().Substring(0, 5),
                rider_id = l.rider_id,
                rider_fullName = l.rider.user.first_name + " " + l.rider.user.last_name,
                horse_id = l.horse_id,
                horse_name = l.horse.name,
                instructor_id = l.instructor_id,
                instructor_fullName = l.worker.user.first_name + " " + l.worker.user.last_name,
                field = l.field,
                lesson_type = l.lesson_type,
                price = l.price,
                funding_source = l.funding_source,
                charge_type = l.charge_type,
                was_present = l.was_present,
                comments = l.comments,
                match_rank = l .match_rank
            }).ToList();

            return lessons;
        }

        // GET api/<controller>/5

        [Route("api/Lesson/{lesson_id}")]

        public HttpResponseMessage Get(int lesson_id)
        {
            horseClubDbContext db = new horseClubDbContext();

            LessonDTO lesson = db.lessons.Where(x=>x.lesson_id == lesson_id).Select(l => new LessonDTO() {
                lesson_id=l.lesson_id,
                date = l.date.ToString(),
                start_time = l.start_time.ToString().Substring(0, 5),
                end_time = l.end_time.ToString().Substring(0, 5),
                rider_id = l.rider_id,
                rider_fullName = l.rider.user.first_name + " " + l.rider.user.last_name,
                horse_id = l.horse_id,
                horse_name = l.horse.name,
                instructor_id = l.instructor_id,
                instructor_fullName = l.worker.user.first_name + " " + l.worker.user.last_name,
                field = l.field,
                lesson_type = l.lesson_type,
                price = l.price,
                funding_source = l.funding_source,
                charge_type = l.charge_type,
                was_present = l.was_present,
                comments = l.comments,
                match_rank =l.match_rank
            }).SingleOrDefault();

            return Request.CreateResponse(HttpStatusCode.OK, lesson);
        }

        [HttpGet]
        [Route("api/Lesson/RiderFeedback/{lesson_id}")]
        public HttpResponseMessage GetRiderFeedback(int lesson_id)
        {
            horseClubDbContext db = new horseClubDbContext();

            RiderFeedbackDTO RiderFeedback = db.rider_feedback.Where(x => x.lesson_id == lesson_id).Select(f => new RiderFeedbackDTO() { 
                feedback_id = f.feedback_id,
                lesson_id = f.lesson_id,
                q1 = f.q1,
                q2 = f.q2,
                q3 = f.q3,
                q4 = f.q4
            }).SingleOrDefault();

            if (RiderFeedback!=null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, RiderFeedback);
            }

            return Request.CreateResponse(HttpStatusCode.NotFound, "לא נמצא משוב עבור שיעור זה");
        }

        [HttpPost]
        [Route("api/Lesson/RiderFeedback")]
        public HttpResponseMessage PostRiderFeedback([FromBody]RiderFeedbackDTO riderFeedback)
        {
            horseClubDbContext db = new horseClubDbContext();

            rider_feedback feedback = db.rider_feedback.Where(x => x.lesson_id == riderFeedback.lesson_id).SingleOrDefault();

            if (feedback != null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "לשיעור זה קיים משוב במערכת");
            }
            else
            {
                rider_feedback rider_Feedback = new rider_feedback
                {
                    lesson_id = riderFeedback.lesson_id,
                    q1 = riderFeedback.q1,
                    q2 = riderFeedback.q2,
                    q3 = riderFeedback.q3,
                    q4 = riderFeedback.q4,
                };

                db.rider_feedback.Add(rider_Feedback);
            }
            try
            {
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "המשוב התווסף בהצלחה");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }


        [HttpGet]
        [Route("api/Lesson/InstructorFeedback/{lesson_id}")]
        public HttpResponseMessage GetInstructorFeedback(int lesson_id)
        {
            horseClubDbContext db = new horseClubDbContext();

            InstructorFeedbackDTO InstructorFeedback = db.instructor_feedback.Where(x => x.lesson_id == lesson_id).Select(f => new InstructorFeedbackDTO()
            {
                feedback_id = f.feedback_id,
                lesson_id = f.lesson_id,
                q1 = f.q1,
                q2 = f.q2,
                q3 = f.q3,
                q4 = f.q4
            }).SingleOrDefault();

            if (InstructorFeedback!=null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, InstructorFeedback);

            }

            return Request.CreateResponse(HttpStatusCode.NotFound, "לא קיים משוב לשיעור זה");

        }

        [HttpPost]
        [Route("api/Lesson/InstructorFeedback")]
        public HttpResponseMessage PostInstructorFeedback([FromBody]InstructorFeedbackDTO instructorFeedback)
        {
            horseClubDbContext db = new horseClubDbContext();

            instructor_feedback feedback = db.instructor_feedback.Where(x => x.lesson_id == instructorFeedback.lesson_id).SingleOrDefault();

            if (feedback != null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "לשיעור זה קיים משוב במערכת");
            }
            else
            {
                lesson l = db.lessons.SingleOrDefault(x => x.lesson_id == instructorFeedback.lesson_id);

                instructor_feedback instructor_Feedback = new instructor_feedback
                {
                    lesson_id = instructorFeedback.lesson_id,
                    lesson=l,
                    q1 = instructorFeedback.q1,
                    q2 = instructorFeedback.q2,
                    q3 = instructorFeedback.q3,
                    q4 = instructorFeedback.q4,
                    score = (double)((instructorFeedback.q1 + instructorFeedback.q2 + instructorFeedback.q3 + instructorFeedback.q4)/4)
                };

                db.instructor_feedback.Add(instructor_Feedback);
            }
            try
            {                                                            //get past feedbacks and calculate the new riding rank of the rider
                db.SaveChanges();

                instructor_feedback last_feedback = db.instructor_feedback.OrderByDescending(x => x.feedback_id).FirstOrDefault();
                List<instructor_feedback> past_lessons_feedback = db.instructor_feedback.Where(x => x.lesson.rider_id == last_feedback.lesson.rider_id).ToList();
                double avg_match_with_horse = past_lessons_feedback.Average(x => x.score).Value;
                rider r = db.riders.SingleOrDefault(x => x.id == last_feedback.lesson.rider_id);
                if (r!=null)
                {
                    r.riding_rank = (int)Math.Round(avg_match_with_horse);
                }

                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "המשוב התווסף בהצלחה");
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, e);
            }
        }


        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]LessonDTO lesson)
        {
            horseClubDbContext db = new horseClubDbContext();

            DateTime date = DateTime.Parse(lesson.date);
            TimeSpan time = TimeSpan.Parse(lesson.start_time);

            if (time > TimeSpan.Parse(lesson.end_time))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין שעת התחלה ושעת סיום תקינה");
            }

            lesson l = db.lessons.FirstOrDefault(x => x.rider_id == lesson.rider_id && x.date == date && x.start_time == time);
            if (l!=null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "לתלמיד זה כבר קיים שיעור באותו המועד");
            }
            else
            {
                List<lesson> lessons = db.lessons.Where(x => x.date == date && x.start_time == time).ToList();
                List<trial_lesson> trial_lessons = db.trial_lesson.Where(x => x.date == date && x.start_time == time).ToList();

                int count = 0;

                foreach (lesson less in lessons)
                {

                    if (lesson.lesson_type=="קבוצתי" && less.lesson_type=="קבוצתי" && less.instructor_id == lesson.instructor_id)
                    {
                        count++;
                        if (count>6)
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "לא ניתן להוסיף מעל 6 משתתפים לשיעור קבוצתי");
                        }
                    }

                    if (less.instructor_id==lesson.instructor_id && less.lesson_type !="קבוצתי")
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "למדריך שנבחר כבר קיים שיעור באותו המועד");
                    }

                    if(less.horse_id == lesson.horse_id && less.horse_id!=null)
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

                rider r = db.riders.SingleOrDefault(x=>x.id==lesson.rider_id);
                worker w = db.workers.SingleOrDefault(x => x.id == lesson.instructor_id);
                horse h = db.horses.SingleOrDefault(x => x.id == lesson.horse_id);


                lesson new_lesson = new lesson
                {
                    rider_id = lesson.rider_id,
                    rider = r,
                    date = DateTime.Parse(lesson.date),
                    start_time = TimeSpan.Parse(lesson.start_time),
                    end_time = TimeSpan.Parse(lesson.end_time),
                    field = lesson.field,
                    instructor_id = lesson.instructor_id,
                    worker = w,
                    funding_source = lesson.funding_source,
                    price = lesson.price,
                    charge_type = lesson.charge_type,
                    lesson_type = lesson.lesson_type,
                    comments = lesson.comments,
                    match_rank = 0
                };

                if (h != null)
                {
                    new_lesson.horse_id = lesson.horse_id;
                    new_lesson.horse = h;
                }                

                db.lessons.Add(new_lesson);
            }

            try
            {
                db.SaveChanges();

                lesson last_lesson = db.lessons.OrderByDescending(x => x.lesson_id).FirstOrDefault();

                notification n = new notification
                {
                    user_id = lesson.rider_id,
                    title = "מילוי משוב",
                    text = "לא לשכוח למלא משוב לגבי השיעור שהיה היום :)",
                    lesson_id = last_lesson.lesson_id,
                    was_sent = false
                };

                db.notifications.Add(n);

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
        [Route("api/Lesson/{id}")]
        public HttpResponseMessage Put(int id, [FromBody]LessonDTO lesson)
        {
            horseClubDbContext db = new horseClubDbContext();
            lesson l = db.lessons.SingleOrDefault(x=>x.lesson_id==id);

            DateTime date = DateTime.Parse(lesson.date);
            TimeSpan time = TimeSpan.Parse(lesson.start_time);

            if (l!=null)
            {
                List<lesson> lessons = db.lessons.Where(x => x.date == date && x.start_time == time).ToList();
                List<trial_lesson> trial_lessons = db.trial_lesson.Where(x => x.date == date && x.start_time == time).ToList();

                foreach (lesson less in lessons)
                {
                    if (less.lesson_id!=l.lesson_id)
                    {
                        if (less.instructor_id == lesson.instructor_id && less.lesson_type != "קבוצתי")
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "למדריך שנבחר כבר קיים שיעור באותו המועד");
                        }

                        if (less.horse_id == lesson.horse_id)
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "הסוס שנבחר משובץ לשיעור אחר באותו המועד");
                        }
                    }
                }

                foreach (trial_lesson less in trial_lessons)
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

                worker w = db.workers.SingleOrDefault(x => x.id == lesson.instructor_id);
                horse h = db.horses.SingleOrDefault(x => x.id == lesson.horse_id);

                l.field = lesson.field;
                if (h != null)
                {
                    if (l.horse_id!=lesson.horse_id)
                    {
                        l.match_rank = 0;
                    }

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
                l.lesson_type = lesson.lesson_type;
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

        [HttpGet]
        [Route("api/Lesson/Match/Criteria")]
        public HttpResponseMessage GetCriteria()
        {
            horseClubDbContext db = new horseClubDbContext();

            List<match_criteriaDTO> match_Criteria = db.match_criteria.Select(c => new match_criteriaDTO()
            {
                criterion_id = c.criterion_id,
                criterion_name = c.criterion_name,
                criterion_description = c.criterion_description,
                last_update = c.last_update,
                criterion_weight = c.criterion_weight,
                max_weight = c.max_weight,
                min_weight = c.min_weight
            }).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, match_Criteria);
        }

        [HttpPut]
        [Route("api/Lesson/Match/Criteria/{id}")]
        public HttpResponseMessage PutCriteria(string id, List<match_criteriaDTO> match_Criteria)
        {
            horseClubDbContext db = new horseClubDbContext();

            double total_rank = 0;
            DateTime now = DateTime.Now;

            foreach (var new_criterion in match_Criteria)
            {
                match_criteria c = db.match_criteria.SingleOrDefault(x => x.criterion_id == new_criterion.criterion_id);

                if (c.min_weight<= new_criterion.criterion_weight && c.max_weight>= new_criterion.criterion_weight)
                {
                    if (c.criterion_weight != new_criterion.criterion_weight)
                    {
                        c.criterion_weight = new_criterion.criterion_weight;
                        c.last_update = now;
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, $"שגיאה בקריטריון של {c.criterion_description} - יש לתת לקריטריון זה משקל בטווח התקין");
                }

                if (new_criterion.criterion_id!=8 && new_criterion.criterion_id != 14)
                {
                    total_rank += (double)new_criterion.criterion_weight;
                }
            }

            if (total_rank>1)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, $"יש לשנות את סכום המשקלים כך שסכומם לא יעלה על 1 (ללא קריטריון מספר 2 ו-8).");
            }

            try
            {
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "הקריטריונים עודכנו בהצלחה");

            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }


        // PUT api/<controller>/start_date
        [HttpPut]
        [Route("api/Lesson/Match/{start_date}")]

        public HttpResponseMessage PutHorse(string start_date,[FromBody]string end_date)
        {
            horseClubDbContext db = new horseClubDbContext();

            DateTime start_week = DateTime.Parse(start_date);
            DateTime end_week = DateTime.Parse(end_date);

            if (start_week > end_week)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "יש להזין תאריכי שיבוץ תקינים");
            }

                                                //שליפת משקלי קריטריונים להתאמה בין סוס לתלמיד

            var criteria = db.match_criteria.ToList();
            Dictionary<string,double?> criteria_dictionary = criteria.ToDictionary(x => x.criterion_name, y => y.criterion_weight);

                                                //group all past lesson of riders by rider id
                                                //קיבוץ שיעורי עבר על פי תעודת זהות של רוכב

            var past_lessons = db.lessons.Where(x => x.rider.isActive == true && x.date<start_week && x.match_rank!=0).GroupBy(r => r.rider_id).ToList();

                                                 //order past lesson list by avarge match rank of each rider 
                                                 //מיון רשימת השיעורים לפי ממוצע ציון התאמה של התלמיד לסוסים בסדר עולה

            past_lessons =  past_lessons.OrderBy(x => x.Average(m => m.match_rank)).ToList();

                                                //get next week lessons & available horses
                                                //שליפת שיעורים בשבוע הנבחר ורשימת סוסים כשירים

            List<lesson> future_lessons = db.lessons.Where(x => x.date >= start_week && x.date <= end_week).ToList();
            List<horse> horses = db.horses.Where(x => x.is_qualified == true && x.is_active==true).ToList();

                                               //loop over all riders by the avarge match rank - starting with the lowest rank
                                               //מעבר על רשימת הרוכבים הפעילים - המתחילה בתלמיד בעל מממוצע ההתאמה הנמוך ביותר 

            foreach (var lesson in past_lessons)
            {
                rider rider = db.riders.SingleOrDefault(x => x.id == lesson.Key);

                                               //get rider's lessons of next week
                                               //שליפת שיעורי התלמיד בשבוע שנבחר

                List<lesson> rider_future_lessons = future_lessons.Where(x => x.rider_id == rider.id).ToList();

                if (rider_future_lessons != null)
                {
                    foreach (lesson future_lesson in rider_future_lessons)
                    {
                        if (future_lesson.horse_id == null)
                        {
                            horse chosen_horse = new horse();
                            double max_rank = 0;

                                                 //loop over all horses and create a rank based on different parameters
                                                 //מעבר על רשימת הסוסים הכשירים

                            foreach (horse horse in horses)
                            {
                                                 //find if there are parallel or close lessons that the horse is assign to
                                                 //איתור שיעורים מקבילים או קרובים על מנת למנוע שיבוץ כפול של סוס וזמני מנוחה

                                List<lesson> parallel_lessons = future_lessons.Where(x => x.date == future_lesson.date && x.start_time == future_lesson.start_time && x.horse_id == horse.id).ToList();
                                List<lesson> following_lessons = future_lessons.Where(x => x.date == future_lesson.date && (x.start_time == future_lesson.end_time || x.end_time ==future_lesson.start_time) && x.horse_id == horse.id).ToList();

                                if (parallel_lessons.Count==0 && following_lessons.Count==0)
                                {
                                    double rank = 0;

                                    if (horse.therapeutic_riding == true && rider.riding_type == "רכיבה טיפולית")
                                    {
                                        rank += (double)criteria_dictionary["therapeutic_riding_type"];

                                    }

                                    if (horse.therapeutic_riding == true && rider.riding_type == "רכיבה ספורטיבית")
                                    {
                                        rank += (double)criteria_dictionary["sports_riding_type"];
                                    }

                                    if (horse.min_weight <= rider.weight && horse.max_weight >= rider.weight)
                                    {
                                        rank += (double)criteria_dictionary["rider_weight"];
                                    }

                                    if (horse.min_height <= rider.height && horse.max_height >= rider.height)
                                    {
                                        rank += (double)criteria_dictionary["rider_height"];
                                    }

                                    if (horse.can_jump == true && rider.riding_rank >= 4)
                                    {
                                        rank += (double)criteria_dictionary["horse_can_jump"];
                                    }

                                    if (horse.required_rank <= rider.riding_rank)
                                    {
                                        rank += (double)criteria_dictionary["riding_rank"];
                                    }
                                                       //get past lessons of the rider with the horse for avarge match rank
                                                       //שליפת שיעורי עבר של תלמיד עם סוס מסויים לחישוב ממוצע ציון התאמה

                                    List<lesson> past_lessons_with_horse = db.lessons.Where(x => x.rider_id == rider.id && x.horse_id == horse.id).ToList();

                                    if (past_lessons_with_horse.Count != 0)
                                    {
                                        double avg_match_with_horse = past_lessons_with_horse.Where(x => x.match_rank!=0).Average(x => x.match_rank).Value;

                                        avg_match_with_horse *= (double)criteria_dictionary["avg_match_rank"];     //multiply avarge rank with the horse (based of previous lessons)
                                        rank += avg_match_with_horse;
                                    }

                                                      //if the rider had this horse in the last lesson - subtract from the rank 
                                                      //ציון ההתאמה יירד במידה והסוס היה משובץ לשיעור האחרון עם אותו תלמיד

                                    lesson last_lesson = past_lessons.First(x => x.Key == rider.id).OrderByDescending(x => x.date).FirstOrDefault();

                                    if (last_lesson != null && last_lesson.horse_id == horse.id)
                                    {
                                        rank += (double)criteria_dictionary["was_in_last_lesson"];
                                    }

                                    if (rank > max_rank)
                                    {
                                        if (rank<0)
                                        {
                                            rank = 0;
                                        }

                                        if (rank > 1)
                                        {
                                            rank = 1;
                                        }

                                        max_rank = rank;
                                        chosen_horse = horse;
                                    }
                                }
                            }                       //שמירת הסוס בעל ציון ההתאמה הגבוה ביותר עבור אותו תלמיד והציון     

                            future_lesson.horse_id = chosen_horse.id;
                            future_lesson.match_rank = max_rank;
                            future_lesson.horse = chosen_horse;
                        }
                    }
                }
            }
            try
            {
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "השיבוץ בוצע בהצלחה");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }

        // DELETE api/<controller>/5
        [HttpDelete]
        [Route("api/Lesson/{lesson_id}")]
        public HttpResponseMessage Delete(int lesson_id)
        {
            horseClubDbContext db = new horseClubDbContext();
            lesson l = db.lessons
                         .Include(x => x.instructor_feedback)
                         .Include(x => x.rider_feedback)
                         .Include(x => x.notifications)
                         .SingleOrDefault(x => x.lesson_id == lesson_id);

            if (l != null)
            {
                List<instructor_feedback> instructor_Feedbacks = l.instructor_feedback.ToList();

                for (int i = 0; i < instructor_Feedbacks.Count; i++)
                {
                    l.instructor_feedback.Remove(instructor_Feedbacks[i]);
                    db.instructor_feedback.Remove(instructor_Feedbacks[i]);
                }

                List<rider_feedback> rider_feedbacks = l.rider_feedback.ToList();

                for (int i = 0; i < rider_feedbacks.Count; i++)
                {
                    l.rider_feedback.Remove(rider_feedbacks[i]);
                    db.rider_feedback.Remove(rider_feedbacks[i]);
                }

                List<notification> lesson_notifications = l.notifications.ToList();

                for (int i = 0; i < lesson_notifications.Count; i++)
                {
                    l.notifications.Remove(lesson_notifications[i]);
                    db.notifications.Remove(lesson_notifications[i]);
                }

                db.lessons.Remove(l);
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