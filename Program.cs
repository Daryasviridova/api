using api.infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Добавление услуг в контейнер
builder.Services.AddDbContext<CurrencyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("pgsql")));

// Добавление HTTP клиентской службы
builder.Services.AddHttpClient();

// Добавление сервисов контроллеров
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

//база данных создана
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CurrencyDbContext>();
    dbContext.Database.Migrate();
}

// Конфигурация HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();
app.MapControllers();
app.Run();
