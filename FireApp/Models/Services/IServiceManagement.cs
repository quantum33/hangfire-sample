namespace FireApp.Models.Services;
using System;

public interface IServiceManagement
{
    void SendEmail();
    void UpdateDatabase();
    
    void SyncRecords();
}

public class ServiceManagement : IServiceManagement
{
    public void SendEmail()
    {
        Console.WriteLine("send email...");
    }

    public void UpdateDatabase()
    {
        Console.WriteLine("update database...");
    }
    public void SyncRecords()
    {
        Console.WriteLine($"Sync Records: at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
    }
}