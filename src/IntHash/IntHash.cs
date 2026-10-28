using System;

namespace NickStrupat;

/// <summary>
/// Pairs of integer hash functions that are each other's inverse. Low bias (~0.0208). For all 32-bit values, no
/// collisions and no hash is equal to its input.
/// </summary>
/// <remarks>
/// Every <c>Hash</c> overload is a bijection, so <c>HashInverse(Hash(x)) == x</c> for every <c>x</c>. This is
/// obfuscation, not encryption: anyone with this library can invert a hash.
/// Ported from https://github.com/skeeto/hash-prospector/blob/396dbe235c94dfc2e9b559fc965bcfda8b6a122c/README.md?plain=1#L237
/// </remarks>
public static class IntHash
{
	/// <summary>Hashes <paramref name="x"/>. Reversed by <see cref="HashInverse(UInt32)"/>.</summary>
	/// <param name="x">The integer to hash</param>
	/// <returns>The hashed integer, which never equals <paramref name="x"/></returns>
	public static UInt32 Hash(UInt32 x)
	{
		x += 2; // change from original: increment by 2 because incrementing by 1 causes `hash(x) = x` where x == 1_279_745_357
		x ^= x >> 17;
		x *= 0xed5ad4bb;
		x ^= x >> 11;
		x *= 0xac4c1b51;
		x ^= x >> 15;
		x *= 0x31848bab;
		x ^= x >> 14;
		return x;
	}

	/// <summary>Reverses <see cref="Hash(UInt32)"/>.</summary>
	/// <param name="x">A value returned by <see cref="Hash(UInt32)"/></param>
	/// <returns>The integer that <see cref="Hash(UInt32)"/> hashed to <paramref name="x"/></returns>
	public static UInt32 HashInverse(UInt32 x)
	{
		x ^= x >> 14 ^ x >> 28;
		x *= 0x32b21703;
		x ^= x >> 15 ^ x >> 30;
		x *= 0x469e0db1;
		x ^= x >> 11 ^ x >> 22;
		x *= 0x79a85073;
		x ^= x >> 17;
		x -= 2; // change from original: decrement by 2 because decrementing by 1 causes `hash(x) = x` where x == 1_279_745_357
		return x;
	}
	
	/// <summary>Hashes <paramref name="x"/>. Reversed by <see cref="HashInverse(Int32)"/>.</summary>
	/// <param name="x">The integer to hash</param>
	/// <returns>The hashed integer, which never equals <paramref name="x"/></returns>
	public static Int32 Hash(Int32 x) => (Int32)Hash((UInt32)x);
	
	/// <summary>Reverses <see cref="Hash(Int32)"/>.</summary>
	/// <param name="x">A value returned by <see cref="Hash(Int32)"/></param>
	/// <returns>The integer that <see cref="Hash(Int32)"/> hashed to <paramref name="x"/></returns>
	public static Int32 HashInverse(Int32 x) => (Int32)HashInverse((UInt32)x);
	
	/// <summary>Hashes <paramref name="x"/>. Reversed by <see cref="HashInverse(UInt64)"/>.</summary>
	/// <param name="x">The integer to hash</param>
	/// <returns>The hashed integer</returns>
	/// <remarks>
	/// Built from two rounds of <see cref="Hash(UInt32)"/> so every input bit affects both halves of the result.
	/// Unlike the 32-bit overload, it is not proven to have no input that hashes to itself.
	/// </remarks>
	public static UInt64 Hash(UInt64 x)
	{
		var (lo, hi) = ((UInt32)x, (UInt32)(x >> 32)); // split the 64-bit integer into two 32-bit integers
		var loHash = Hash(lo); // hash the low part
		var hiXorHash = Hash(hi ^ loHash); // hash the high part XORed with the low hashed part so low bits affect the high bits
		var loHashXor = loHash ^ hiXorHash; // XOR the low hashed part with the high xor hashed part so high bits affect the low bits
		return ((UInt64)hiXorHash << 32) | loHashXor;
	}
	
	/// <summary>Reverses <see cref="Hash(UInt64)"/>.</summary>
	/// <param name="x">A value returned by <see cref="Hash(UInt64)"/></param>
	/// <returns>The integer that <see cref="Hash(UInt64)"/> hashed to <paramref name="x"/></returns>
	public static UInt64 HashInverse(UInt64 x)
	{
		var (loHashXor, hiXorHash) = ((UInt32)x, (UInt32)(x >> 32));
		var loHash = loHashXor ^ hiXorHash;
		var hi = HashInverse(hiXorHash) ^ loHash;
		var lo = HashInverse(loHash);
		return ((UInt64)hi << 32) | lo;
	}
	
	/// <inheritdoc cref="Hash(UInt64)"/>
	public static Int64 Hash(Int64 x) => (Int64)Hash((UInt64)x);

	/// <summary>Reverses <see cref="Hash(Int64)"/>.</summary>
	/// <param name="x">A value returned by <see cref="Hash(Int64)"/></param>
	/// <returns>The integer that <see cref="Hash(Int64)"/> hashed to <paramref name="x"/></returns>
	public static Int64 HashInverse(Int64 x) => (Int64)HashInverse((UInt64)x);
}