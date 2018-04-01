using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetIdentityDD1.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetIdentityDD1.Controllers
{
	public class HomeController : Controller
	{
		private UserManager<User> _userManager;

		public HomeController(UserManager<User> userManager)
		{
			_userManager = userManager;
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

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterModel model)
		{
			if(ModelState.IsValid)
			{
				var user = await _userManager.FindByNameAsync(model.UserName);


				if(user == null)
				{
					user = new User
					{
						Id = Guid.NewGuid().ToString(),
						UserName = model.UserName
					};
					var result = await _userManager.CreateAsync(user, model.Password);
				}
				return View("Success");
			}
			return View();
		}
	}

}
