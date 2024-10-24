// See https://aka.ms/new-console-template for more information

using System.Net.Http.Json;
using Observability.Homework.Models;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var serviceName = "Observability.Homework.Tracing.Client";

using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource(serviceName)
    .AddHttpClientInstrumentation()
    .SetResourceBuilder(
        ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName))
    .AddJaegerExporter()
    .Build();
Random random = new Random();
while (true)
{
    using var httpClient = new HttpClient();
    var response = await httpClient.PostAsync("http://localhost:5216/order/", JsonContent.Create(Order.Create((ProductType)random.Next(0,2))));
    response.EnsureSuccessStatusCode();
    Console.WriteLine("Response received successfully.");
}

