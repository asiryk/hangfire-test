var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// app.MapGet("/", () => 
// {
//     var v = 5;
//     Console.WriteLine(v);
//     return "Hello World!";
// }

// );

app.Run();
