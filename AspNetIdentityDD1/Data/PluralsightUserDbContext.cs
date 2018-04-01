using AspNetIdentityDD1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetIdentityDD1.Data
{
	public class PluralsightUserDbContext : IdentityDbContext<PluralsightUser>
	{
		public PluralsightUserDbContext(DbContextOptions<PluralsightUserDbContext> options) : base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.Entity<PluralsightUser>(user => user.HasIndex(x => x.Locale).IsUnique(false));
			builder.Entity<Organization>(org => 
			{
				org.ToTable("Organization");
				org.HasKey(x => x.Id);
				org.HasMany<PluralsightUser>().WithOne().HasForeignKey(x => x.OrgId).IsRequired(false);
			});
		}
	}
}
