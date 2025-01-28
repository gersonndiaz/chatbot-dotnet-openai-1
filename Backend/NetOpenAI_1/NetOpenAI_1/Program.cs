using NetOpenAI_1.Hubs.OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Carga el archivo base y el específico del entorno
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSignalR(hubOptions =>
{
    // 20 MB en bytes
    hubOptions.MaximumReceiveMessageSize = 20 * 1024 * 1024;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200", "http://localhost")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

app.UseRouting();

app.MapControllers();

app.MapHub<ChatHub>("/hubs/chat");

app.Run();
