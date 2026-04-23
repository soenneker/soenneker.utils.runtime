using Soenneker.Tests.Attributes.Local;
using System.Threading.Tasks;


namespace Soenneker.Utils.Runtime.Tests;

public class RuntimeUtilTests
{
    [Test]
    public void Default()
    {
    }

    [LocalOnly]
    public void IsWindows()
    {
        bool result = RuntimeUtil.IsWindows();
    }

    [LocalOnly]
    public async ValueTask IsContainer()
    {
        bool result = await RuntimeUtil.IsContainer(System.Threading.CancellationToken.None);
    }
}
