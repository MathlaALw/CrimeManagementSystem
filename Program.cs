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
using System.Text.Json;
using System.Threading.RateLimiting;
using Crime_Management_System.Services;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;



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
            builder.Services.AddScoped<ICrimeReportRepository, CrimeReportRepository>();
            builder.Services.AddScoped<ICaseAssigneeRepository, CaseAssigneeRepository>();
            builder.Services.AddScoped<ICitizenSubscriptionRepo, CitizenSubscriptionRepo>();

            // ---------- SERVICES ----------
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICaseService, CaseService>();
            builder.Services.AddScoped<IReportService, ReportService>();
            builder.Services.AddScoped<IEvidenceService, EvidenceService>();
            builder.Services.AddScoped<IParticipantService, ParticipantService>();
            builder.Services.AddScoped<IPasswordService, PasswordService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<ICrimeReportService, CrimeReportService>();
            builder.Services.AddScoped<ICaseAssigneeService, CaseAssigneeService>();
            builder.Services.AddScoped<CaseCommentService>();
            builder.Services.AddScoped<ICitizenSubscriptionService, CitizenSubscriptionService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();


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


                    // handle forbidden responses for specific policies
                    options.Events = new JwtBearerEvents
                {
                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var payload = JsonSerializer.Serialize(new
                        {
                            message = "You don't have permission"
                        });

                        await context.Response.WriteAsync(payload);
                    },
                        OnChallenge = async context =>
                        {

                            context.HandleResponse();

                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            var result = JsonSerializer.Serialize(new
                            {
                                error = "Unauthorized",
                                message = "you are not allow to do this"
                            });

                            await context.Response.WriteAsync(result);
                        }
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                   options.AddPolicy("OfficerOrHigher",
                   p => p.RequireRole("Officer", "Investigator", "Admin"));

                  options.AddPolicy("InvestigatorOrAbove",
                  p => p.RequireRole("Investigator", "Admin"));

                options.AddPolicy("AdminOnly",
                  p => p.RequireRole("Admin"));


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

            builder.Services.AddControllers().AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });
            // ---------- EMAIL SERVICE ----------

            builder.Services.Configure<EmailSettings>(
            builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            //---Rate Limiting-------
            builder.Services.AddRateLimiter(options =>
            {
                // Global limiter 
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    var userKey = context.User?.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

                    return RateLimitPartition.GetTokenBucketLimiter(userKey, _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit =10,
                        TokensPerPeriod = 10,
                        ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                        AutoReplenishment = true,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
                });
       options.AddPolicy("AdminLimiter", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromSeconds(30),
                            QueueLimit = 0
                        }));

                // 
                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.ContentType = "application/json";
                    await context.HttpContext.Response.WriteAsync(
                        JsonSerializer.Serialize(new
                        {
                            error = "Too Many Requests",
                            message = "You have exceeded the allowed limit"
                        }), token);
                };
            });





            // ------------ API versioning -------------
            builder.Services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0); // v1 as default
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader(); // /api/v{version}/...
            });
            // Versioned API Explorer (so Swagger can generate one doc per version)
            builder.Services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";           // v1, v2, v3
                options.SubstituteApiVersionInUrl = true;     // replace {version:apiVersion} in routes
            });

            // Hook up IHttpContextAccessor for services that need it
            builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();


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
                var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                app.UseSwaggerUI(options =>
                {
                    foreach (var desc in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json",
                                                $"Crime Management System API {desc.GroupName.ToUpperInvariant()}");
                    }
                });
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();


            // app.UseMiddleware<JwtMiddleware>(); // fixed

            app.MapControllers();

            app.Run();
        }
        
    }
}
