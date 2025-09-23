using System;
using System.Runtime.InteropServices;

namespace NickStrupat;

public static class AsGuidHashExtensions
{
	// https://en.wikipedia.org/w/index.php?title=Universally_unique_identifier&oldid=1298251820#Version_8_(custom)
	private const Byte guidVersion = 8;
	
	public static Guid AsGuidHash(this UInt32 x)
	{
		Span<UInt32> uint32s = stackalloc UInt32[]
		{
			IntHash.Hash(x + 3),
			IntHash.Hash(x + 2),
			IntHash.Hash(x + 1),
			IntHash.Hash(x)
		};
		var bytes = MemoryMarshal.AsBytes(uint32s);
		return new Guid(bytes).WithVersionNumber(guidVersion);
	}

	public static Guid AsGuidHash(this UInt64 x)
	{
		Span<UInt64> uint64s = stackalloc UInt64[]
		{
			IntHash.Hash(x + 1),
			IntHash.Hash(x)
		};
		var bytes = MemoryMarshal.AsBytes(uint64s);
		return new Guid(bytes).WithVersionNumber(guidVersion); // Set the version number to 8 to indicate a non-standard GUID 
	}

	public static Guid AsGuidHash(this Int32 x) => ((UInt32)x).AsGuidHash();
	public static Guid AsGuidHash(this Int64 x) => ((UInt64)x).AsGuidHash();

	// private static Guid WithAssignedVersionAndVariant(this Guid guid)
	// {
	// 	// Assign version 8 (custom) and variant 2 (RFC 4122 variant)
	// 	// https://en.wikipedia.org/w/index.php?title=Universally_unique_identifier&oldid=1298251820#Version_8_(custom)
	// 	return guid
	// 		.WithVersionNumber(guidVersion)
	// 		.WithVariantNumber(0b_10) // RFC 4122 variant (0b_10)
	// 		;
	// }
	
	public static Boolean TryFromGuidHash(this Guid guid, out UInt32 hash)
	{
		if (guid.Version() == guidVersion)
		{
			Span<UInt32> uint32s = stackalloc UInt32[4];
			if (guid.TryWriteBytes(MemoryMarshal.AsBytes(uint32s)))
			{
				hash = IntHash.HashInverse(uint32s[^1]);
				return true;
			}
		}
		hash = default;
		return false;
	}
	
	public static Boolean TryFromGuidHash(this Guid guid, out Int32 hash)
	{
		if (guid.TryFromGuidHash(out UInt32 u32Hash))
		{
			hash = (Int32)u32Hash;
			return true;
		}
		hash = default;
		return false;
	}
	
	public static Boolean TryFromGuidHash(this Guid guid, out UInt64 hash)
	{
		if (guid.Version() == guidVersion)
		{
			Span<UInt64> uint64s = stackalloc UInt64[2];
			if (guid.TryWriteBytes(MemoryMarshal.AsBytes(uint64s)))
			{
				hash = IntHash.HashInverse(uint64s[^1]);
				return true;
			}
		}
		hash = default;
		return false;
	}
	
	public static Boolean TryFromGuidHash(this Guid guid, out Int64 hash)
	{
		if (guid.TryFromGuidHash(out UInt64 u64Hash))
		{
			hash = (Int64)u64Hash;
			return true;
		}
		hash = default;
		return false;
	}

	// public static UInt32 FromGuidHash(this Guid guid)
	// {
	// 	Span<UInt32> uint32s = stackalloc UInt32[16];
	// 	guid.TryWriteBytes(MemoryMarshal.AsBytes(uint32s));
	// 	return IntHash.HashInverse(uint32s[0]);
	// }

	private const Int32 guidVersionByteIndex = 7;

	public static Byte Version(this Guid guid)
	{
		Span<Byte> bytes = stackalloc Byte[16];
		guid.TryWriteBytes(bytes);
		return (Byte)(bytes[guidVersionByteIndex] >> 4);
	}

	private static Guid WithVersionNumber(this Guid guid, Byte version)
	{
		if (version > 0b_1111)
			throw new ArgumentOutOfRangeException(nameof(version), "Version must be a 4-bit value (0-15).");
		Span<Byte> bytes = stackalloc Byte[16];
		guid.TryWriteBytes(bytes);
		var b = bytes[guidVersionByteIndex] & 0b_0000_1111; // Preserve the least significant 4 bits
		var v = (Byte)(version << 4); // Shift the version to the upper 4 bits
		bytes[guidVersionByteIndex] = (Byte)(b | v); // Set the version number in the upper 4 bits
		return new Guid(bytes);
	}
	
	// private static Guid WithVariantNumber(this Guid guid, Byte variant)
	// {
	// 	const Int32 index = 9;
	// 	if (variant > 0b_1111)
	// 		throw new ArgumentOutOfRangeException(nameof(variant), "Variant must be a 4-bit value (0-15).");
	// 	Span<Byte> bytes = stackalloc Byte[16];
	// 	guid.TryWriteBytes(bytes);
	// 	var b = bytes[index] & 0b_0000_1111; // Preserve the least significant 6 bits
	// 	var v = (Byte)(variant << 4); // Shift the variant to the upper 2 bits
	// 	bytes[index] = (Byte)(b | v); // Set the variant number in the upper 2 bits
	// 	return new Guid(bytes);
	// }
}