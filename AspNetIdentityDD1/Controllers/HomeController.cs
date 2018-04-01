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
						UserName = model.UserName
					};
					var result = await _userManager.CreateAsync(user, model.Password);
					if(result == null)
					{

					}
				}
				return View("Success");
			}
			return View();
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

				if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
				{
					//var identity = new ClaimsIdentity("Identity.Application");
					//identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
					//identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
					var principal = await _claimsPrincipalFactory.CreateAsync(user);
					await HttpContext.SignInAsync("Identity.Application", principal);
					return RedirectToAction("Index");
				}

				//SignInManager alternate method
				//var signInResult = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);

				//if(signInResult.Succeeded)
				//{
				//	return RedirectToAction("Index");
				//}

				ModelState.AddModelError("", "Invalid UserName or Password");
			}

			return View();
		}
	}

}
