using System.Runtime.CompilerServices;

namespace Cotisations.Tests.Acceptance;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyPlaywright.Initialize(installPlaywright: true);
        VerifierSettings.InitializePlugins();
    }
}