using Infrastructure.Config;
using Infrastructure.Middlewares;
using Microsoft.EntityFrameworkCore;
using MyBackedApi;
using MyBackedApi.Data;
using MyBackedApi.Hubs;
using MyBackedApi.Repositories;
using MyBackedApi.Services;
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

builder.Services.AddSwagger();

// Add services to the container.
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<QuestionService>();
builder.Services.AddScoped<AnswerService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<NotesService>();
builder.Services.AddScoped<EventsService>();

// Add repositories to the container.
builder.Services.AddScoped<BaseRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<QuestionRepository>();
builder.Services.AddScoped<AnswerRepository>();
builder.Services.AddScoped<UserAuthTokenRepository>();
builder.Services.AddScoped<ActivationCodeRepository>();
builder.Services.AddScoped<OccupationRepository>();
builder.Services.AddScoped<UserTopicsRepository>();
builder.Services.AddScoped<ChatRepository>();
builder.Services.AddScoped<NotesRepository>();
builder.Services.AddScoped<EventsRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddJwtAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddSignalR();


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

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("AllowAngularApp");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chatHub");

app.Run();
