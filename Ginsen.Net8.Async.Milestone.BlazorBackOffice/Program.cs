using Ginsen.Net8.Async.Milestone.Api.Client;
using Ginsen.Net8.Async.Milestone.BlazorBackOffice.Client.Pages;
using Ginsen.Net8.Async.Milestone.BlazorBackOffice.Components;
using Ginsen.Net8.Async.Milestone.BlazorBackOffice.Hubs;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddSignalR();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});

// Add the generated HttpClientFactory
builder.Services.AddApiHttpClient(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:5001");

var app = builder.Build();

if (!(app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker")))
{
    app.UseResponseCompression();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Ginsen.Net8.Async.Milestone.BlazorBackOffice.Client._Imports).Assembly);

app.MapHub<ChatHub>("/chathub");

app.Run();
