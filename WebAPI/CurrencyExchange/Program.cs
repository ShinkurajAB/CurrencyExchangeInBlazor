using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using CurrencyExchange.API.AppServices;
using CurrencyExchange.APIService.Interface;
using CurrencyExchange.APIService;
using CurrencyExchange.DataBaseContext.Dapper;
using CurrencyExchange.Repository.Interface;
using CurrencyExchange.Repository;

var builder = WebApplication.CreateBuilder(args);

LogService.AddLogger(builder);

builder.Environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory());

builder.Services.AddCors();

builder.Services.AddControllers(x => x.AllowEmptyInputInBodyModelBinding = true);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//swagger
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Currency Exchange", Version = "v1" });
   
});

builder.Services.AddScoped<IDapperContext, DapperContext>();
builder.Services.AddScoped<ICurrencyTableRepository, CurrencyTableRepository>();
builder.Services.AddScoped<ICurrencyExchangeService, CurrencyExchangeService>();

builder.Services.AddScoped<ICurrencyExchangeRatesRepository, CurrencyExchangeRatesRepository>();


var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder =>
{
    builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
});

app.UseRouting();

app.UseHttpsRedirection();

app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
});


app.Run();
