using Hangfire;
using Hangfire.MySql;
using HangfireDemo.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<TestJob>(); // add it just in case
builder.Services.AddHangfire((sp, config) =>
{
    var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("HangfireMySQL");
    var mysqlStorage = new MySqlStorage(connectionString, new MySqlStorageOptions());
    config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseStorage(mysqlStorage);
});
builder.Services.AddHangfireServer();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.UseHangfireDashboard();

app.Run();
