using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetIdentityDD1.Models;
using System.Threading;
using System.Data.SqlClient;
using System.Data.Common;
using Dapper;

namespace AspNetIdentityDD1
{
	public class IdentityUserStore : IUserStore<User>
	{

		public static DbConnection GetOpenConnection()
		{
			var connection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=IdentityDemo;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
			connection.Open();
			return connection;
		}

		public async Task<IdentityResult> CreateAsync(Models.User user, CancellationToken cancellationToken)
		{
			using (var connection = GetOpenConnection())
			{
				await connection.ExecuteAsync(
					"insert into Users(Id, UserName, NormalizedUserName, PasswordHash) " + 
					"values(@id, @userName, @normalizedUserName, @passwordHash)",
					new
					{
						id = user.Id,
						userName = user.UserName,
						normalizedUserName = user.NormalizedUserName,
						passwordHash = user.PasswordHash
					}
				);
			}
			return IdentityResult.Success;
		}

		public Task<IdentityResult> DeleteAsync(Models.User user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
		}

		public async Task<Models.User> FindByIdAsync(string userId, CancellationToken cancellationToken)
		{
			using (var connection = GetOpenConnection())
			{
				return await connection.QueryFirstOrDefaultAsync<User>(
					"select * from Users where Id = @id", 
					new
					{
						id = userId
					}
				);
			}
		}

		public async Task<Models.User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
		{
			using (var connection = GetOpenConnection())
			{
				return await connection.QueryFirstOrDefaultAsync<User>(
					"select * from Users where NormalizedUserName = @name",
					new
					{
						name = normalizedUserName
					}
				);
			}
		}

		public Task<string> GetNormalizedUserNameAsync(Models.User user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.NormalizedUserName);
		}

		public Task<string> GetUserIdAsync(Models.User user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.Id);
		}

		public Task<string> GetUserNameAsync(Models.User user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.UserName);
		}

		public Task SetNormalizedUserNameAsync(Models.User user, string normalizedName, CancellationToken cancellationToken)
		{
			user.UserName = normalizedName;
			return Task.CompletedTask;
		}

		public Task SetUserNameAsync(Models.User user, string userName, CancellationToken cancellationToken)
		{
			user.UserName = userName;
			return Task.CompletedTask;
		}

		public Task<IdentityResult> UpdateAsync(Models.User user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
