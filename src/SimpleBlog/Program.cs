using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;
using SimpleBlog.Authorization;
using SimpleBlog.Database;
using SimpleBlog.Models;
using SimpleBlog.Services;
using SimpleBlog.StartupTasks;

namespace SimpleBlog;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //Database configuration
        builder.Services.AddDbContext<BlogDatabase>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            options.UseOpenIddict();
        });
        builder.Services.AddScoped<IBlogDatabase>(provider => provider.GetRequiredService<BlogDatabase>());

        //HTTPS Hsts configuration
        builder.Services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromMinutes(60); //TimeSpan.FromDays(60);
        });

        builder.Services.AddHttpsRedirection(options => { });

        //MVC
        builder.Services.AddMvcCore().AddDataAnnotations();

        //Authentication and Authorization
        builder.Services
            .AddIdentity<Account, AccountRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequiredLength = 1;
            })
            .AddEntityFrameworkStores<BlogDatabase>();//.AddDefaultTokenProviders();

        builder.Services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore().UseDbContext<BlogDatabase>();
            })
            .AddServer(options =>
            {
                options.SetTokenEndpointUris("/auth/token");
                options.AllowPasswordFlow().AllowRefreshTokenFlow();
                options.UseAspNetCore().EnableTokenEndpointPassthrough();//.DisableTransportSecurityRequirement();
                options.AddDevelopmentEncryptionCertificate().AddDevelopmentSigningCertificate().DisableAccessTokenEncryption();
            })
            .AddValidation(options =>
            {
                //Import the configuration from the local OpenIddict server instance.
                options.UseLocalServer();
                //Register the ASP.NET Core host.
                options.UseAspNetCore();
            });

        builder.Services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy(Policies.SameOwner, policy => policy.Requirements.Add(new SameOwnerRequirement()));
            options.AddPolicy(Policies.PostTagFullAccess, policy => policy.Requirements.Add(new PostTagFullAccessRequirement()));
        });

        builder.Services.AddSingleton<IAuthorizationHandler, PostSameOwnerAuthorizationHandler>();
        builder.Services.AddSingleton<IAuthorizationHandler, PostTagFullAccessAuthorizationHandler>();

        //MyServices
        builder.Services.AddScoped<IProfileService, ProfileService>();
        builder.Services.AddScoped<IPostService, PostService>();
        builder.Services.AddScoped<IPostTagService, PostTagService>();

        //StartupTasks
        builder.Services.AddHostedService<StartupTasksRunner>();

        //Middlewares
        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            //Unhandled Exceptions
            app.UseExceptionHandler((app) => app.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Internal Server Error");
            }));
            //HTTPS 
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapFallbackToFile("index.html");

        app.Run();
    }
}