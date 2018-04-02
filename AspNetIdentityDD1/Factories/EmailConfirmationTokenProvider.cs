using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetIdentityDD1.Factories
{
	public class EmailConfirmationTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
	{
		public EmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider, 
			IOptions<EmailConfirmationTokenProviderOptions> options) : base(dataProtectionProvider, options)
		{

		}
	}
}
