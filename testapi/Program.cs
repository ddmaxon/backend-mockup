using testapi;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Docs
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
});

var app = builder.Build();

// enable Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
});

// Define your API routes
app.MapGet("/", () => "Hello World!");

CsvLoader loader = new CsvLoader("./csv/testdata.csv");

app.MapGet("/csv/count", () =>
{
    return new { status = 200, data = loader.GetCsvLines().Count() };
});
app.MapGet("/csv/search/index/{index}", (int index) =>
{
    try
    {
        return new { status = 200, data = loader.GetCsvLine(index) };
    }
        catch (Exception err)
    {
        return new { status = 500, data = err.Message };
    }
});
app.MapGet("/csv/search/substring/{substring}", (string substring) =>
{
    try
    {
        var res = loader.searchSubstringInCsv(substring);
        return new { status = 200, data = new { substring, value = res }  };
    }
    catch (Exception err)
    {
        var res = new List<string>();

        res.Add(err.Message);

        return new { status = 500, data = new { substring, value = res } };
    }
});
app.MapGet("/csv/search/datetime/{startDate}/{endDate}", (string startDate, string endDate) =>
{
    return HandleErrors(() =>
    {
        object res = loader.getDataBetween(startDate, endDate);

        return new { status = 200, data = new { startDate, value = res } };
    });
});

app.Run();


// Handle errors for API Routes
object HandleErrors(Func<object> method)
{
    try
    {
        return method(); // Führe die übergebene Aktion aus und gib das Ergebnis zurück
    }
    catch (Exception err)
    {
        return new { status = 500, data = err.Message }; // Gib die Ausnahme im gewünschten Format zurück
    }
}
