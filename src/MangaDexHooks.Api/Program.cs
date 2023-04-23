using MangaDexHooks.Api;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi.Models;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "JWT Authorization header using the Bearer scheme."
	});
	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			Array.Empty<string>()
		}
	});
});

builder.Services.AddWebHooks(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseExceptionHandler(err =>
{
	err.Run(async ctx =>
	{
		ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
		ctx.Response.ContentType = Application.Json;
		var error = app.Environment.IsDevelopment() ? 
			ctx.Features.Get<IExceptionHandlerFeature>()?.Error ??
			ctx.Features.Get<IExceptionHandlerPathFeature>()?.Error : null;

		await ctx.Response.WriteAsJsonAsync(ApiResults.Error(error ?? new Exception("An error has occurred, please reach out on discord for more information!")));
	});
});

app.UseCors(c =>
{
	c.AllowAnyHeader()
	 .AllowAnyMethod()
	 .AllowAnyOrigin()
	 .WithExposedHeaders("Content-Disposition");
});

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
