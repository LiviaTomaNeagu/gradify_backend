using Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
using MyBackedApi.Data;
using MyBackendApi.Repositories;
using MyBackendApi.Services;

var builder = WebApplication.CreateBuilder(args);

AppConfig.Init(builder.Configuration);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = null;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // URL-ul frontend-ului Angular
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Dac? folose?ti cookies sau autentificare
    });
});

// Add services to the container.
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<QuestionService>();
builder.Services.AddScoped<AnswerService>();
builder.Services.AddScoped<AuthService>();

// Add repositories to the container.
builder.Services.AddScoped<BaseRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<QuestionRepository>();
builder.Services.AddScoped<AnswerRepository>();
builder.Services.AddScoped<UserAuthTokenRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular app origin
              .AllowAnyMethod() // Allow all HTTP methods
              .AllowAnyHeader(); // Allow all headers
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseCors("AllowAngularApp");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
