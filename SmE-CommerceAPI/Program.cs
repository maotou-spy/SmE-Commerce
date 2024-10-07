using SmE_CommerceRepositories;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices;
using SmE_CommerceServices.Interface;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SmE_CommerceModels.DatabaseContext;
using SmE_CommerceModels.Models;
using SmE_CommerceUtilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

#region Timezone

var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // +7
builder.Services.AddSingleton(timeZoneInfo);

#endregion

#region Database

builder.Services.AddScoped<DefaultdbContext>();

#endregion

#region Service

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

#endregion

#region Repository

builder.Services.AddScoped<IUserRepository, UserRepository>();

#endregion

#region Auth

//
// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin();
    });
});

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmE-Commerce"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseAuthentication();

app.MapControllers();

app.UseCors("AllowAll");

app.Run();
