using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Ginsen.Net8.Async.Milestone.HttpClientGenerator
{
    [Generator]
    public class HttpClientFactoryGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for now
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // Find all DTOs in the Contracts namespace
            var contractDtos = context.Compilation.SyntaxTrees
                .SelectMany(tree => tree.GetRoot().DescendantNodes())
                .OfType<RecordDeclarationSyntax>()
                .Where(r => r.Parent is NamespaceDeclarationSyntax ns && ns.Name.ToString().Contains("Contracts"))
                .ToList();

            // Generate a simple HttpClientFactory class
            var sb = new StringBuilder();
            sb.AppendLine("using System.Net.Http;");
            sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
            sb.AppendLine("using System.Net.Http.Json;");
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("namespace Ginsen.Net8.Async.Milestone.Api.Client");
            sb.AppendLine("{");
            sb.AppendLine("    public static class ApiHttpClientFactory");
            sb.AppendLine("    {");
            sb.AppendLine("        public static IHttpClientBuilder AddApiHttpClient(this IServiceCollection services, string baseUrl)");
            sb.AppendLine("        {");
            sb.AppendLine("            return services.AddHttpClient<ApiHttpClient>(c => c.BaseAddress = new System.Uri(baseUrl));");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            // Find all Minimal API endpoints in the API project
            var endpointInfos = new List<(string Name, string HttpMethod, string Route, string? RequestType, string? ResponseType)>();
            foreach (var tree in context.Compilation.SyntaxTrees)
            {
                var root = tree.GetRoot();
                // Look for MapGet/MapPost/MapPut/MapDelete in endpoint files
                var invocations = root.DescendantNodes().OfType<InvocationExpressionSyntax>()
                    .Where(inv => inv.Expression is MemberAccessExpressionSyntax member &&
                        (member.Name.Identifier.Text.StartsWith("MapGet") ||
                         member.Name.Identifier.Text.StartsWith("MapPost") ||
                         member.Name.Identifier.Text.StartsWith("MapPut") ||
                         member.Name.Identifier.Text.StartsWith("MapDelete")));
                foreach (var inv in invocations)
                {
                    var member = (MemberAccessExpressionSyntax)inv.Expression;
                    var httpMethod = member.Name.Identifier.Text.Replace("Map", "");
                    var routeArg = inv.ArgumentList.Arguments.FirstOrDefault();
                    var route = routeArg?.ToString().Trim('"') ?? "";
                    // Try to get the delegate parameter types for request/response
                    string? requestType = null;
                    string? responseType = null;
                    var lambda = inv.ArgumentList.Arguments.Skip(1).FirstOrDefault()?.Expression as ParenthesizedLambdaExpressionSyntax;
                    if (lambda != null)
                    {
                        // Try to find [FromBody] or [FromRoute] parameters for requestType
                        var param = lambda.ParameterList.Parameters.FirstOrDefault(p => p.AttributeLists.Any(a => a.ToString().Contains("FromBody")));
                        requestType = param?.Type?.ToString();
                    }
                    // Try to find .Produces<T> for responseType
                    var produces = inv.Parent?.DescendantNodes().OfType<GenericNameSyntax>()
                        .FirstOrDefault(g => g.Identifier.Text == "Produces");
                    if (produces != null && produces.TypeArgumentList.Arguments.Count > 0)
                        responseType = produces.TypeArgumentList.Arguments[0].ToString();
                    var name = inv.Parent?.DescendantNodes().OfType<InvocationExpressionSyntax>()
                        .SelectMany(i => i.ArgumentList.Arguments)
                        .FirstOrDefault(a => a.ToString().Contains("WithName"))?.ToString().Trim('"');
                    endpointInfos.Add((name ?? route, httpMethod, route, requestType, responseType));
                }
            }

            // Generate a HttpClient wrapper class for the discovered endpoints
            sb.AppendLine("    public class ApiHttpClient");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly HttpClient _httpClient;");
            sb.AppendLine("        public ApiHttpClient(HttpClient httpClient) => _httpClient = httpClient;");

            foreach (var ep in endpointInfos)
            {
                // Only generate for endpoints with a name and response type
                if (string.IsNullOrWhiteSpace(ep.Name) || string.IsNullOrWhiteSpace(ep.ResponseType)) continue;
                var methodName = ep.Name.Replace("\"", "");
                var returnType = $"Task<{ep.ResponseType}?>";
                var route = ep.Route.Replace("\"", "");
                var httpMethod = ep.HttpMethod.ToUpperInvariant();
                var requestParam = ep.RequestType != null ? $"{ep.RequestType} request, " : "";
                var bodyArg = ep.RequestType != null ? ", request" : "";
                sb.AppendLine($"        public async {returnType} {methodName}Async({requestParam}CancellationToken cancellationToken = default)");
                sb.AppendLine("        {");
                sb.AppendLine("            try");
                sb.AppendLine("            {");
                if (httpMethod == "GET")
                {
                    sb.AppendLine($"                var response = await _httpClient.GetAsync(\"{route}\", cancellationToken);");
                }
                else if (httpMethod == "POST")
                {
                    sb.AppendLine($"                var response = await _httpClient.PostAsJsonAsync(\"{route}\"{bodyArg}, cancellationToken);");
                }
                else
                {
                    sb.AppendLine($"                // TODO: Implement {httpMethod} method");
                    sb.AppendLine($"                var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.{httpMethod}, \"{route}\"), cancellationToken);");
                }
                sb.AppendLine("                if (!response.IsSuccessStatusCode)");
                sb.AppendLine("                {");
                sb.AppendLine("                    // Optionally log or throw");
                sb.AppendLine("                    return default;");
                sb.AppendLine("                }");
                sb.AppendLine($"                return await response.Content.ReadFromJsonAsync<{ep.ResponseType}>(cancellationToken: cancellationToken);");
                sb.AppendLine("            }");
                sb.AppendLine("            catch (Exception ex)");
                sb.AppendLine("            {");
                sb.AppendLine("                // Optionally log exception");
                sb.AppendLine("                return default;");
                sb.AppendLine("            }");
                sb.AppendLine("        }");
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            context.AddSource("ApiHttpClient.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }
}
