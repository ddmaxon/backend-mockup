using testapi;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Hosting;
using System.Reflection.PortableExecutable;
using SharpCompress.Crypto;

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
app.MapGet("/", () =>
{
    return HandleErrors(() =>
    {
        return new { status = 200, data = helper.GetExecutionTime(loader.GetCsvLine(83572)) };
    });
});

app.MapGet("/csv/count", () =>
{
    return HandleErrors(() =>
    {
        return new { status = 200, data = loader.GetCsvLines().Count() };
    });
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
        Console.WriteLine(startDate);
        Console.WriteLine(endDate);
        object res = loader.getDataBetween(startDate, endDate);

        return new { status = 200, data = new { startDate, value = res } };
    });
});
app.MapGet("/csv/search/tests", () =>
{
    return HandleErrors(() =>
    {
        object res = loader.getAllTestsData();

        return new { status = 200, data = res };
    });
});
app.MapGet("/csv/avarage/executiontime", () => 
{
    return HandleErrors(() => 
    {
        dynamic res = helper.GetExecutionTime(loader.GetCsvLines());

        return new { status = 200, data = res };
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
        return new { status = ParseErrorCode(err), data = new { Message = err.Message, Data = err.Data } }; // Gib die Ausnahme im gew�nschten Format zur�ck
    }
}

// Parse every Exception into a http error code
int ParseErrorCode(Exception exception)
{
    // Standard-HTTP-Fehlercode
    int errorCode = 500;

    // Hier kannst du verschiedene Arten von Ausnahmen überprüfen und die entsprechenden HTTP-Fehlercodes festlegen
    if (exception is UnauthorizedAccessException)
    {
        errorCode = 401; // Unauthorized
    }
    else if (exception is ArgumentException)
    {
        errorCode = 400; // Bad Request
    }
    else if (exception is FileNotFoundException)
    {
        errorCode = 404; // Not Found
    }
    else if (exception is NotSupportedException)
    {
        errorCode = 405; // Method Not Allowed
    }
    else if (exception is NotImplementedException)
    {
        errorCode = 501; // Not Implemented
    }
    else if (exception is TimeoutException)
    {
        errorCode = 408; // Request Timeout
    }
    else if (exception is DataLengthException)
    {
        errorCode = 404;
    }
    else if (exception is IndexOutOfRangeException)
    {
        errorCode = 400;
    }
    // Weitere Ausnahmen können hier hinzugefügt werden...
    else
    {
        errorCode = 500; // Internal Server Error
    }


    return errorCode;
}
