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
    
    public partial class regular_lesson
    {
        public int lesson_id { get; set; }
        public string day { get; set; }
        public System.TimeSpan start_time { get; set; }
        public Nullable<System.TimeSpan> end_time { get; set; }
        public string lesson_type { get; set; }
        public Nullable<int> price { get; set; }
        public string funding_source { get; set; }
        public string rider_id { get; set; }
    
        public virtual rider rider { get; set; }
    }
}
