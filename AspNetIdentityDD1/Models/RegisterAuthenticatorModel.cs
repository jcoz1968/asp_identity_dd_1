﻿using System.ComponentModel.DataAnnotations;

namespace AspNetIdentityDD1.Models
{
    public class RegisterAuthenticatorModel
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string AuthenticatorKey { get; set; }
    }
}