using Amazon.Runtime;
using Amazon.S3;
using DevJob.API.Middleware;
using DevJob.Application.Configurations;
using DevJob.Application.DTOs.Auth;
using DevJob.Application.Repository_Contract;
using DevJob.Application.ServiceContract;
using DevJob.Application.Validation;
using DevJob.Domain.Entities;
using DevJob.Infrastructure.Data;
using DevJob.Infrastructure.Hubs;
using DevJob.Infrastructure.Repositories;
using DevJob.Infrastructure.Service;
using DevJob.Infrastructure.Validation;
using FluentValidation;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Net.WebSockets;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
// Add services to the container.

var minioSection = builder.Configuration.GetSection("MinIO");

var s3Config = new AmazonS3Config
{
    ServiceURL = "https://s3.us-east-005.backblazeb2.com",
    ForcePathStyle = true
};
builder.Services.AddSingleton<IAmazonS3>(sp =>
    new AmazonS3Client(minioSection["AccessKey"], minioSection["SecretKey"], s3Config));


builder.Services.AddControllers()
    .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(
    x =>
    {
        x.UseSqlServer(builder.Configuration["cs"]);

    }

    );
builder.Services.Configure<GeminiConfig>(builder.Configuration.GetSection("GeminiSetting"));
Console.WriteLine(builder.Configuration["cs"]);
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(x => x.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddHangfire(config =>
{
    config.UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration["hangfireCs"]);
});

builder.Services.AddHangfireServer();
builder.Services.AddScoped<IChatServices, ChatServices>();
//Repo Layer
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IUserCvDataRepository, UserCvDataRepository>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IUserSkillsRepository, UserSkillRepository>();
builder.Services.AddScoped<IUserJobRepository, UserJobRepository>();
builder.Services.AddScoped<IRecommendedJobRepository, RecommendedJobsRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtServices, JwtServices>();
builder.Services.AddScoped<IBackgroundService, DevJob.Infrastructure.Service.BackgroundService>();
builder.Services.AddTransient<IMailServices, MailServices>();
builder.Services.AddTransient<IPasswordHash, PasswordHash>();
builder.Services.AddScoped<IUploadToAzure, UploadToAzure>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICompanyServices, CompanyServices>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJobServices, JobService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IUploadToAzure, UploadToAzure>();
builder.Services.AddScoped<ICVServices, CvServices>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IGeminiService, GeminiService>();
builder.Services.AddScoped<IStorageService, BackBlazeService>();
builder.Services.AddScoped<SkillsService>();
builder.Services.AddTransient<IValidator<CompanyRegisterDTO>, RegisterValidation>();
builder.Services.AddScoped<IMockInterviewService, MockInterviewService>();
//builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<JobValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateCompanyDataValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateProfileValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<UploadLogoValidate>();
builder.Services.AddSignalR();

builder.Services.Configure<MailSetting>(builder.Configuration.GetSection("MailSetting"));
//builder.Services.AddAuthentication()
//    .AddGoogle
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFlutter",
        policy =>
        {
            policy.AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials()
                  .SetIsOriginAllowed((host) => true); // Allow all origins
        });
});


Console.WriteLine("Validation Key: " + builder.Configuration["Jwt:Key"]);

//JWT
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
 .AddJwtBearer(options => {
     options.TokenValidationParameters = new TokenValidationParameters()
     {
         ValidateAudience = true,
         ValidAudience = builder.Configuration["Jwt:Audience"],
         ValidateIssuer = true,
         ValidIssuer = builder.Configuration["Jwt:Issuer"],
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
     };
 });
builder.Services.AddAuthentication()
    .AddGoogle(opt =>
    {
        opt.ClientId = builder.Configuration["Authentication:Google:ClientID"];
        opt.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });
builder.Services.AddAuthorization(options => {
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.UseHangfireDashboard();

app.UseHttpsRedirection();
app.UseCors("AllowFlutter");
app.MapControllers();
app.MapHub<NotificationHub>("/NotificationHub");
app.MapHub<MessageHub>("/messageHub");
app.MapHub<JobHub>("/jobHub");
//app.Services.RegisterRecurringJobs();
app.Run();