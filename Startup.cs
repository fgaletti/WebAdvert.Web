using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using WebAdvert.Web.ServiceClients;
using WebAdvert.Web.Services;

namespace WebAdvert.Web
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
            //services.AddMvc(option => option.EnableEndpointRouting = false);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc(option => option.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddCognitoIdentity(config =>
            {
                config.Password = new Microsoft.AspNetCore.Identity.PasswordOptions
                {
                    RequireDigit = false,
                    RequiredLength = 6,
                    RequiredUniqueChars = 0,
                    RequireLowercase = false,
                    RequireNonAlphanumeric = false,
                    RequireUppercase = false
                };
            });


            // 14 
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Accounts/Login";
            });

            services.AddAutoMapper();
            services.AddTransient<IFileUploader, S3FileUploader>(); // 22 
                                                                    // services.AddHttpClient<IAdvertApiClient, AdvertApiClient>(); //24 

            services.AddHttpClient<IAdvertApiClient, AdvertApiClient>();
            // 26 Add extension Polly
            //services.AddHttpClient<IAdvertApiClient, AdvertApiClient>().AddPolicyHandler(GetRetryPolicy()) //26
            //         .AddPolicyHandler(GetCircuitBreakerPatternPolicy());

            //  services.AddRazorPages();
        }

        private IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPatternPolicy()
        {
            // any call should be bropken until 30 seconds
            return HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));
        }

        private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions.HandleTransientHttpError().OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                 .WaitAndRetryAsync(5, retryAttempy => TimeSpan.FromSeconds(Math.Pow(2, retryAttempy)));
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
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


        }
    }
}
