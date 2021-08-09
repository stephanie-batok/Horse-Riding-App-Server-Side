using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATA.EF;
using WebApi.Controllers;


namespace WebApi.Models
{
    //code for timer
    public static class TimerServices
    {
        //code for timer
        public static void CheckRiderFeedbackReminder()
        {
            horseClubDbContext db = new horseClubDbContext();
            DateTime now = DateTime.Now.Date;

            List<lesson> lessons = db.lessons.Where(x => x.date == now).ToList();

            foreach (var less in lessons)
            {
                List<notification> notifications = db.notifications.Where(x => x.lesson_id == less.lesson_id).ToList();
                
                foreach (var n in notifications)
                {
                    if (!(bool)n.was_sent && less.end_time < DateTime.Now.TimeOfDay)
                    {
                        user u = db.users.SingleOrDefault(x=>x.id==n.user_id);

                        if (u.token!=null)
                        {
                            var objectToSend = new
                            {
                                to = u.token,
                                title = "מועדון רכיבה רעננה",
                                body = "התקבלה התראה חדשה",
                                badge = 4,
                                data = new { chat_num = 0, from_id = "", action = "notification" }
                            };

                            string res = notificationServices.PushNotificationSend(objectToSend);
                        }

                        DateTime today = DateTime.Now;
                        n.dateTime = today;
                        n.timeStr = today.TimeOfDay.ToString("hh\\:mm");
                        n.dateStr = today.Date.ToString("dd'/'MM'/'yyyy");
                        n.was_sent = true;

                        db.SaveChanges();
                    }
                }
            }
        }

        public static void CheckInstructorFeedbackReminder()
        {
            horseClubDbContext db = new horseClubDbContext();
            DateTime now = DateTime.Now.Date;

            var lessons = db.lessons.Where(x => x.date == now).GroupBy(x=>x.instructor_id).ToList();

            foreach (var lessonGroup in lessons)
            {
                user u = db.users.SingleOrDefault(x => x.id == lessonGroup.Key);

                if (now.TimeOfDay >= TimeSpan.Parse("20:00") && now.TimeOfDay < TimeSpan.Parse("21:00"))
                {
                    if (u.token!=null)
                    {
                        var objectToSend = new
                        {
                            to = u.token,
                            title = "מועדון רכיבה רעננה",
                            body = "התקבלה התראה חדשה",
                            badge = 4,
                            data = new { chat_num = 0, from_id = "", action = "notification" }
                        };

                        string res = notificationServices.PushNotificationSend(objectToSend);
                    }

                    DateTime today = DateTime.Now;
                    notification n = new notification
                    {
                        dateTime = today,
                        timeStr = today.TimeOfDay.ToString("hh\\:mm"),
                        dateStr = today.Date.ToString("dd'/'MM'/'yyyy"),
                        user_id = u.id,
                        title = "מילוי משובים",
                        text = "לא לשכוח למלא משוב לשיעורים שהיו היום :)",
                        was_sent = true,
                    };

                    db.notifications.Add(n);
                    db.SaveChanges();
                }
            }
        }

        public static void CheckHorseMatchReminder()
        {
            DateTime now = DateTime.Now;

            if (now.DayOfWeek == DayOfWeek.Friday)
            {
                if (now.TimeOfDay >= TimeSpan.Parse("20:00") && now.TimeOfDay < TimeSpan.Parse("21:00"))
                {
                    string start_date = now.AddDays(1).Date.ToString();
                    now = DateTime.Now;
                    string end_date = now.AddDays(8).Date.ToString();
                    LessonController controller = new LessonController();
                    controller.PutHorse(start_date, end_date);
                }
            }
        }
    }
}