using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        //code for timer
        static Timer riderFeedbackTimer = new Timer();
        static Timer instructorFeedbackTimer = new Timer();
        static Timer horseMatchTimer = new Timer();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //code for rider reminder timer
            riderFeedbackTimer.Interval = 3600000;
            riderFeedbackTimer.Elapsed += Rider_tm_Tick;

            //code for rider reminder timer
            instructorFeedbackTimer.Interval = 3600000;
            instructorFeedbackTimer.Elapsed += Instructor_tm_Tick;

            //code for horse match
            horseMatchTimer.Interval = 3600000;
            horseMatchTimer.Elapsed += HorseMatch_tm_Tick;

        }

        //code for timer
        private void Rider_tm_Tick(object sender, ElapsedEventArgs e)
        {
            //EndTimer();
            WebApi.Models.TimerServices.CheckRiderFeedbackReminder();
        }

        private void Instructor_tm_Tick(object sender, ElapsedEventArgs e)
        {
            //EndTimer();
            WebApi.Models.TimerServices.CheckInstructorFeedbackReminder();
        }

        private void HorseMatch_tm_Tick(object sender, ElapsedEventArgs e)
        {
            //EndTimer();
            WebApi.Models.TimerServices.CheckHorseMatchReminder();
        }

        //code for timer
        public static void StartTimer()
        {
            riderFeedbackTimer.Enabled = true;
            instructorFeedbackTimer.Enabled = true;
            horseMatchTimer.Enabled = true;
        }

        public static void EndTimer()
        {
            riderFeedbackTimer.Enabled = false;
            instructorFeedbackTimer.Enabled = false;
            horseMatchTimer.Enabled = false;
        }
    }
}
