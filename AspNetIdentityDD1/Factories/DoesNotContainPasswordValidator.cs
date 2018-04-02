using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetIdentityDD1.Factories
{
	public class DoesNotContainPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : class
	{
		public DoesNotContainPasswordValidator()
		{

		}
		public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
		{
			var username = await manager.GetUserNameAsync(user);
			if(username == password)
			{
				return IdentityResult.Failed(new IdentityError
				{
					Description = "Password cannot contain username"
				});
			}
			if(password.Contains("password"))
			{
				return IdentityResult.Failed(new IdentityError
				{
					Description = "Password cannot contain the word 'password'"
				});
			}
			return IdentityResult.Success;
		}
	}
}
