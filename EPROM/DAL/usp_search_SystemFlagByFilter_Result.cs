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
    
    public partial class usp_search_SystemFlagByFilter_Result
    {
        public Nullable<long> RowNo { get; set; }
        public int ID { get; set; }
        public string SystemFlagName { get; set; }
        public int FlagGroupID { get; set; }
        public string DefaultValue { get; set; }
        public string Value { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
