using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetIdentityDD1.Models
{
	public class PluralsightUser : IdentityUser
	{
		public string Locale { get; set; } = "en-US";
		public string OrgId { get; set; }
	}
}
