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
    
    public partial class PatientSurveys_Temp
    {
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> PatientID { get; set; }
        public Nullable<int> SurveyID { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.Guid> ProviderID { get; set; }
        public Nullable<System.Guid> OrganizationID { get; set; }
        public Nullable<System.Guid> PracticeID { get; set; }
    }
}