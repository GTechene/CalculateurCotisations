using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cotisations.Tests.EndToEnd;

/// <summary>
/// Technique récupérée depuis https://github.com/donbavand/playwright-webapplicationfactory/blob/main/Playwright.App.Tests/Infrastructure/CustomWebApplicationFactory.cs
/// Via l'article https://danieldonbavand.com/2022/06/13/using-playwright-with-the-webapplicationfactory-to-test-a-blazor-application/
/// Via les travaux de Martin Costello qui a ouvert l'issue https://github.com/dotnet/aspnetcore/issues/33846 ; devrait être résolu avec .Net 10 et le UseKestrel() sur la WebApplicationFactory : https://github.com/dotnet/aspnetcore/pull/60635
/// </summary>
public class CotisationsApi : WebApplicationFactory<Program>
{
    private IHost? _host;

    public string ServerAddress
    {
        get
        {
            EnsureServer();
            return ClientOptions.BaseAddress.ToString();
        }
    }

    /// <summary>
    /// Le but est ici de configurer un Host avec Kestrel afin d'avoir la bonne adresse, celle avec le bon port (plutôt que le "localhost" donné par la WebApplicationFactory de base).
    /// Une fois qu'on a démarré ce Host, on récupère l'adresse et on la sette sur le TestServer de la WebApplicationFactory et on est bons.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var testHost = builder.Build();

        builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel());
        _host = builder.Build();
        _host.Start();

        var server = _host.Services.GetRequiredService<IServer>();
        var addresses = server.Features.Get<IServerAddressesFeature>();

        ClientOptions.BaseAddress = addresses!.Addresses
            .Select(x => new Uri(x))
            .Last();

        testHost.Start();
        return testHost;
    }

    protected override void Dispose(bool disposing)
    {
        _host?.Dispose();
    }

    private void EnsureServer()
    {
        if (_host is null)
        {
            // This forces WebApplicationFactory to bootstrap the server
            using var _ = CreateDefaultClient();
        }
    }
}