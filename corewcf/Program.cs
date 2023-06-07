using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

IWebHostBuilder builder = WebHost.CreateDefaultBuilder(args)
    .UseKestrel(options =>
    {
        options.ListenAnyIP(8000);
    })
    .UseStartup<Startup>();

IWebHost app = builder.Build();
app.Run();
