using NickStrupat;

namespace UnitTests;

public class PerfectDistributionTests(ITestOutputHelper toh)
{
	[Fact]
	public void Hash_WhenCalledWithAllPossibleValues_ReturnsUniqueValues()
	{
		var _32BitValueCount = UInt32.MaxValue + 1ul;
		var bitStore = new Byte[_32BitValueCount / 8];
		const UInt32 tenPercent = UInt32.MaxValue / 10;
		var cancellationToken = TestContext.Current.CancellationToken;
		for (var i = 0ul; i <= UInt32.MaxValue; i++)
		{
			cancellationToken.ThrowIfCancellationRequested();
			
			var (quotient, remainder) = Math.DivRem(i, tenPercent);
			if (remainder == 0)
				toh.WriteLine($"{quotient * 10}% - {i:N0}");
			
			var input = (UInt32)i;
			var hash = IdHash.Hash(input);
			var byteIndex = (Int32)(hash / 8);
			var bitIndex = (Byte)(hash & 7);
			ref var @byte = ref bitStore[byteIndex];
			var bit = (@byte >> bitIndex) & 1;
			if (bit == 1)
			{
				Assert.Fail($"Collision found at {i} => {hash}");
			}
			else
			{
				@byte |= (Byte)(1 << bitIndex);
			}
		}
		toh.WriteLine("No collision found");
		
		Assert.True(bitStore.All(x => x == 0xff), "Collision found");
	}
}