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






CsvLoader loader = new CsvLoader("./csv/testdata.csv");
Helper helper = new Helper();




// Define your API routes
app.MapGet("/", () => {
    return HandleErrors(() => 
    {
        return helper.SplitList(loader.GetCsvLine(83572));
    });
});

app.MapGet("/csv/count", () =>
{
    return new { status = 200, data = loader.GetCsvLines().Count() };
});
app.MapGet("/csv/search/index/{index}", (int index) => 
{
    return HandleErrors(() =>
    {
        return new { status = 200, data = loader.GetCsvLine(index) };
    });
});
app.MapGet("/csv/search/substring/{substring}", (string substring) => 
{
    return HandleErrors(() =>
    {
        var res = loader.searchSubstringInCsv(substring);
        return new { status = 200, data = new { substring, value = res }  };
    });
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
        return method(); // F�hre die �bergebene Aktion aus und gib das Ergebnis zur�ck
    }
    catch (Exception err)
    {
        return new { status = 500, data = err.Message }; // Gib die Ausnahme im gew�nschten Format zur�ck
    }
}
