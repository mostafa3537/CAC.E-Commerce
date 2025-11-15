using Microsoft.EntityFrameworkCore;
using FluentValidation;
using CAC.Infrastrucure;

namespace CAC.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Register MediatR
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CAC.Application.Features.Categories.Commands.CreateCategory.CreateCategoryCommand).Assembly);
            cfg.AddOpenBehavior(typeof(CAC.Application.Common.Behaviors.ValidationBehavior<,>));
        });

        // Register FluentValidation
        builder.Services.AddValidatorsFromAssembly(typeof(CAC.Application.Features.Categories.Commands.CreateCategory.CreateCategoryCommandValidator).Assembly);

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
