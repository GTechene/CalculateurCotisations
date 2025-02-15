using System.Runtime.CompilerServices;

namespace Cotisations.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyPlaywright.Initialize(installPlaywright: true);
        VerifierSettings.InitializePlugins();
        VerifierSettings.RegisterStreamComparer("png", FakeImageComparison);
    }

    private static Task<CompareResult> FakeImageComparison(Stream received, Stream verified, IReadOnlyDictionary<string, object> context)
    {
        return Task.FromResult(CompareResult.Equal);
    }
}