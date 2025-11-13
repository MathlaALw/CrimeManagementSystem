
using CitizenManagementSystem.Data;
using CitizenManagementSystem.Repos;
using CitizenManagementSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace CitizenManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // DbContext
            builder.Services.AddDbContext<CitizenDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("CitizenDb")));

            // Add services to the container.
            builder.Services.AddScoped<ICitizenRepository, CitizenRepository>();
            builder.Services.AddScoped<ICitizenService, CitizenService>();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

        
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
