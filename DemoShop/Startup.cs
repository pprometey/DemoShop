using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DemoShop.UI.Data;
using DemoShop.Core.Domain;
using DemoShop.Core.Services;
using SparkPostDotNet.Core;
using SparkPostDotNet;
using DemoShop.Core.Infrastructure;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Rewrite;

namespace DemoShop
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
            services.AddOptions();

            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddDbContext<ShopDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                b => b.UseRowNumberForPaging()));

            services.AddIdentity<ShopUser, ShopRole>(config =>
            {
                config.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<ShopDbContext>()
                .AddErrorDescriber<EnglishIdentityErrorDescriber>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                //options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromDays(30);
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout"; 
                options.AccessDeniedPath = "/Account/AccessDenied"; 
                options.SlidingExpiration = true;
            });

            //EmailProviders
            services.AddSparkPost();
            services.Configure<SparkPostOptions>(options => Configuration.GetSection("SparkPost").Bind(options));
            services.Configure<SparkPostSenderOptions>(options => Configuration.GetSection("SparkPostSenderOptions").Bind(options));
            services.AddTransient<SparkPostSender>();

            services.Configure<SMTPSenderOptions>(options => Configuration.GetSection("SMTPSenderOptions").Bind(options));
            services.AddTransient<SMTPSender>();

            services.Configure<EmailSenderOptions>(options => Configuration.GetSection("EmailSenderOptions").Bind(options));
            services.AddScoped<IEmailSender, EmailSender>();

            //---DB Services
            //Goods
            services.AddScoped<IGoodsDbContext, ShopDbContext>();
            services.AddScoped<IGoodsDbRepositories, GoodsDbRepositories>();
            //Purchases
            services.AddScoped<IPurchasesDbContext, ShopDbContext>();
            services.AddScoped<IPurchasesDbRepositories, PurchasesDbRepositories>();
            //Sales
            services.AddScoped<ISalesDbContext, ShopDbContext>();
            services.AddScoped<ISalesDbRepositories, SalesDbRepositories>();

            services.AddRecaptcha(new RecaptchaOptions
            {
                SiteKey = Configuration["Recaptcha:SiteKey"],
                SecretKey = Configuration["Recaptcha:SecretKey"],
                ValidationMessage = "Вы не прошли провекру на робота.",
                LanguageCode = "en"
            });

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });

            //MVC
            services.AddMvc()
             .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSession();

            app.UseAuthentication();

            app.UseRewriter(new RewriteOptions()
                .AddRedirectToHttps());

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
