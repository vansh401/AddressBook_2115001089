using BusinessLayer.Interface;
using BusinessLayer.Service;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using RepositoryLayer.Context;
using RepositoryLayer.Helper;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CacheLayer.Service;
using StackExchange.Redis;
using Middleware.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);
var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();

logger.Info("Application Starting...");

// Adding Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));

// JWT Authentication Configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
        };
    });


// Configure Redis connection
var redisConfig = builder.Configuration.GetSection("Redis:ConnectionString").Value;
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig));

builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();


builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Dependency Injection for Services
builder.Services.AddScoped<IAddressBookService, AddressBookService>();
builder.Services.AddScoped<IAddressBookRL, AddressBookRL>();
builder.Services.AddScoped<IUserBL, UserBL>();
builder.Services.AddScoped<IUserRL, UserRL>();
builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<RabbitMQConsumer>();
builder.Services.AddSingleton<RabbitMQProducer>();

// NLog Configuration
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
builder.Host.UseNLog();

// Swagger for API documentation
builder.Services.AddSwaggerGen();




var app = builder.Build();

// rabbit config
var rabbitConsumer = app.Services.GetRequiredService<RabbitMQConsumer>();
Task.Run(()=> rabbitConsumer.StartListening());


// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
