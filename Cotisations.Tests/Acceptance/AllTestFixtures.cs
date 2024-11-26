using Diverse;

namespace Cotisations.Tests.Acceptance;

[SetUpFixture]
public class AllTestFixtures
{
    [OneTimeSetUp]
    public void Init()
    {
        Fuzzer.Log = TestContext.WriteLine;
    }
}