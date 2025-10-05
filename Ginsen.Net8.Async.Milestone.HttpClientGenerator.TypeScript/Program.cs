using System.IO;
using Mono.TextTemplating;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Workspaces.MSBuild;
using Microsoft.CodeAnalysis;
using Ginsen.Net8.Async.Milestone.HttpClientGenerator.TypeScript;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class Program
{
    // Maps C# types to TypeScript types
    static string MapType(string csharpType)
    {
        return csharpType switch
        {
            "int" or "long" or "float" or "double" or "decimal" => "number",
            "string" => "string",
            "bool" => "boolean",
            "DateTime" => "string",
            _ when csharpType.EndsWith("?") => MapType(csharpType.TrimEnd('?')) + " | null",
            _ => csharpType // fallback to original type name
        };
    }

    static async Task Main(string[] args)
    {
        GeneratorExecutionContext context = new GeneratorExecutionContext();
        // Generate DTO interfaces
        var dtos = context.Compilation.SyntaxTrees
          .SelectMany(tree => tree.GetRoot().DescendantNodes())
          .OfType<RecordDeclarationSyntax>()
          .Where(r => r.Parent is NamespaceDeclarationSyntax ns && ns.Name.ToString().Contains("Contracts"))
          .ToList();

      // Find Minimal API endpoints
      var endpointInfos = new System.Collections.Generic.List<(string Name, string HttpMethod, string Route, string? RequestType, string? ResponseType)>();
      foreach (var tree in context.Compilation.SyntaxTrees)
      {
          var root = tree.GetRoot();
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
              string? requestType = null;
              string? responseType = null;
              var lambda = inv.ArgumentList.Arguments.Skip(1).FirstOrDefault()?.Expression as ParenthesizedLambdaExpressionSyntax;
              if (lambda != null)
              {
                  var param = lambda.ParameterList.Parameters.FirstOrDefault(p => p.AttributeLists.Any(a => a.ToString().Contains("FromBody")));
                  requestType = param?.Type?.ToString();
              }
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

      // Map to DTOs for T4
      var dtoList = dtos.Select(dto => new RecordInfo
      {
          Name = dto.Identifier.Text,
          Properties = dto.Members.OfType<PropertyDeclarationSyntax>()
              .Select(prop => (prop.Identifier.Text, MapType(prop.Type.ToString())))
              .ToList()
      }).ToList();

      var endpointList = endpointInfos.Select(ep => new EndpointInfo
      {
          Name = ep.Name.Replace("\"", ""),
          HttpMethod = ep.HttpMethod.ToUpperInvariant(),
          Route = ep.Route.Replace("\"", ""),
          RequestType = ep.RequestType,
          ResponseType = ep.ResponseType
      }).ToList();

      var engine = new TemplatingEngine();
      var host = new TemplateGenerator();

      // Add parameters to the host for T4 template

      string templatePath = "ApiClient.tt";
      string templateContent = File.ReadAllText(templatePath);

      string output = await engine.ProcessTemplateAsync(templateContent, host);

      File.WriteAllText("ApiClient.ts", output);
      System.Console.WriteLine("ApiClient.ts generated successfully.");
      Console.ReadLine();
    }
}