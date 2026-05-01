using FluffGameApi.Repositories;
using FluffGameApi.Services;

var builder = WebApplication.CreateBuilder(args);

var mysqlConnectionString = builder.Configuration.GetConnectionString("MySqlConnection");
if (string.IsNullOrWhiteSpace(mysqlConnectionString))
{
    throw new InvalidOperationException("Connection string 'MySqlConnection' is not configured.");
}

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Inyect repositories and services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IScoreRepository, ScoreRepository>();
builder.Services.AddScoped<ScoreService>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
