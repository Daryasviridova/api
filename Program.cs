using api.infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ���������� ����� � ���������
builder.Services.AddDbContext<CurrencyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("pgsql")));

// ���������� HTTP ���������� ������
builder.Services.AddHttpClient();

// ���������� �������� ������������
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

//���� ������ �������
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CurrencyDbContext>();
    dbContext.Database.Migrate();
}

// ������������ HTTP request pipeline
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
