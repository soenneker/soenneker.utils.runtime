using Soenneker.Facts.Local;
using Soenneker.Tests.FixturedUnit;
using System.Threading.Tasks;
using Xunit;


namespace Soenneker.Utils.Runtime.Tests;

[Collection("Collection")]
public class RuntimeUtilTests : FixturedUnitTest
{
    public RuntimeUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    [Fact]
    public void Default()
    { 
    
    }

    [LocalFact]
    public void IsWindows()
    {
        bool result = RuntimeUtil.IsWindows();
    }

    [LocalFact]
    public async ValueTask IsContainer()
    { 
        bool result = await RuntimeUtil.IsContainer(CancellationToken);
    }
}
