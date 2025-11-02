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
using Crime_Management_System.Repositories.Implementations;
using Crime_Management_System.Middleware;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Crime_Management_System.Helper;
using Microsoft.OpenApi.Models;
using System.Security.Claims;



namespace Crime_Management_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ---------- DATABASE ----------
            builder.Services.AddDbContext<CrimeDbContext>(o =>
                o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ---------- AUTOMAPPER ----------
            builder.Services.AddAutoMapper(typeof(CrimeMappingProfile));

            // ---------- REPOSITORIES ----------
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ICaseRepository, CaseRepository>();
            builder.Services.AddScoped<IReportRepo, ReportRepo>();
            builder.Services.AddScoped<IEvidenceRepository, EvidenceRepository>();
            builder.Services.AddScoped<IParticipantRepo, ParticipantRepo>();

            // ---------- SERVICES ----------
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICaseService, CaseService>();
            builder.Services.AddScoped<IReportService, ReportService>();
            builder.Services.AddScoped<IEvidenceService, EvidenceService>();
            builder.Services.AddScoped<IParticipantService, ParticipantService>();
            builder.Services.AddScoped<IPasswordService, PasswordService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            // builder.Services.AddScoped<JwtService>(); // scoped, safe now with middleware fix

            // ---------- JWT CONFIG ----------
            var jwtSection = builder.Configuration.GetSection("Jwt");
            builder.Services.Configure<JwtSettings>(jwtSection);
            var jwt = jwtSection.Get<JwtSettings>();

            if (string.IsNullOrEmpty(jwt?.Key))
                throw new InvalidOperationException("JWT Key is missing in appsettings.json");

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwt.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwt.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1),
                        RoleClaimType = ClaimTypes.Role,        // <- important
                        NameClaimType = ClaimTypes.NameIdentifier
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                   options.AddPolicy("OfficerOrHigher",
                   p => p.RequireRole("Officer", "Investigator", "Admin"));

                  options.AddPolicy("InvestigatorOrAbove",
                  p => p.RequireRole("Investigator", "Admin"));


                // Clearance (string-based comparisons)
                options.AddPolicy("ClearanceMediumOrAbove", p => p.RequireAssertion(ctx =>
                {
                    var c = ctx.User.FindFirst("ClearanceLevel")?.Value?.ToLowerInvariant();
                    return c is "medium" or "high" or "critical";
                }));

                options.AddPolicy("ClearanceHighOrAbove", p => p.RequireAssertion(ctx =>
                {
                    var c = ctx.User.FindFirst("ClearanceLevel")?.Value?.ToLowerInvariant();
                    return c is "high" or "critical";
                }));

                options.AddPolicy("ClearanceCriticalOnly", p => p.RequireClaim("ClearanceLevel", "Critical", "critical"));
            });

            // ---------- SWAGGER ----------
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "Crime Management API", Version = "v1" });

                var scheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer {token}'",
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                };
                c.AddSecurityDefinition("Bearer", scheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement { { scheme, Array.Empty<string>() } });
            });

            builder.Services.AddControllers();

            var app = builder.Build();

            // ---------- SEED DATA ----------
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<CrimeDbContext>();
                SeedData.seed(db);
            }

            // ---------- MIDDLEWARE ----------
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

           // app.UseMiddleware<JwtMiddleware>(); // fixed

            app.MapControllers();

            app.Run();
        }
        
    }
}
