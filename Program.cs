var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Remova ou comente para não forçar HTTPS
// app.UseHttpsRedirection();
app.MapGet("/", () => "API rodando! Acesse /api/sensors");

app.MapControllers();

app.Run();

