using System;
using System.Buffers.Binary;

namespace NickStrupat;

/// <summary>
/// Encodes 32/64-bit integers as RFC 9562 version 8 (custom) GUIDs, and decodes them back.
/// </summary>
/// <remarks>
/// The GUID holds the hashed integer plus check data chained from it, so <c>TryFromGuidHash</c> rejects GUIDs that
/// were not produced by the matching <c>AsGuidHash</c> overload.
/// </remarks>
public static class AsGuidHashExtensions
{
	// https://www.rfc-editor.org/rfc/rfc9562#name-uuid-version-8
	private const Byte guidVersion = 8;
	private const Byte guidVariant = 0b_10;

	private const Int32 guidVersionByteIndex = 7; // upper nibble, in the little-endian layout of Guid.TryWriteBytes
	private const Int32 guidVariantByteIndex = 8; // upper 2 bits

	/// <summary>Encodes <paramref name="x"/> as a version 8 GUID.</summary>
	/// <param name="x">The integer to encode</param>
	/// <returns>A GUID that <see cref="TryFromGuidHash(Guid, out UInt32)"/> decodes back to <paramref name="x"/></returns>
	public static Guid AsGuidHash(this UInt32 x)
	{
		// Each check word mixes in x as well as the previous word, so GUIDs of nearby values share no words. The
		// previous word is hashed first so a zero payload doesn't make every check word equal H(x) == 0.
		var payload = IntHash.Hash(x);
		var check1 = IntHash.Hash(x + IntHash.Hash(payload));
		var check2 = IntHash.Hash(x + IntHash.Hash(check1));
		var check3 = IntHash.Hash(x + IntHash.Hash(check2));
		return ToGuidHash(
			check3,
			check2, // overlaps the version nibble
			check1, // overlaps the variant bits
			payload
		);
	}

	/// <summary>Encodes <paramref name="x"/> as a version 8 GUID.</summary>
	/// <param name="x">The integer to encode</param>
	/// <returns>A GUID that <see cref="TryFromGuidHash(Guid, out UInt64)"/> decodes back to <paramref name="x"/></returns>
	public static Guid AsGuidHash(this UInt64 x)
	{
		var payload = IntHash.Hash(x);
		var check = IntHash.Hash(x + IntHash.Hash(payload)); // see AsGuidHash(UInt32)
		return ToGuidHash(
			(UInt32)payload,
			(UInt32)check,          // overlaps the version nibble
			(UInt32)(check >> 32),  // overlaps the variant bits
			(UInt32)(payload >> 32)
		);
	}

	/// <inheritdoc cref="AsGuidHash(UInt32)"/>
	public static Guid AsGuidHash(this Int32 x) => ((UInt32)x).AsGuidHash();

	/// <inheritdoc cref="AsGuidHash(UInt64)"/>
	public static Guid AsGuidHash(this Int64 x) => ((UInt64)x).AsGuidHash();

	/// <summary>Decodes a GUID produced by <see cref="AsGuidHash(UInt32)"/>.</summary>
	/// <param name="guid">The GUID to decode</param>
	/// <param name="hash">The decoded integer, or <c>0</c> if decoding failed</param>
	/// <returns><c>true</c> if <paramref name="guid"/> was produced by <see cref="AsGuidHash(UInt32)"/>; otherwise <c>false</c></returns>
	public static Boolean TryFromGuidHash(this Guid guid, out UInt32 hash)
	{
		if (guid.Version() == guidVersion)
		{
			Span<Byte> bytes = stackalloc Byte[16];
			if (guid.TryWriteBytes(bytes))
			{
				var candidate = IntHash.HashInverse(ReadWord(bytes, 3));
				if (candidate.AsGuidHash() == guid)
				{
					hash = candidate;
					return true;
				}
			}
		}
		hash = default;
		return false;
	}

	/// <summary>Decodes a GUID produced by <see cref="AsGuidHash(Int32)"/>.</summary>
	/// <param name="guid">The GUID to decode</param>
	/// <param name="hash">The decoded integer, or <c>0</c> if decoding failed</param>
	/// <returns><c>true</c> if <paramref name="guid"/> was produced by <see cref="AsGuidHash(Int32)"/>; otherwise <c>false</c></returns>
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

	/// <summary>Decodes a GUID produced by <see cref="AsGuidHash(UInt64)"/>.</summary>
	/// <param name="guid">The GUID to decode</param>
	/// <param name="hash">The decoded integer, or <c>0</c> if decoding failed</param>
	/// <returns><c>true</c> if <paramref name="guid"/> was produced by <see cref="AsGuidHash(UInt64)"/>; otherwise <c>false</c></returns>
	public static Boolean TryFromGuidHash(this Guid guid, out UInt64 hash)
	{
		if (guid.Version() == guidVersion)
		{
			Span<Byte> bytes = stackalloc Byte[16];
			if (guid.TryWriteBytes(bytes))
			{
				var payload = ((UInt64)ReadWord(bytes, 3) << 32) | ReadWord(bytes, 0);
				var candidate = IntHash.HashInverse(payload);
				if (candidate.AsGuidHash() == guid)
				{
					hash = candidate;
					return true;
				}
			}
		}
		hash = default;
		return false;
	}

	/// <summary>Decodes a GUID produced by <see cref="AsGuidHash(Int64)"/>.</summary>
	/// <param name="guid">The GUID to decode</param>
	/// <param name="hash">The decoded integer, or <c>0</c> if decoding failed</param>
	/// <returns><c>true</c> if <paramref name="guid"/> was produced by <see cref="AsGuidHash(Int64)"/>; otherwise <c>false</c></returns>
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

	/// <summary>Gets the RFC 9562 version number of <paramref name="guid"/>.</summary>
	/// <param name="guid">The GUID to inspect</param>
	/// <returns>The 4-bit version number</returns>
	public static Byte Version(this Guid guid)
	{
		Span<Byte> bytes = stackalloc Byte[16];
		guid.TryWriteBytes(bytes);
		return (Byte)(bytes[guidVersionByteIndex] >> 4);
	}

	private const Int32 wordSize = sizeof(UInt32);

	// Words are little-endian on every platform, matching the little-endian fields of Guid.TryWriteBytes, so a value
	// encodes to the same GUID on big-endian and little-endian machines
	private static Guid ToGuidHash(UInt32 word0, UInt32 word1, UInt32 word2, UInt32 word3)
	{
		Span<Byte> bytes = stackalloc Byte[16];
		BinaryPrimitives.WriteUInt32LittleEndian(bytes, word0);
		BinaryPrimitives.WriteUInt32LittleEndian(bytes[wordSize..], word1);
		BinaryPrimitives.WriteUInt32LittleEndian(bytes[(2 * wordSize)..], word2);
		BinaryPrimitives.WriteUInt32LittleEndian(bytes[(3 * wordSize)..], word3);
		bytes[guidVersionByteIndex] = (Byte)((bytes[guidVersionByteIndex] & 0b_0000_1111) | (guidVersion << 4));
		bytes[guidVariantByteIndex] = (Byte)((bytes[guidVariantByteIndex] & 0b_0011_1111) | (guidVariant << 6));
		return new Guid(bytes);
	}

	private static UInt32 ReadWord(ReadOnlySpan<Byte> bytes, Int32 index) =>
		BinaryPrimitives.ReadUInt32LittleEndian(bytes[(index * wordSize)..]);
}
