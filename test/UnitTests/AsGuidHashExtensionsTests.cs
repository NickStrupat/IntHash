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