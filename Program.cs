using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

const string customActivitySourceName = "MySource";
var activitySource = new ActivitySource(customActivitySourceName);
builder.Services.AddOpenTelemetryTracing(x =>
{
    x.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MyService"));
    x.AddAspNetCoreInstrumentation();
    x.AddSource(customActivitySourceName);
    x.AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://collector:4317");
    });
});

const string customMeterName = "MyMeter";
var meter = new Meter(customMeterName);
var counter = meter.CreateCounter<int>("my-counter");
builder.Services.AddOpenTelemetryMetrics(x =>
{
    x.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MyService"));
    x.AddAspNetCoreInstrumentation();
    x.AddMeter(customMeterName);
    x.AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://collector:4317");
    });
});

builder.Logging.AddOpenTelemetry(x =>
{
    x.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MyService"));
    x.IncludeFormattedMessage = true;
    x.IncludeScopes = true;
    x.ParseStateValues = true;
    x.AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://collector:4317");
    });
});

var app = builder.Build();

app.MapGet("/", (ILogger<Program> logger) =>
{
    logger.LogInformation("Request");
    using (var activity = activitySource.StartActivity())
    {
        activity?.AddTag("custom.tag", "hello-world");
        counter.Add(1);
        return "Hello World!";
    }
});

app.Run();
