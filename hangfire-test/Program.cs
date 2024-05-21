using Hangfire;
using Hangfire.MySql;
using HangfireTest.SignalR;
using HangfireTest.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<TestJob>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder
                .WithOrigins("http://localhost:5173")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

builder.Services.AddHangfire((sp, config) =>
{
    // var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("HangfireMySQL");
    // var mysqlStorage = new MySqlStorage(connectionString, new MySqlStorageOptions());
    config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseInMemoryStorage();
        // .UseStorage(mysqlStorage);
});
builder.Services.AddHangfireServer();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.UseHangfireDashboard();
app.UseCors();
app.MapHub<JobHub>("/jobHub");

app.Run();
