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
    
    public partial class rider
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public rider()
        {
            this.parents = new HashSet<parent>();
            this.regular_lesson = new HashSet<regular_lesson>();
            this.lessons = new HashSet<lesson>();
        }
    
        public string id { get; set; }
        public Nullable<int> weight { get; set; }
        public Nullable<double> height { get; set; }
        public Nullable<int> riding_rank { get; set; }
        public string riding_type { get; set; }
        public Nullable<System.DateTime> starting_date { get; set; }
        public string instructor_id { get; set; }
        public Nullable<int> horse_id { get; set; }
        public Nullable<bool> isActive { get; set; }
    
        public virtual horse horse { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<parent> parents { get; set; }
        public virtual worker worker { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<regular_lesson> regular_lesson { get; set; }
        public virtual user user { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<lesson> lessons { get; set; }
    }
}
