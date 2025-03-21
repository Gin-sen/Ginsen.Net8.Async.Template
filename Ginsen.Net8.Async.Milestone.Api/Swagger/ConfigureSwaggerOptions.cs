namespace Ginsen.Net8.Async.Milestone.Api.Swagger;

using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;

/// <summary>
/// Configures the Swagger generation options.
/// </summary>
/// <remarks>This allows API versioning to define a Swagger document per API version after the
/// <see cref="IApiVersionDescriptionProvider"/> service has been resolved from the service container.</remarks>
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
    /// </summary>
    /// <param name="provider">The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents.</param>
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => this.provider = provider;

    /// <inheritdoc />
    public void Configure(SwaggerGenOptions options)
    {
        // add a swagger document for each discovered API version
        // note: you might choose to skip or document deprecated API versions differently
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }

        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        var baseName = assemblyName?.Substring(0, assemblyName.Length - ".Api".Length) ?? "Ginsen.Net8.Async.Milestone";

        foreach (var subName in new[]{
                ".Contracts.Http",
                // ".Contracts.Messaging"
                })
        {
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{baseName}{subName}.xml"));
        }
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var text = new StringBuilder("An example application with OpenAPI, Swashbuckle, API versioning.");
        var info = new OpenApiInfo()
        {
            Title = "Example Async API",
            Version = description.ApiVersion.ToString(),
            Contact = new OpenApiContact() { Name = "Maxime Places", Email = "gin-sen@github.com" },
            License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
        };

        if (description.IsDeprecated)
        {
            text.Append(" This API version has been deprecated.");
        }

        text.Append("<h4>Additional Information</h4>");
        info.Description = text.ToString();

        return info;
    }
}