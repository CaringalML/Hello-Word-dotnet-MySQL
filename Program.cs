using HelloWorldApi.Migrations;
using HelloWorldApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register repository
builder.Services.AddScoped<HelloRepository>();

// Register migration script
builder.Services.AddScoped<MigrationScript>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000", "https://yourdomain.com")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
            
            // Or for development, you can allow any origin
            // builder.AllowAnyOrigin()
            //        .AllowAnyHeader()
            //        .AllowAnyMethod();
        });
});

var app = builder.Build();

// Run database migrations
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var migrationScript = scope.ServiceProvider.GetRequiredService<MigrationScript>();
        migrationScript.MigrateAsync().Wait();
    }
}

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins"); // Apply CORS policy
app.UseAuthorization();
app.MapControllers();

app.Run();