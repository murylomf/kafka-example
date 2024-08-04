using System.Reflection;
using Carter;
using Carter.OpenApi;
using Microsoft.OpenApi.Models;

namespace KafkaExample;

public static class DependencyInjection
{
    private static readonly Assembly Assembly = typeof(Program).Assembly;
    
    public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCarter();
        
        //services.AddHostedService<ExpireAlertJob>();
        services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly));

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Description = "Kafka Example",
                Version = "v1",
                Title = "kafka Example",
            });

            options.DocInclusionPredicate((s, description) =>
                description.ActionDescriptor.EndpointMetadata.OfType<IIncludeOpenApi>().Any());
        });

        return services;
    }
}