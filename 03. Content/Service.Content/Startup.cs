using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.User;
using Library.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Sdk.User;
using Service.Auth;
using Service.Content.Data;

namespace Service.Content
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Service.Content", Version = "v1" });
            });

            //Database connection inizialization.
            string connectionString = Configuration.GetConnectionString("ContentDatabase");
            services.AddDbContext<ContentDbContext>(options => options.UseSqlServer(connectionString));
            
            // Database auto-migration
            DbContextOptionsBuilder<ContentDbContext> optionsBuilder = new DbContextOptionsBuilder<ContentDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            ContentDbContext dbContext = new ContentDbContext(optionsBuilder.Options);
            dbContext.Database.Migrate();

            //User client library. To make external call authenticated with internal-inter-service token
            UserCli user = new UserCli(CommonConstants.SERVICE_LOCAL_URL_USER, null);
            user.UseSession(JwtHelper.GenerateJwtToken(Program.SERVICE_TAG, Program.SERVICE_TAG, Program.SERVICE_TAG));
            services.AddSingleton<IUserApi>(user);

            //Allow requests from any base URL for debuf purposes.
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder => { builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Service.Content v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
