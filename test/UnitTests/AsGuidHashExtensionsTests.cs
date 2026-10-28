using NickStrupat;
using System.Runtime.InteropServices;
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

	[Theory]
	[InlineData(0u)]
	[InlineData(1_000_000u)]
	[InlineData(UInt32.MaxValue - 500)]
	public void AsGuidHashUInt32_WhenCalledWithAdjacentValues_ReturnsGuidsWithNoSharedWords(UInt32 start)
	{
		for (var x = start; x != start + 1_000; x++)
			Assert.Empty(MaskedWords(x.AsGuidHash()).Intersect(MaskedWords((x + 1).AsGuidHash())));
	}

	[Theory]
	[InlineData(0ul)]
	[InlineData(UInt32.MaxValue - 500ul)]
	[InlineData(UInt64.MaxValue - 500)]
	public void AsGuidHashUInt64_WhenCalledWithAdjacentValues_ReturnsGuidsWithNoSharedWords(UInt64 start)
	{
		for (var x = start; x != start + 1_000; x++)
			Assert.Empty(MaskedWords(x.AsGuidHash()).Intersect(MaskedWords((x + 1).AsGuidHash())));
	}

	// Guards against chaining the check words without mixing in x, which makes guid(x) and guid(H(x)) share
	// a shifted run of words
	[Fact]
	public void AsGuidHashUInt32_WhenCalledWithValuesWhoseChainsMeet_ReturnsGuidsSharingAtMostOneWord()
	{
		for (var x = 0u; x != 1_000; x++)
		{
			var y = x + IntHash.Hash(x);
			Assert.True(MaskedWords(x.AsGuidHash()).Intersect(MaskedWords(y.AsGuidHash())).Count() <= 1);
		}
	}

	// A zero payload must not collapse the check words to zero, which made both widths share the all-zero v8 GUID
	[Fact]
	public void AsGuidHash_WhenPayloadIsZero_ReturnsANonDegenerateGuidThatOnlyDecodesAtItsOwnWidth()
	{
		var allZero = Guid.Parse("00000000-0000-8000-8000-000000000000");
		var guid32 = IntHash.HashInverse(0u).AsGuidHash();
		var guid64 = IntHash.HashInverse(0ul).AsGuidHash();

		Assert.NotEqual(allZero, guid32);
		Assert.NotEqual(allZero, guid64);
		Assert.False(guid32.TryFromGuidHash(out UInt64 _));
		Assert.False(guid64.TryFromGuidHash(out UInt32 _));
		Assert.Equal(-2, guid32.TryFromGuidHash(out Int32 decoded32) ? decoded32 : throw new());
		Assert.Equal(IntHash.HashInverse(0ul), guid64.TryFromGuidHash(out UInt64 decoded64) ? decoded64 : throw new());
	}

	// The 32-bit words of a GUID with the version and variant bits cleared, so shifted copies of a word compare equal
	private static UInt32[] MaskedWords(Guid guid)
	{
		const UInt32 versionAndVariantMask = 0x0FFF_FF3F;
		var bytes = new Byte[16];
		guid.TryWriteBytes(bytes);
		return MemoryMarshal.Cast<Byte, UInt32>(bytes).ToArray().Select(w => w & versionAndVariantMask).ToArray();
	}

	// Pins the encoding format; a change here breaks decoding of GUIDs that are already stored
	[Theory]
	[InlineData(0u,"bb92a8f2-56ae-8179-a158-38bde9e8dff1")]
	[InlineData(12345u, "69548a26-1ab8-8cf6-b15e-1749e7c66ad4")]
	public void AsGuidHashUInt32_WhenCalled_ReturnsTheStableEncoding(UInt32 x, String expected)
	{
		Assert.Equal(expected, x.AsGuidHash().ToString("D"));
	}

	// Pins the encoding format; a change here breaks decoding of GUIDs that are already stored
	[Theory]
	[InlineData(0ul, "bafb8363-8e9c-80b5-9cc6-31b28a6b244b")]
	[InlineData(12_345_678_901_234_567_890ul, "40947a23-f850-8e7c-af8b-7f77a503cd23")]
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