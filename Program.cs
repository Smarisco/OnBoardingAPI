using OnboardingAPI.ServiceInterface;
using OnboardingAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//inje��o de depend�ncia
builder.Services.AddScoped<IEstagiarioService, EstagiarioService>();
builder.Services.AddScoped<IEstadoService, EstadoService>();
builder.Services.AddScoped<IAcaoService, AcaoService>();
builder.Services.AddScoped<IInteracaoService, InteracaoService>();
builder.Services.AddScoped<IAnaliseService, AnaliseService>();
builder.Services.AddScoped<IQLearningService, QLearningService>();
builder.Services.AddScoped<IAlgoritmoGeneticoService, AlgoritmoGeneticoService>();
builder.Services.AddScoped<IAnaliseService, AnaliseService>();




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
