//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Contact
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Contact()
        {
            this.Patients = new HashSet<Patient>();
            this.Providers = new HashSet<Provider>();
        }
    
        public int ID { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Mobile2 { get; set; }
        public string FAX { get; set; }
        public string Pager { get; set; }
        public string Emergency { get; set; }
        public string EmailBusiness { get; set; }
        public string EmailPersonal { get; set; }
        public string Skype { get; set; }
        public string LinkedIN { get; set; }
        public string Website { get; set; }
        public string OtherFieldsJSON { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.Guid> ModifiedBy { get; set; }
        public Nullable<System.Guid> UserId { get; set; }
    
        public virtual UserDetail UserDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Patient> Patients { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Provider> Providers { get; set; }
    }
}
