using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using UrlShortenerBackend.Constants;
using UrlShortenerBackend.Data;
using UrlShortenerBackend.Filters;
using UrlShortenerBackend.Interfaces;
using UrlShortenerBackend.MappingProfiles;
using UrlShortenerBackend.Middlewares;
using UrlShortenerBackend.Models;
using UrlShortenerBackend.Services;

var builder = WebApplication.CreateBuilder(args);


// Load application configuration

builder.Configuration
    .AddJsonFile(
        "appsettings.json",
        optional: false,
        reloadOnChange: true)
    .AddJsonFile(
        $"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: true,
        reloadOnChange: true)
    .AddEnvironmentVariables();

var provider =
    builder.Configuration["DatabaseSettings:Provider"]
    ?? throw new Exception(
        "Database provider not configured.");


// Bind AppSettings

builder.Services.Configure<AppSettings>(
    builder.Configuration);


// Configure database

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.ConfigureWarnings(w =>
        w.Ignore(
            Microsoft.EntityFrameworkCore.Diagnostics
                .RelationalEventId
                .PendingModelChangesWarning));

    if (provider.ToLower() == "sqlserver")
    {
        options.UseSqlServer(
            builder.Configuration.GetConnectionString(
                "SqlServer"));
    }
});


// Configure JWT authentication

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!))
            };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();

                context.Response.StatusCode =
                    StatusCodes.Status401Unauthorized;

                context.Response.ContentType =
                    "application/json";

                var result = new
                {
                    status = false,
                    httpStatus = 401,
                    message =
                        ResponseMessages.SessionExpired
                };

                return context.Response.WriteAsJsonAsync(
                    result);
            }
        };
    });


// Add authorization

builder.Services.AddAuthorization();


// Configure Swagger

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "UrlShortener-Backend",
            Version = "v1"
        });


    // JWT authentication in Swagger

    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Description =
                "Enter JWT token using Bearer authentication.",

            Name = "Authorization",

            In = ParameterLocation.Header,

            Type = SecuritySchemeType.ApiKey,

            Scheme = "Bearer"
        });


    // API Key authentication in Swagger

    c.AddSecurityDefinition(
        "ApiKey",
        new OpenApiSecurityScheme
        {
            Description =
                "Enter API Key.",

            Name = "AK",

            In = ParameterLocation.Header,

            Type = SecuritySchemeType.ApiKey,

            Scheme = "ApiKey"
        });


    c.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(
                "Bearer",
                document)]
                = new List<string>(),

            [new OpenApiSecuritySchemeReference(
                "ApiKey",
                document)]
                = new List<string>()
        });
});


// Register logging service

builder.Services.AddSingleton<LoggingService>();
builder.Services.AddScoped<CustomExceptionFilter>();

builder.Services
    .AddControllers(options =>
    {
        options.Filters.Add<CustomExceptionFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization
                .ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddHttpContextAccessor();

// Register AutoMapper

builder.Services.AddAutoMapper(
    _ => { },
    typeof(MappingProfile));


// Register API Key service

builder.Services.AddSingleton<
    IApiKeyService,
    ApiKeyService>();


// Register application services

builder.Services.AddScoped<
    IUserService,
    UserService>();

builder.Services.AddScoped<
    ILoginService,
    LoginService>();

builder.Services.AddScoped<
    ILoginTokenService,
    LoginTokenService>();

builder.Services.AddScoped<
    IUrlService,
    UrlService>();

builder.Services.AddScoped<
    IUserContextService,
    UserContextService>();


// Existing credential services

builder.Services.AddScoped<
    ICredentialService,
    CredentialService>();

builder.Services.AddScoped<
    ICredentialValidationService,
    CredentialValidationService>();


// External URL service

builder.Services.AddScoped<
    IExternalUrlService,
    ExternalUrlService>();


// Build application

var app = builder.Build();


// Enable Swagger

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}


// Redirect HTTP to HTTPS

app.UseHttpsRedirection();


// Validate API Key

app.UseMiddleware<ApiKeyMiddleware>();


// Validate JWT

app.UseAuthentication();


// Check authorization

app.UseAuthorization();


// Map controllers

app.MapControllers();


// Start application

app.Run();