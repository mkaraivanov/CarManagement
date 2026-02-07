using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Backend.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set environment to Testing - Program.cs will use In-Memory database
        builder.UseEnvironment("Testing");

        // Set content root to the backend project directory
        // This ensures paths like 'tessdata' resolve correctly
        var backendPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "..",
            "..",
            "..",
            "..",
            "backend"
        );
        var absoluteBackendPath = Path.GetFullPath(backendPath);

        builder.UseContentRoot(absoluteBackendPath);
    }
}
