using Soenneker.Facts.Local;
using Soenneker.Tests.FixturedUnit;
using Xunit;
using Xunit.Abstractions;

namespace Soenneker.Utils.Runtime.Tests;

[Collection("Collection")]
public class RuntimeUtilTests : FixturedUnitTest
{
    public RuntimeUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    [LocalFact]
    public void IsWindows()
    {
        var result = RuntimeUtil.IsWindows();
    }
}
