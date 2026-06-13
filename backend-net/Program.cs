using backend_net.Data;
using Microsoft.EntityFrameworkCore;
using backend_net.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("db-ai_bot")));

builder.Services.AddScoped<AnalisaService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- TAMBAHKAN BLOK INI ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Memastikan folder Database terbentuk dan tabel di-migrate otomatis
    db.Database.EnsureCreated(); // Gunakan ini jika tidak pakai Migrations
    // db.Database.Migrate();    // ATAU Gunakan ini jika Anda pakai dotnet ef migrations
}
// --------------------------

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
