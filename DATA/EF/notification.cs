//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DATA.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class notification
    {
        public int notification_id { get; set; }
        public string user_id { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        public string dateStr { get; set; }
        public string timeStr { get; set; }
        public Nullable<System.DateTime> dateTime { get; set; }
        public Nullable<int> lesson_id { get; set; }
        public Nullable<bool> was_sent { get; set; }
    
        public virtual user user { get; set; }
        public virtual lesson lesson { get; set; }
    }
}
