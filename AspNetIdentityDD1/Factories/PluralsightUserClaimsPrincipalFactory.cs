using AspNetIdentityDD1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetIdentityDD1.Factories
{
	public class PluralsightUserClaimsPrincipalFactory
		: UserClaimsPrincipalFactory<PluralsightUser>
	{
		public PluralsightUserClaimsPrincipalFactory(UserManager<PluralsightUser> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
		{
		}

		protected override async Task<ClaimsIdentity> GenerateClaimsAsync(PluralsightUser user)
		{
			var identity = await base.GenerateClaimsAsync(user);
			identity.AddClaim(new Claim("locale", user.Locale));
			return identity;
		}
	}
}
