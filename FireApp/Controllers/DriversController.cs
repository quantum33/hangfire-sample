using FireApp.Models;
using FireApp.Models.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace FireApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DriversController(ILogger<DriversController> logger) : Controller
{
    private readonly ILogger<DriversController> _logger = logger;
    private static readonly List<Driver> drivers = new();

    [HttpPost]
    public IActionResult Add(Driver driver)
    {
        if (ModelState.IsValid)
        {
            drivers.Add(driver);
            
            // Fire and Forget Job
            var jobId = BackgroundJob.Enqueue<IServiceManagement>(x => x.SendEmail());
            Console.WriteLine($"Job id: {jobId}");
            
            return CreatedAtAction(nameof(Get), new { driver.Id }, driver);
        }
        return BadRequest();
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var items = drivers.Where(x => x.Status == 1).ToList();
        return Ok(items);
    }
    
    [HttpGet("{Id}")]
    public IActionResult Get(string id)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        
        var driver = drivers.FirstOrDefault(x => x.Id == id);
        return driver != null
            ? Ok(driver)
            : NotFound();
    }

    [HttpDelete]
    public IActionResult Delete(string id)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        
        var driver = drivers.FirstOrDefault(x => x.Id == id);

        if (driver == null)
            return NotFound();

        driver.Status = 0;
        return NoContent();
    }
}