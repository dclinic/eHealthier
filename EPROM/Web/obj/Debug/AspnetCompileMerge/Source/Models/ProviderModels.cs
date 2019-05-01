using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace ePRom.Models
{
    public class ProviderContext : DbContext
    {
        public ProviderContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }

    public class ProviderRegisterModel
    {        
        public short SecretQuestionID { get; set; }
        public string Answer { get; set; }
        public Guid? UserId { get; set; }
        public bool? isPatient { get; set; }
        public string Role { get; set; }
        //[Display(Name = "Landline")]
        //public string Landline { get; set; }

        //[Display(Name = "Mobile")]
        //public string Mobile { get; set; }

        //[Display(Name = "Fax")]
        //public string Fax { get; set; }

        //[Display(Name = "Pager")]
        //public string Pager { get; set; }

        //[Display(Name = "Emergency")]
        //public string Emergency { get; set; }

        //[Display(Name = "Skype")]
        //public string Skype { get; set; }

        //[DataType(DataType.EmailAddress)]
        //[EmailAddress(ErrorMessage = "Invalid Email Address")]
        //[Display(Name = "EmailOrg")]
        //public string EmailOrg { get; set; }

        //[DataType(DataType.EmailAddress)]
        //[EmailAddress(ErrorMessage = "Invalid Email Address")]
        //[Display(Name = "EmailPrivate")]
        //public string EmailPrivate { get; set; }

        //[Display(Name = "LinkedIN")]
        //public string LinkedinUrl { get; set; }

        //[Display(Name = "My URL")]
        //public string MyUrl { get; set; }

        //[Display(Name = "Medicare Provider Number")]
        //public string MedicareProviderNumber { get; set; }

        //[Display(Name = "Health Provider Identifier")]
        //public string HealthProviderIdentifier { get; set; }

        //[Display(Name = "My PHN")]
        //public string MyPHN { get; set; }

        //[Display(Name = "My LHD")]
        //public string MyLHD { get; set; }

        //[Display(Name = "First name")]
        //public string FirstName { get; set; }

        //[Display(Name = "Last name")]
        //public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        //[Required]
        //[DataType(DataType.PhoneNumber)]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        //[Display(Name = "ContactNo")]
        //public string ContactNo { get; set; }

        //[Required]
        //[Display(Name = "IsActive")]
        //public bool IsActive { get; set; }

        //[Required]
        //[Display(Name = "User name")]
        //public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        //[Display(Name = "Secret Question")]
        //public string SecretQuestion { get; set; }

        //[Display(Name = "Secret Answer")]
        //public string SecretAnswer { get; set; }
    }

    public class SecretQuestionModel
    {
        public int ID { get; set; }
        public string Questions { get; set; }
        public bool? IsActive { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
