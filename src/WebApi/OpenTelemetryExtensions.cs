using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace WebApi;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddObservability(
        this IServiceCollection services,
        string serviceName, string serviceVersion,
        IConfiguration configuration)
    {
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion);

        services.AddOpenTelemetry()
            .WithTracing(tracing =>
                {
                    tracing
                        .AddSource(serviceName)
                        .SetResourceBuilder(resourceBuilder)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddGrpcClientInstrumentation()
                        .AddJaegerExporter(o => {
                            o.AgentHost = configuration["OpenTelemetry:Jaeger:Host"];
                            o.AgentPort = int.TryParse(configuration["OpenTelemetry:Jaeger:Port"], out int jaegerPort) 
                                ? jaegerPort 
                                : 6831;
                        });//.AddConsoleExporter();
                });

        return services;        
    }
}
