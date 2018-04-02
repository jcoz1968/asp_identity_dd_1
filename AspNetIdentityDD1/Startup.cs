using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspNetIdentityDD1.Data;
using AspNetIdentityDD1.Factories;
using AspNetIdentityDD1.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetIdentityDD1
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();
			var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
			var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PluralsightUser;" +
				"Integrated Security=True;Connect Timeout=30;" +
				"Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

			services.AddDbContext<PluralsightUserDbContext>(opt => opt.UseSqlServer(connectionString, sql => 
				sql.MigrationsAssembly(migrationAssembly.ToString())
			));

			services.AddIdentity<PluralsightUser, IdentityRole>(opt => { })
				.AddEntityFrameworkStores<PluralsightUserDbContext>()
				.AddDefaultTokenProviders();

			services.AddScoped<IUserClaimsPrincipalFactory<PluralsightUser>, PluralsightUserClaimsPrincipalFactory>();

			services.ConfigureApplicationCookie(opt => opt.LoginPath = "/Home/Login");

			services.Configure<DataProtectionTokenProviderOptions>(opt => 
				opt.TokenLifespan = TimeSpan.FromHours(3));



		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseBrowserLink();
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseAuthentication();

			app.UseStaticFiles();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
