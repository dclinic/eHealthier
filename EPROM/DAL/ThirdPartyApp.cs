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
    
    public partial class ThirdPartyApp
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ThirdPartyApp()
        {
            this.ProviderPatientThirdPartyApps = new HashSet<ProviderPatientThirdPartyApp>();
        }
    
        public int ID { get; set; }
        public string AppName { get; set; }
        public string URL { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string PhoneNo { get; set; }
        public Nullable<short> SurveyCategoryID { get; set; }
        public Nullable<short> SurveySubCategoryID { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.DateTime> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedBy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProviderPatientThirdPartyApp> ProviderPatientThirdPartyApps { get; set; }
        public virtual SurveyCategory SurveyCategory { get; set; }
        public virtual SurveyCategory SurveyCategory1 { get; set; }
    }
}
