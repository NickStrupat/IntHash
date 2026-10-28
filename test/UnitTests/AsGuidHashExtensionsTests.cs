using NickStrupat;
using Shouldly;

namespace UnitTests;

public class AsGuidHashExtensionsTests(ITestOutputHelper toh)
{
	[Fact]
	public void AsGuid_UInt32()
	{
		const UInt32 u = 0u;
		var guid = u.AsGuidHash();
		guid.Version.ShouldBe(8);
		Assert.Equal(u, guid.TryFromGuidHash(out UInt32 decoded) ? decoded : throw new());
		//Assert.Equal("d2c3f4e5-6a7b-8c9d-0e1f-2a3b4c5d6e7f", guid.ToString("D"));
	}

	[Fact]
	public void AsGuid_UInt64()
	{
		const UInt64 @ulong = 12_345_678_901_234_567_890ul;
		var guid = @ulong.AsGuidHash();
		Assert.Equal(@ulong, guid.TryFromGuidHash(out UInt64 decoded) ? decoded : throw new());
		// Assert.Equal("1a2b3c4d-5e6f-7a8b-9c0d-e1f2a3b4c5d6", guid.ToString("D"));
	}

	[Fact]
	public void AsGuid_Int32()
	{
		const Int32 i = -1_234_567_890;
		var guid = i.AsGuidHash();
		Assert.Equal(i, guid.TryFromGuidHash(out Int32 decoded) ? decoded : throw new());
		//Assert.Equal("fedcba98-7654-3210-fedc-ba9876543210", guid.ToString("D"));
	}

	[Fact]
	public void AsGuid_Int64()
	{
		const Int64 l = -1_234_567_890_123_456_789L;
		var guid = l.AsGuidHash();
		Assert.Equal(l, guid.TryFromGuidHash(out Int64 decoded) ? decoded : throw new());
		// Assert.Equal("09876543-21fe-dcba-9876-543210fedcba", guid.ToString("D"));
	}

	[Theory]
	[MemberData(nameof(GuidVersionTestData))]
	public void Version(Guid guid, Byte version)
	{
		Assert.Equal(version, guid.Version());
	}

	public static Object[][] GuidVersionTestData = [
		[Guid.NewGuid(), 4],
		[Guid.CreateVersion7(), 7],
		[1234.AsGuidHash(), 8],
		[12345u.AsGuidHash(), 8],
		[123456L.AsGuidHash(), 8],
		[1234567uL.AsGuidHash(), 8]
	]; 
	
	public static TheoryData<Guid> GuidHashes => [
		0u.AsGuidHash(),
		UInt32.MaxValue.AsGuidHash(),
		(-1).AsGuidHash(),
		0ul.AsGuidHash(),
		UInt64.MaxValue.AsGuidHash(),
		(-1L).AsGuidHash()
	];

	[Theory]
	[MemberData(nameof(GuidHashes))]
	public void AsGuidHash_WhenCalled_SetsVersion8AndRfc9562Variant(Guid guid)
	{
		Span<Byte> bytes = stackalloc Byte[16];
		guid.TryWriteBytes(bytes);
		Assert.Equal(8, guid.Version());
		Assert.Equal(0b_10, bytes[8] >> 6);
		Assert.Contains(guid.ToString("D")[19], "89ab");
	}

	[Theory]
	[InlineData(0u)]
	[InlineData(1u)]
	[InlineData(UInt32.MaxValue - 2)]
	[InlineData(UInt32.MaxValue)]
	public void TryFromGuidHashUInt32_WhenCalledWithAGuidHash_ReturnsTheOriginalValue(UInt32 x)
	{
		Assert.True(x.AsGuidHash().TryFromGuidHash(out UInt32 decoded));
		Assert.Equal(x, decoded);
	}

	[Theory]
	[InlineData(0ul)]
	[InlineData(1ul)]
	[InlineData(0xFFFF_FFFFul)]
	[InlineData(0x1_0000_0000ul)]
	[InlineData(UInt64.MaxValue)]
	public void TryFromGuidHashUInt64_WhenCalledWithAGuidHash_ReturnsTheOriginalValue(UInt64 x)
	{
		Assert.True(x.AsGuidHash().TryFromGuidHash(out UInt64 decoded));
		Assert.Equal(x, decoded);
	}

	[Theory]
	[InlineData(0ul)]
	[InlineData(42ul)]
	[InlineData(UInt64.MaxValue)]
	public void TryFromGuidHashUInt32_WhenCalledWithAUInt64GuidHash_ReturnsFalse(UInt64 x)
	{
		Assert.False(x.AsGuidHash().TryFromGuidHash(out UInt32 _));
		Assert.False(x.AsGuidHash().TryFromGuidHash(out Int32 _));
	}

	[Theory]
	[InlineData(0u)]
	[InlineData(42u)]
	[InlineData(UInt32.MaxValue)]
	public void TryFromGuidHashUInt64_WhenCalledWithAUInt32GuidHash_ReturnsFalse(UInt32 x)
	{
		Assert.False(x.AsGuidHash().TryFromGuidHash(out UInt64 _));
		Assert.False(x.AsGuidHash().TryFromGuidHash(out Int64 _));
	}

	[Theory]
	[InlineData("00000000-0000-8000-8000-000000000000")]
	[InlineData("ffffffff-ffff-8fff-bfff-ffffffffffff")]
	[InlineData("12345678-9abc-8def-9123-456789abcdef")]
	public void TryFromGuidHash_WhenCalledWithAForeignV8Guid_ReturnsFalse(String guidString)
	{
		var guid = Guid.Parse(guidString);
		Assert.False(guid.TryFromGuidHash(out UInt32 _));
		Assert.False(guid.TryFromGuidHash(out UInt64 _));
	}

	[Fact]
	public void TryFromGuidHash_WhenCalledWithAGuidHashWithOneBitFlipped_ReturnsFalse()
	{
		Span<Byte> bytes = stackalloc Byte[16];
		42u.AsGuidHash().TryWriteBytes(bytes);
		bytes[0] ^= 1;
		Assert.False(new Guid(bytes).TryFromGuidHash(out UInt32 _));

		42ul.AsGuidHash().TryWriteBytes(bytes);
		bytes[5] ^= 1;
		Assert.False(new Guid(bytes).TryFromGuidHash(out UInt64 _));
	}

	[Fact]
	public void TryFromGuidHash_WhenCalledWithStandardGuids_ReturnsFalse()
	{
		Assert.False(Guid.NewGuid().TryFromGuidHash(out UInt32 _));
		Assert.False(Guid.CreateVersion7().TryFromGuidHash(out UInt64 _));
		Assert.False(Guid.Empty.TryFromGuidHash(out UInt32 _));
	}

	// Pins the encoding format; a change here breaks decoding of GUIDs that are already stored
	[Theory]
	[InlineData(0u, "e33de521-5f95-83a1-87b5-f0c0e9e8dff1")]
	[InlineData(12345u, "08f310f2-52c5-8622-99f2-c08ce7c66ad4")]
	public void AsGuidHashUInt32_WhenCalled_ReturnsTheStableEncoding(UInt32 x, String expected)
	{
		Assert.Equal(expected, x.AsGuidHash().ToString("D"));
	}

	// Pins the encoding format; a change here breaks decoding of GUIDs that are already stored
	[Theory]
	[InlineData(0ul, "bafb8363-828b-8554-8c37-a4f58a6b244b")]
	[InlineData(12_345_678_901_234_567_890ul, "40947a23-7579-8ec4-9716-40a5a503cd23")]
	public void AsGuidHashUInt64_WhenCalled_ReturnsTheStableEncoding(UInt64 x, String expected)
	{
		Assert.Equal(expected, x.AsGuidHash().ToString("D"));
	}

	void PrintNibbles(ReadOnlySpan<Byte> bytes)
	{
		toh.WriteLine("0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9"); 
		foreach (var b in bytes)
		{
			var firstNibble = (b >> 4) & 0x0F;
			var secondNibble = b & 0x0F;
			toh.Write($"{firstNibble:X} {secondNibble:X} ");
		}
		toh.WriteLine("");
	}
}