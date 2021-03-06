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
    
    public partial class Patient
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Patient()
        {
            this.PatientIndicators = new HashSet<PatientIndicator>();
            this.PatientProviders = new HashSet<PatientProvider>();
            this.PatientSurveys = new HashSet<PatientSurvey>();
            this.UserSurveys = new HashSet<UserSurvey>();
        }
    
        public System.Guid ID { get; set; }
        public System.Guid UserID { get; set; }
        public string IHINumber { get; set; }
        public string MedicareNumber { get; set; }
        public string Code { get; set; }
        public Nullable<int> ContactID { get; set; }
        public Nullable<short> PatientCategoryID { get; set; }
        public Nullable<long> AddressID { get; set; }
        public string Salutation { get; set; }
        public string Description { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.Guid> ModifiedBy { get; set; }
    
        public virtual Address Address { get; set; }
        public virtual Contact Contact { get; set; }
        public virtual PatientCategory PatientCategory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PatientIndicator> PatientIndicators { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PatientProvider> PatientProviders { get; set; }
        public virtual UserDetail UserDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PatientSurvey> PatientSurveys { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserSurvey> UserSurveys { get; set; }
    }
}
