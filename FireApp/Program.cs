using System.IO.Abstractions;
using FireApp.Identifiers;
using FireApp.Models.Services;
using Hangfire;
using Hangfire.MemoryStorage;
using HangfireBasicAuthenticationFilter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddControllers();

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

app.UseHangfireDashboard(
    "/hangfire",
    new DashboardOptions
    {
        DashboardTitle = "Hangfire Dashboard",
        Authorization = new[]
        {
            new HangfireCustomBasicAuthenticationFilter
            {
                User = "hello",
                Pass = "world"
            }
        }
    });
app.MapHangfireDashboard();

FileSystem fileSystem = new();
LocalWindowsFileIdentifierStrategy strategy = new();
Console.WriteLine(
    strategy.TryValueFileId(
        file: fileSystem.FileInfo.New("Resources/sample1.txt"),
        out WindowsFileId? id1)
        ? id1!.ValueFileId()
        : "No id found");

Console.WriteLine(
    strategy.TryValueFileId(
        file: fileSystem.FileInfo.New("Resources/subFolder/sample2.txt"),
        out WindowsFileId? id2)
        ? id2!.ValueFileId()
        : "No id found 2");

Console.WriteLine(id2?.ValueFileId() == "6422528123095|908969687");
app.Run();