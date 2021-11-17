using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PursiXMVC.Data;

namespace PursiXMVC
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
            //You will need this and using Microsoft.Extensions.Configuration to get appsetting.json connectionstrings to work, also you need to add context to your controllers too.
            services.AddDbContext<PursiDBContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("PursiDB")));

            //services.AddControllersWithViews();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            //Login stuff
            services.AddMvcCore()
                .AddDataAnnotations();

            services.AddMvc(
                options => {
                    options.EnableEndpointRouting = false;
                    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
                    _ => "Tämä tieto on pakollinen.");

                }
                );

            services.AddCors(options =>
            {
                options.AddPolicy("MyCorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

            });

            services.AddDistributedMemoryCache();
            services.AddSession();

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

            app.UseAuthorization();

            //Login stuff
            app.UseSession();
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
