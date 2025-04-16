using System;

namespace NickStrupat;

/// <summary>
/// A pair of integer hash functions that are inverses of each other. No collisions on 32-bit inputs. Low bias (~0.0208). No hashes are equal to their inputs.
/// </summary>
/// <note>Ported from https://github.com/skeeto/hash-prospector/blob/396dbe235c94dfc2e9b559fc965bcfda8b6a122c/README.md?plain=1#L237</note>
public static class IdHash
{
	/// <summary>The inverse of <see cref="HashInverse(UInt32)"/>.</summary>
	/// <param name="x">The integer to hash</param>
	/// <returns>The hashed integer</returns>
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

	/// <summary>The inverse of <see cref="Hash(UInt32)"/>.</summary>
	/// <param name="x">The integer to hash</param>
	/// <returns>The hashed integer</returns>
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
	
	/// <summary>The inverse of <see cref="HashInverse(Int32)"/>.</summary>
	/// <param name="x">The integer to hash</param>
	/// <returns>The hashed integer</returns>
	public static Int32 Hash(Int32 x) => (Int32)Hash((UInt32)x);
	
	/// <summary>The inverse of <see cref="Hash(Int32)"/>.</summary>
	/// <param name="x">The integer to hash</param>
	/// <returns>The hashed integer</returns>
	public static Int32 HashInverse(Int32 x) => (Int32)Hash((UInt32)x);
}