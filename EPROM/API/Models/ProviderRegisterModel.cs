﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class ProviderRegisterModel
    {
        public short SecretQuestionID { get; set; }
        public string Answer { get; set; }
        public Guid? UserId { get; set; }
        public bool? isPatient { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}