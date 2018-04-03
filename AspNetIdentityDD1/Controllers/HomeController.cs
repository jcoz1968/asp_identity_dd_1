using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetIdentityDD1.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace AspNetIdentityDD1.Controllers
{
	public class HomeController : Controller
	{
		private UserManager<PluralsightUser> _userManager;
		private IUserClaimsPrincipalFactory<PluralsightUser> _claimsPrincipalFactory;
		private SignInManager<PluralsightUser> _signInManager;

		public HomeController(UserManager<PluralsightUser> userManager,
			IUserClaimsPrincipalFactory<PluralsightUser> claimsPrincipalFactory,
			SignInManager<PluralsightUser> signInManager)
		{
			_userManager = userManager;
			_claimsPrincipalFactory = claimsPrincipalFactory;
			_signInManager = signInManager;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult About()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		public IActionResult Contact()
		{
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterModel model)
		{
			if(ModelState.IsValid)
			{
				var user = await _userManager.FindByNameAsync(model.UserName);


				if(user == null)
				{
					user = new PluralsightUser
					{
						Id = Guid.NewGuid().ToString(),
						UserName = model.UserName, 
						Email = model.UserName
					};
					var result = await _userManager.CreateAsync(user, model.Password);
					if(result.Succeeded)
					{
						var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
						var confirmationEmail = Url.Action("ConfirmEmailAddress", "Home", new
						{
							token = token,
							email = user.Email
						}, Request.Scheme);
						await System.IO.File.WriteAllTextAsync("emailConfirmLink.txt", confirmationEmail);
					}
				}
				return View("Success");
			}
			return View();
		}

		public async Task<IActionResult> ConfirmEmailAddress(string token, string email)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if(user != null)
			{
				var result = await _userManager.ConfirmEmailAsync(user, token);
				if(result.Succeeded)
				{
					return View("Success");
				}
			}
			return View("Error");
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByNameAsync(model.UserName);

				if(user != null && await _userManager.IsLockedOutAsync(user))
				{
					if (await _userManager.CheckPasswordAsync(user, model.Password))
					{
						//only allow if email is confirmed flow
						if (!await _userManager.IsEmailConfirmedAsync(user))
						{
							ModelState.AddModelError("", "Email is not confirmed");
							return View();
						}
						await _userManager.ResetAccessFailedCountAsync(user);

						//Two Factor flow
						if(await _userManager.GetTwoFactorEnabledAsync(user))
						{
							var validProviders = await _userManager.GetValidTwoFactorProvidersAsync(user);

							//Authenticator Provider
							if(validProviders.Contains(_userManager.Options.Tokens.AuthenticatorTokenProvider))
							{
								await HttpContext.SignInAsync(IdentityConstants.TwoFactorUserIdScheme,
									Store2FA(user.Id, _userManager.Options.Tokens.AuthenticatorTokenProvider));
								return RedirectToAction("TwoFactor");
							}

							//Email Provider
							if(validProviders.Contains("Email"))
							{
								var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
								await System.IO.File.WriteAllTextAsync("email2sv.txt", token);

								await HttpContext.SignInAsync(IdentityConstants.TwoFactorUserIdScheme, 
									Store2FA(user.Id, "Email"));
								return RedirectToAction("TwoFactor");
							}
						}

						//Two factor NOT enabled gets here
						var principal = await _claimsPrincipalFactory.CreateAsync(user);
						await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);
						return RedirectToAction("Index");
					}
					ModelState.AddModelError("", "Invalid UserName or Password");
				}
				await _userManager.AccessFailedAsync(user);
				if(await _userManager.IsLockedOutAsync(user))
				{
					// email user, notifying them of lockout
				}
			}
			return View();
		}

		private ClaimsPrincipal Store2FA(string userid, string provider)
		{
			var identity = new ClaimsIdentity(new List<Claim>
			{
				new Claim("sub", userid),
				new Claim("amr", provider)
			}, IdentityConstants.TwoFactorUserIdScheme);
			return new ClaimsPrincipal(identity);
		}

		[HttpGet]
		public IActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
		{
			if(ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				if(user != null)
				{
					var token = await _userManager.GeneratePasswordResetTokenAsync(user);
					var resetUrl = Url.Action("ResetPassword", "Home", new
					{
						token = token, 
						email = user.Email
					}, Request.Scheme);
					await System.IO.File.WriteAllTextAsync("resetLink.txt", resetUrl);
				}
				{
					// email user and inform them that they do not have an account
				}
				return View("Success");
			}
			return View();
		}

		[HttpGet]
		public IActionResult ResetPassword(string token, string email)
		{
			return View(new ResetPasswordModel { Token = token, Email = email });
		}

		[HttpPost]
		public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				if (user != null)
				{
					var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
					if (!result.Succeeded)
					{
						foreach (var error in result.Errors)
						{
							ModelState.AddModelError("", error.Description);
						}
						return View();
					}
					if (await _userManager.IsLockedOutAsync(user))
					{
						await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
					}
					return View("Success");
				}
				ModelState.AddModelError("", "Invalid Request");
			}
			return View();
		}

		[HttpGet]
		public IActionResult TwoFactor()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> TwoFactor(TwoFactorModel model)
		{
			var result = await HttpContext.AuthenticateAsync(IdentityConstants.TwoFactorUserIdScheme);
			if(!result.Succeeded)
			{
				ModelState.AddModelError("", "Your login request has expired. please start over");
				return View();
			}
			if(ModelState.IsValid)
			{
				var user = await _userManager.FindByIdAsync(result.Principal.FindFirstValue("sub"));
				if(user != null)
				{
					var isValid = await _userManager.VerifyTwoFactorTokenAsync(user,
						result.Principal.FindFirstValue("amr"), model.Token);
					if(isValid)
					{
						await HttpContext.SignOutAsync(IdentityConstants.TwoFactorUserIdScheme);

						var claimsPrincipal = await _claimsPrincipalFactory.CreateAsync(user);
						await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, claimsPrincipal);
						return RedirectToAction("Index");
					}

					ModelState.AddModelError("", "Invalid token");
					return View();
				}
				ModelState.AddModelError("", "Invalid Request");
			}
			return View();
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> RegisterAuthenticator()
		{
			var user = await _userManager.GetUserAsync(User);

			var authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);

			if (authenticatorKey == null)
			{
				await _userManager.ResetAuthenticatorKeyAsync(user);
				authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
			}

			return View(new RegisterAuthenticatorModel { AuthenticatorKey = authenticatorKey });
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> RegisterAuthenticator(RegisterAuthenticatorModel model)
		{
			var user = await _userManager.GetUserAsync(User);

			var isValid = await _userManager.VerifyTwoFactorTokenAsync(user,
				_userManager.Options.Tokens.AuthenticatorTokenProvider, model.Code);

			if (!isValid)
			{
				ModelState.AddModelError("", "Code is invalid");
				return View(model);
			}

			await _userManager.SetTwoFactorEnabledAsync(user, true);
			return View("Success");
		}


	}

}
