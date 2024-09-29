using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Додайте Ocelot до контейнера служб
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot();

var app = builder.Build();

// Використовуємо Ocelot Middleware для маршрутизації
await app.UseOcelot();

app.Run();
