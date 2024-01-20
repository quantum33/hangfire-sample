using System.Net;
using Hangfire;
using Hangfire.Server;
using Microsoft.AspNetCore.Mvc;

namespace FireApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContinueController(
    ILogger<ContinueController> logger,
    IHttpClientFactory factory
    ) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        BackgroundJob.Enqueue(() => FireAndForgetJob(null));

        return Ok();
    }

    public async Task<HttpStatusCode> FireAndForgetJob(PerformContext context)
    {
        HttpClient http = factory.CreateClient();
        var jobId = context.BackgroundJob.Id;
        logger.LogInformation($"Executing Job Id: {jobId}...");

        logger.LogInformation("will call web api within 2 seconds....");
        await Task.Delay(TimeSpan.FromSeconds(2));
        HttpResponseMessage result = await http.GetAsync("https://www.google.com");
        logger.LogInformation("will call web api within 2 seconds....");
        
        return result.StatusCode;
    }
}