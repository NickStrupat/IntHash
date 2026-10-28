using NickStrupat;

namespace UnitTests;

public class RoundTripTests
{
	public static TheoryData<Int32> Int32Values => [0, 1, -1, 12_345, -1_234_567_890, Int32.MinValue, Int32.MaxValue];
	public static TheoryData<Int64> Int64Values => [0L, 1L, -1L, 12_345L, -1_234_567_890_123_456_789L, Int64.MinValue, Int64.MaxValue];
	public static TheoryData<UInt32> UInt32Seeds => [0u, 1u, 0xDEADBEEFu, UInt32.MaxValue];
	public static TheoryData<UInt64> UInt64Seeds => [0ul, 1ul, 0xDEADBEEF_CAFEBABEul, UInt64.MaxValue];

	[Theory]
	[MemberData(nameof(Int32Values))]
	public void HashInverseInt32_WhenCalledWithTheResultOfCallingHash_ReturnsTheOriginalValue(Int32 x)
	{
		Assert.Equal(x, IntHash.HashInverse(IntHash.Hash(x)));
		Assert.Equal(x, IntHash.Hash(IntHash.HashInverse(x)));
	}

	[Theory]
	[MemberData(nameof(Int64Values))]
	public void HashInverseInt64_WhenCalledWithTheResultOfCallingHash_ReturnsTheOriginalValue(Int64 x)
	{
		Assert.Equal(x, IntHash.HashInverse(IntHash.Hash(x)));
		Assert.Equal(x, IntHash.Hash(IntHash.HashInverse(x)));
	}

	[Theory]
	[MemberData(nameof(UInt32Seeds))]
	public void Int32Hasher_WhenHashInverseCalledWithTheResultOfCallingHash_ReturnsTheOriginalValue(UInt32 seed)
	{
		var hasher = new Int32Hasher(seed);
		foreach (var x in new[] { 0u, 1u, 42u, 0x8000_0000u, UInt32.MaxValue })
		{
			Assert.Equal(x, hasher.HashInverse(hasher.Hash(x)));
			Assert.Equal(x, hasher.Hash(hasher.HashInverse(x)));
		}
		foreach (var x in new[] { 0, 1, -1, 42, Int32.MinValue, Int32.MaxValue })
		{
			Assert.Equal(x, hasher.HashInverse(hasher.Hash(x)));
			Assert.Equal(x, hasher.Hash(hasher.HashInverse(x)));
		}
	}

	[Theory]
	[MemberData(nameof(UInt64Seeds))]
	public void Int64Hasher_WhenHashInverseCalledWithTheResultOfCallingHash_ReturnsTheOriginalValue(UInt64 seed)
	{
		var hasher = new Int64Hasher(seed);
		foreach (var x in new[] { 0ul, 1ul, 42ul, 0x8000_0000_0000_0000ul, UInt64.MaxValue })
		{
			Assert.Equal(x, hasher.HashInverse(hasher.Hash(x)));
			Assert.Equal(x, hasher.Hash(hasher.HashInverse(x)));
		}
		foreach (var x in new[] { 0L, 1L, -1L, 42L, Int64.MinValue, Int64.MaxValue })
		{
			Assert.Equal(x, hasher.HashInverse(hasher.Hash(x)));
			Assert.Equal(x, hasher.Hash(hasher.HashInverse(x)));
		}
	}

	[Fact]
	public void Hasher_WhenSeedIsNonZero_ProducesDifferentHashThanUnseeded()
	{
		Assert.NotEqual(IntHash.Hash(42u), new Int32Hasher(0xDEADBEEFu).Hash(42u));
		Assert.NotEqual(IntHash.Hash(42ul), new Int64Hasher(0xDEADBEEF_CAFEBABEul).Hash(42ul));
	}
}
