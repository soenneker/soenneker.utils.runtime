using Soenneker.Facts.Local;
using System.Threading.Tasks;
using Xunit;


namespace Soenneker.Utils.Runtime.Tests;

public class RuntimeUtilTests
{
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
        bool result = await RuntimeUtil.IsContainer(TestContext.Current.CancellationToken);
    }
}