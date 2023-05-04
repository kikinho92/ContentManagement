using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Service.Auth.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.HttpOverrides;
using Api.User;
using Sdk.User;
using Library.Common;

namespace Service.Auth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private readonly string customCorsPolicy = "customCorsPolicy";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIROMENT") == "Development";
            bool isStaging = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIROMENT") == "Staging";

            //Database connection inizialization.
            string connectionString = Configuration.GetConnectionString("AuthDatabase");
            services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(connectionString));

            // Database auto-migration
            DbContextOptionsBuilder<AuthDbContext> optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            AuthDbContext dbContext = new AuthDbContext(optionsBuilder.Options);
            dbContext.Database.Migrate();

            //User client library. To make external call authenticated with internal-inter-service token
            string userApiUrl = isStaging ? CommonConstantsStaging.SERVICE_LOCAL_URL_USER : CommonConstants.SERVICE_LOCAL_URL_USER;
            UserCli user = new UserCli(userApiUrl, null);
            user.UseSession(JwtHelper.GenerateJwtToken(Program.SERVICE_TAG, Program.SERVICE_TAG, Program.SERVICE_TAG));
            services.AddSingleton<IUserApi>(user);

            services.AddIdentity<IdentityUser, IdentityRole>(options => {
                //Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;

                //Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                //User settings
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

            // TODO In case we implement multi-languaje text
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwt => {
                byte[] key = Encoding.ASCII.GetBytes(JwtHelper.SecretKeyProvider());

                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters{
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                };
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Service.Auth", Version = "v1" });
            });

            //Allow requests from any base URL for debug purposes
            /*services.AddCors(options =>{
                options.AddDefaultPolicy(builder => { builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials(); });
            } );*/

            services.AddCors(options =>
            {
                options.AddPolicy(name: customCorsPolicy,
                                policy  =>
                                {
                                    policy.WithOrigins("http://10.96.249.12",
                                                        "http://10.96.249.12:8005",
                                                        "http://localhost",
                                                        "http://localhost:8005")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowCredentials();
                                });
            });

            //Lets the controller know the external URL used to reach it
            services.Configure<ForwardedHeadersOptions>(options => {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Service.Auth v1"));
            }

            //Allow requests from any base URL for debug purposes
            app.UseCors(customCorsPolicy);
            //Lets the controller know the external URL used to reach it
            app.UseForwardedHeaders();
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
