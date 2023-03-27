using Dapr;
using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);
var daprClient = new DaprClientBuilder().Build();
var app = builder.Build();

// Dapr configurations
app.UseCloudEvents();

app.MapSubscribeHandler();

app.MapPost("/A", [Topic("pubsub", "A")] (ILogger<Program> logger, MessageEvent item) => {
    Console.WriteLine($"{item.MessageType}: {item.Message}");
    return Results.Ok();
});

app.MapPost("/B", [Topic("pubsub", "B")] (ILogger<Program> logger, MessageEvent item) => {
    Console.WriteLine($"{item.MessageType}: {item.Message}");
    return Results.Ok();
});

app.MapPost("/C", [Topic("pubsub", "C")] (ILogger<Program> logger, Dictionary<string, string> item) => {
    Console.WriteLine($"{item["messageType"]}: {item["message"]}");
    return Results.Ok();
});

app.MapPost("/compress", [Topic("pubsub", "compress")] async (ILogger<Program> logger, Dictionary<string, string> item) => {
    Console.WriteLine($"{item["messageType"]}: {item["message"]}");
    Console.WriteLine(item["message"]);
    Console.WriteLine(daprClient.InvokeMethodAsync<string>("node-subscriber", "compress", item["message"]).ToString());
    // Console.WriteLine("mensaje comprimido:", result);
    return Results.Ok();
});

app.Run();

internal record MessageEvent(string MessageType, string Message);