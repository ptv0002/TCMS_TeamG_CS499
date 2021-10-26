using DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Mail;
using Microsoft.AspNetCore.Http;

namespace TCMS_Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register AppDbContext, use to connect to MS SQL Server
            services.AddDbContext<TCMS_Context>(options => {
                // Read Connection String
                string connectstring = Configuration.GetConnectionString("DefaultConnection");
                // Use MS SQL Server
                options.UseSqlServer(connectstring);
            });

            // Register Identity services
            services.AddIdentity<Employee, IdentityRole>()
                .AddEntityFrameworkStores<TCMS_Context>()
                .AddDefaultTokenProviders();

            // Access IdentityOptions
            services.Configure<IdentityOptions>(options =>
            {
                // Config Password
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 3;
                options.Password.RequiredUniqueChars = 1;

                // Config User
                options.User.AllowedUserNameCharacters = // Characters allowed for username
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;

                // Config SignIn
                options.SignIn.RequireConfirmedEmail = true;

                // Config Cookie
                services.ConfigureApplicationCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    options.LoginPath = $"/Account/Login";                                 // Url login page
                    options.LogoutPath = $"/Account/Logout";
                    options.AccessDeniedPath = $"/Account/AccessDenied";
                });
                services.Configure<SecurityStampValidatorOptions>(options =>
                {
                    // Re login after 10 seconds will have to re-enter login info
                    // SecurityStamp in User table change -> re-enter info for Security
                    options.ValidationInterval = TimeSpan.FromSeconds(10);
                }); 
            });

            services.AddAuthorization();
            services.AddControllersWithViews();  
            // Handle email settings
            services.AddOptions();                                         // Activate Options
            var mailsettings = Configuration.GetSection("MailSettings");   // Read config
            services.Configure<MailSettings>(mailsettings);                // Register to inject
            services.AddTransient<ISendMailService, SendMailService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // Recover login info (confirmation)

            app.UseAuthorization(); // Recover User role info

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=ShippingAssignment}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                      name: "areas",
                      pattern: "{area:exists}/{controller=NoRole}/{action=Index}/{id?}");


                // Test Email directory
                //endpoints.MapGet("/", async context => {
                //    await context.Response.WriteAsync("Hello World!");
                //});

                endpoints.MapGet("/testmail", async context => {

                    // Get service sendmailservice
                    var sendmailservice = context.RequestServices.GetService<ISendMailService>();

                    MailContent content = new MailContent
                    {
                        To = "ptv0002@uah.edu",
                        Subject = "TCMS email test",
                        Body = "<p><strong>Test test test...............</strong></p>"
                    };

                    await sendmailservice.SendMail(content);
                    await context.Response.WriteAsync("Send mail");
                });
            });
        }
    }
}
