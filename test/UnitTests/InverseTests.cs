using NickStrupat;

namespace UnitTests;

public class InverseTests(ITestOutputHelper toh)
{
	[Fact]
	public void HashInverseUInt32_WhenCalledWithTheResultOfCallingHash_ReturnsTheOriginalValue()
	{
		const UInt32 tenPercent = UInt32.MaxValue / 10;
		for (var i = 0ul; i <= UInt32.MaxValue; i++)
		{
			TestContext.Current.CancellationToken.ThrowIfCancellationRequested();
			
			var (quotient, remainder) = Math.DivRem(i, tenPercent);
			if (remainder == 0)
				toh.WriteLine($"{quotient * 10}% - {i:N0}");
			
			var input = (UInt32)i;
			var hash = IntHash.Hash(input);
			if (input == hash) // Assert class methods are too slow
				Assert.Fail("Input and hash result are equal: " + input + " => " + hash);
			var inverse = IntHash.HashInverse(hash);
			if (input != inverse) // Assert class methods are too slow
				Assert.Fail("Input and inverse result are not equal: " + input + " => " + hash + " => " + inverse);
		}
	}
	
	[Fact]
	public void HashInverseUInt64_WhenCalledWithTheResultOfCallingHash_ReturnsTheOriginalValue()
	{
		const UInt64 tenPercent = UInt32.MaxValue / 10;
		for (var i = 0ul; i <= UInt32.MaxValue; i++)
		{
			TestContext.Current.CancellationToken.ThrowIfCancellationRequested();
			
			var (quotient, remainder) = Math.DivRem(i, tenPercent);
			if (remainder == 0)
				toh.WriteLine($"{quotient * 10}% - {i:N0}");
			
			var input = i * i;
			var hash = IntHash.Hash(input);
			if (input == hash) // Assert class methods are too slow
				Assert.Fail("Input and hash result are equal: " + input + " => " + hash);
			var inverse = IntHash.HashInverse(hash);
			if (input != inverse) // Assert class methods are too slow
				Assert.Fail("Input and inverse result are not equal: " + input + " => " + hash + " => " + inverse);
		}
	}
}