using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TravelGalleryWeb.Data;
using TravelGalleryWeb.Models;
using TravelGalleryWeb.Pages.Admin;

namespace TravelGalleryWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment appEnvironment)
        {
            Configuration = configuration;
            AppEnvironment = appEnvironment;
            
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(appEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true);
            Configuration = builder.Build();
            
            using (ApplicationContext context = new ApplicationContext())
            {
                context.Database.Migrate();
                if (context.Admins.Any()) return;
                var admin = new Admin()
                {
                    Login = "admin",
                    Password = configuration.GetSection("Constants")["DefaultPass"], //"qwerty"
                    LastChanged = DateTime.UtcNow,
                    Role = Role.Administrator
                };
                context.Admins.Add(admin);
                context.SaveChanges();
            }

            //This ensures that all the directories for file uploading are created 
            System.IO.Directory.CreateDirectory(AppEnvironment.WebRootPath +
                                                Configuration.GetSection("Constants")["UploadDir"]);
        }

        private IConfiguration Configuration { get; }
        private IHostingEnvironment AppEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
            });
            services.AddTransient<IStorageOperations, CloudinaryOperations>();


            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/Admin").AllowAnonymousToPage("/Admin/Signin");
                    options.Conventions.AuthorizeFolder("/Admin/Admins", "RequireAdministratorRole");
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddEntityFrameworkNpgsql().AddDbContext<ApplicationContext>();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.AccessDeniedPath = "/Admin/Signin";
                    options.LoginPath = "/Admin/Signin";
                    options.LogoutPath = "/Index";
                    options.EventsType = typeof(CustomCookieAuthenticationEvents);
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Administrator"));
            });
            services.AddScoped<CustomCookieAuthenticationEvents>();
            
            services.AddOptions();
            services.Configure<Constants>(Configuration.GetSection("Constants"));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }
            
            var cookiePolicyOptions = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Lax,
            };
            
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy(cookiePolicyOptions);
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}