using NickStrupat;

namespace UnitTests;

public class ZeroTests
{
	[Fact]
	public void HashUInt32_WhenCalledWithZero_DoesNotReturnZero()
	{
		var result = IntHash.Hash(0u);
		Assert.NotEqual(0u, result);
	}

	[Fact]
	public void HashInverseUInt32_WhenCalledWithZero_DoesNotReturnZero()
	{
		var result = IntHash.HashInverse(0u);
		Assert.NotEqual(0u, result);
	}
	
	[Fact]
	public void HashInt32_WhenCalledWithZero_DoesNotReturnZero()
	{
		var result = IntHash.Hash(0);
		Assert.NotEqual(0, result);
	}

	[Fact]
	public void HashInverseInt32_WhenCalledWithZero_DoesNotReturnZero()
	{
		var result = IntHash.HashInverse(0);
		Assert.NotEqual(0, result);
	}
	
	[Fact]
	public void HashUInt64_WhenCalledWithZero_DoesNotReturnZero()
	{
		var result = IntHash.Hash(0ul);
		Assert.NotEqual(0ul, result);
	}
	
	[Fact]
	public void HashInverseUInt64_WhenCalledWithZero_DoesNotReturnZero()
	{
		var result = IntHash.HashInverse(0ul);
		Assert.NotEqual(0ul, result);
	}
	
	[Fact]
	public void HashInt64_WhenCalledWithZero_DoesNotReturnZero()
	{
		var result = IntHash.Hash(0L);
		Assert.NotEqual(0L, result);
	}
	
	[Fact]
	public void HashInverseInt64_WhenCalledWithZero_DoesNotReturnZero()
	{
		var result = IntHash.HashInverse(0L);
		Assert.NotEqual(0L, result);
	}
}