using CardboardBox.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace MangaDexHooks.Core;

using Auth;
using Database;
using Database.Models;
using Database.Services;

public static class DiExtensions
{
	public static bool IsSqlite(this IConfiguration config)
	{
		return bool.TryParse(config["Database:Sqlite"], out var sqlite) && sqlite;
	}

	public static IServiceCollection AddWebHooks(this IServiceCollection services, IConfiguration config) 
	{
		var isSqlite = config.IsSqlite();
		return services
			.AddWebHooks(isSqlite)
			.AddJwt(config);
	}

	public static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration config)
	{
		services
			.AddAuthentication(opt =>
			{
				opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(opt =>
			{
				opt.SaveToken = true;
				opt.RequireHttpsMetadata = false;
				opt.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidAudience = config["OAuth:Audience"],
					ValidIssuer = config["OAuth:Issuer"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["OAuth:Key"] ?? throw new ArgumentNullException("OAuth:Key", "Required setting is not present")))
				};
			});

		return services;
	}

	public static IServiceCollection AddWebHooks(this IServiceCollection services, bool sqlite = true)
	{
		return services
			.AddSqlService(c =>
			{
				c.ConfigureGeneration(a => a.WithCamelCaseChange())
				 .ConfigureTypes(a =>
				 {
					 a.CamelCase()
					  .Entity<DbWebhookResult>()
					  .Entity<DbWebhook>()
					  .Entity<DbProfile>();

					 a.DefaultJsonHandler<Webhook?>(() => null);

					 if (!sqlite) return;

					 a.PolyfillBooleanHandler()
					  .PolyfillDateTimeHandler()
					  .ArrayHandler<string>()
					  .ArrayHandler<int>()
					  .ArrayHandler<double>();
				 });

				if(!sqlite) 
				{
					c.AddPostgres<SqlConfig>(a => a.OnInit(con => ExecuteFiles(con, "*.postgres.sql")));
					return;
				}

				c.AddSQLite<SqlConfig>(a => a.OnInit(con => ExecuteFiles(con, "*.sqlite.sql")));
			})
			.AddCardboardHttp()
			.AddTransient<IWebhookApiService, WebhookApiService>()
			.AddTransient<IOAuthService, OAuthService>()
			.AddTransient<ITokenService, TokenService>()
			.AddTransient<IFakeUpsertQueryService, FakeUpsertQueryService>()
			
			.AddTransient<IWebhookDbService, WebhookDbService>()
			.AddTransient<IProfileDbService, ProfileDbService>()
			
			.AddTransient<IDbService, DbService>();
	}

	public static async Task ExecuteFiles(IDbConnection con, string extension)
	{
		var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts");
		if (!Directory.Exists(path)) return;

		var files = Directory.GetFiles(path, extension, SearchOption.AllDirectories)
			.OrderBy(t => Path.GetFileName(t))
			.ToArray();

		if (files.Length <= 0) return;

		foreach (var file in files)
		{
			var context = await File.ReadAllTextAsync(file);
			await con.ExecuteAsync(context);
		}
	}
}
