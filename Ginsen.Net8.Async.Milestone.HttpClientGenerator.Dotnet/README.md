# Ginsen.Net8.Async.Milestone.HttpClientGenerator

This project is a Roslyn source generator that generates a strongly-typed `HttpClientFactory` for the API using the Contracts DTOs.

## How it works
- Scans the Contracts project for DTOs.
- Generates an extension method to register a named `HttpClient` for the API.

## Usage
1. Reference this project from your API or client project.
2. Call `services.AddApiHttpClient(baseUrl)` in your DI setup.

## Extending
You can extend the generator to emit strongly-typed client methods for each DTO or endpoint as needed.
