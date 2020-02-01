using System.Net;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using DateApp.DAL.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DateApp.API.Helpers;
using DatingApp.API.Filters;
using DateApp.Core.Contracts.DAL;
using DateApp.DAL.Implementation;
using Date.API.Configs;
using DatingApp.Model.Mappers;
using DateApp.DAL.Abstraction;
using DateApp.API.Configs;
using DateApp.DAL.Mappers;
using Microsoft.OpenApi.Models;
using DateApp.Core.Contracts.Services;
using DateApp.Business.Services;

namespace DateApp.API
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
            services.AddControllers().AddNewtonsoftJson();
            //.AddJsonOptions(opt=>
            //{
            //    opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //});
            var dbconnstr = Configuration["DatingAppDatabase"];
            services.AddCors();
            services.AddSingleton<ICommonConfigurations, CommonConfigurations>();
            services.AddSingleton<IUserMapper, UserMapper>();
            services.AddTransient<Seed>();
            services.AddDbContext<DataContext>(x => x.UseSqlServer(Configuration["DatingAppDatabase"]));
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            //    .AddJsonOptions(opt =>
            //    {
            //        opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //    });
            var cloudName = Configuration["CloudName"];
            var apiKey = Configuration["ApiKey"];
            var apiSecret = Configuration["ApiSecret"];
            var cloudinarySettings = new CloudinarySettings
            {
                ApiKey = apiKey,
                ApiSecret = apiSecret,
                CloudName = cloudName
            };
            services.AddSingleton(cloudinarySettings);
            //services.AddCors();
            services.AddAutoMapper(typeof(AutoMapperProfiles));
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IBaseRepository, BaseRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPhotoRepository, PhotoRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["Token"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DateApp API Endpoints", Version = "v1" });
            });

            services.AddScoped<LogUserActivityFilter>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Seed seeder)
        {
            UpdateDatabase(app);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //seed.SeedData();
            }
            else
            {
                // app.UseHsts();
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();

                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
            }

            

            app.UseHttpsRedirection();

            app.UseRouting();


            app.UseAuthentication();
            app.UseAuthorization();

            // seeder.SeedUserData();
            app.UseCors(
                options => options.SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowAnyHeader().AllowCredentials()
            );

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DateApp API V1");
            });
            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
        private void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<DataContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
