using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Repositories;
using Ecommerce;

namespace EcommerceEndUser
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins("https://localhost:44327");
                                  });
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services
                .AddControllersWithViews();
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });
            services.AddMvc()
               .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(100);//You can set Time  
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                //options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;

            });
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
            });
            services.AddHttpContextAccessor();
            services.AddControllers();
            string connectionString = Configuration.GetConnectionString("Default");
            services
                .AddDbContext<EcommerceDbContext>(options => options
                    .UseMySql(
                        connectionString,
                        ServerVersion.AutoDetect(connectionString)
                    )
            );
            //db context - end
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                app.UseDeveloperExceptionPage();
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseCors(MyAllowSpecificOrigins);

            app.UseSession();

            app.UseCookiePolicy();

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            
            var provider = new FileExtensionContentTypeProvider();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=HomeEndUser}/{action=Index}/{id?}");
     
		});
        }
    }
}







