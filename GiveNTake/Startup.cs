using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GiveNTake.Infrastructure.ApiErrors;
using GiveNTake.Infrastructure.CorrelationID;
using GiveNTake.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace GiveNTake
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(
                config =>
                {
                    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                    config.Filters.Add(new AuthorizeFilter(policy));
                    config.Filters.Add<GlobalExceptionFilter>();
                });
            services.AddDbContext<GiveNTakeContext>(options => options.UseSqlServer(Configuration.GetConnectionString("GiveNTakeDB")));
            services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<GiveNTakeContext>()
                .AddDefaultTokenProviders();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwtOptions =>
            {
                jwtOptions.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateActor = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = Configuration["JWTConfiguration:Issuer"],
                    ValidAudience = Configuration["JWTConfiguration:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWTConfiguration:SigningKey"]))
                };
            });

            services.AddAuthorization(options => options.AddPolicy("ExperiencedUser", (AuthorizationPolicyBuilder policy) =>
            {
                policy.RequireAssertion(context =>
                {
                    var registrationClaimValue = context.User.Claims.SingleOrDefault(c => c.Type == "registration-date")?.Value;
                    if (DateTime.TryParseExact(registrationClaimValue, "yy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var registrationTime))
                    {
                        return registrationTime.AddYears(1) < DateTime.UtcNow;
                    }
                    return false;
                });
            }));

            services.AddCors();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            var appInsightsLogLevel = Configuration.GetValue<LogLevel>("Logging:Application Insights:LogLevel:Default");
            loggerFactory.AddApplicationInsights(app.ApplicationServices, appInsightsLogLevel);
            //Each response will now include a 'X-Correlation-ID' header 
            app.UseCorrelationIdHeader();
            app.UseDefaultFiles();
            app.UseCors(b =>
            {
                b.AllowAnyHeader();
                b.AllowAnyOrigin();
                b.AllowAnyMethod();
            });
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}")
                    .MapRoute(name: "api", template: "api/{controller=Messages}/{action=My}/{id:int?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
