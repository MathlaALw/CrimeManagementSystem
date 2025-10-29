using Microsoft.AspNetCore.Authentication.JwtBearer;
using Crime_Management_System.Data;
using Crime_Management_System.Mapping;
using Crime_Management_System.Repos.Implementations;
using Crime_Management_System.Services.Implementations;
using Crime_Management_System.Services.Interfaces;
using Crime_Management_System.Models;
using Crime_Management_System.Repos;
using Crime_Management_System.Servises;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Crime_Management_System.Repos.Implementations;
using Crime_Management_System.Repositories.Implementations;
using Crime_Management_System.Middleware;
using Microsoft.IdentityModel.Tokens;
using System.Text;



namespace Crime_Management_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure DbContext with SQL Server
            builder.Services.AddDbContext<CrimeDbContext>(o =>
            o.UseSqlServer(builder.Configuration.GetConnectionString("CrimeDb")));
            // Configure AutoMapper
            builder.Services.AddAutoMapper(typeof(CrimeMappingProfile));


            // Register repositories
            //builder.Services.AddScoped<IUserRepository, UserRepository>();
            //builder.Services.AddScoped<ICaseRepository, CaseRepository>();
            builder.Services.AddScoped<IReportRepo, ReportRepo>();
            builder.Services.AddScoped<IEvidenceRepository, EvidenceRepository>();
            builder.Services.AddScoped<IParticipantRepo, ParticipantRepo>();
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
            builder.Services.AddScoped<JwtService>();



            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
                    };
                });
            builder.Services.AddAuthorization();


            // Register services
            //builder.Services.AddScoped<IUserService, UserService>();
            //builder.Services.AddScoped<ICaseService, CaseService>();
            builder.Services.AddScoped<IReportService, ReportService>();
            builder.Services.AddScoped<IEvidenceService, EvidenceService>();
            builder.Services.AddScoped<IParticipantService, ParticipantService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            // Update the namespace for IUserRepository to match the one used by UserRepository
           // builder.Services.AddScoped<Crime_Management_System.Repos.Implementations.IUserRepository, UserRepository>();

            // User and Case Repositories and Services
            builder.Services.AddScoped<IUserRepository, UserRepository>();


            builder.Services.AddScoped<ICaseRepository, CaseRepository>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICaseService, CaseService>();

            var app = builder.Build();

            // Seed the database
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<Crime_Management_System.Data.CrimeDbContext>();
               // db.Database.Migrate();
                SeedData.seed(db);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<JwtMiddleware>(); // Custom middleware for token extraction

            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
