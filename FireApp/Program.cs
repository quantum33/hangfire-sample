using System.Runtime.InteropServices;
using FireApp.Models.Services;
using Hangfire;
using Hangfire.MemoryStorage;
using HangfireBasicAuthenticationFilter;

namespace FireApp;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddControllers();

        builder.Services.AddHttpClient();
        
        builder.Services.AddHangfire(configuration => configuration
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseMemoryStorage()
        );

// Add the processing server as IHostedService
        builder.Services.AddHangfireServer();
        builder.Services.AddScoped<IServiceManagement, ServiceManagement>();
// RecurringJob.AddOrUpdate<IServiceManagement>(x =>
//     x.SyncRecords(),
//     cronExpression: "0 * * ? * *");

        var app = builder.Build();

// Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.MapControllers();

        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            DashboardTitle = "Hangfire Dashboard",
        });
        app.MapHangfireDashboard();

        app.Run();
    }
}