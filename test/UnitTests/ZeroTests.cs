using NickStrupat;

namespace UnitTests;

public class ZeroTests
{
	[Fact]
	public void Hash_WhenCalledWithZero_DoesNotReturnZero()
	{
		var result = IdHash.Hash(0);
		Assert.NotEqual(0u, result);
	}

	[Fact]
	public void HashInverse_WhenCalledWithZero_DoesNotReturnZero()
	{
		var result = IdHash.HashInverse(0);
		Assert.NotEqual(0u, result);
	}
}