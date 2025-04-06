using System;

namespace NickStrupat;

/// <summary>
/// A pair of 32-bit hash functions with no collisions on all 32-bit inputs
/// </summary>
/// <note>Ported from https://github.com/skeeto/hash-prospector/blob/396dbe235c94dfc2e9b559fc965bcfda8b6a122c/README.md?plain=1#L237</note>
public static class IdHash
{
	/// A perfect hash function for 32-bit integers. No collisions for all 32-bit values. The inverse of <see cref="Hash2"/>.
	/// <param name="x">The integer to hash</param>
	/// <returns>The hashed integer</returns>
	public static UInt32 Hash(UInt32 x)
	{
		unchecked
		{
			x += 2; // change from original implementation: increment by 2 instead of 1 because 1 produced a result that was equal to the input (input: 1,279,745,357)
			x ^= x >> 17;
			x *= 0xed5ad4bb;
			x ^= x >> 11;
			x *= 0xac4c1b51;
			x ^= x >> 15;
			x *= 0x31848bab;
			x ^= x >> 14;
			return x;
		}
	}

	/// A perfect hash function for 32-bit integers. No collisions for all 32-bit values. The inverse of <see cref="Hash"/>.
	/// <param name="x">The integer to hash</param>
	/// <returns>The hashed integer</returns>
	public static UInt32 HashInverse(UInt32 x)
	{
		unchecked
		{
			x ^= x >> 14 ^ x >> 28;
			x *= 0x32b21703;
			x ^= x >> 15 ^ x >> 30;
			x *= 0x469e0db1;
			x ^= x >> 11 ^ x >> 22;
			x *= 0x79a85073;
			x ^= x >> 17;
			x -= 2;
			return x;
		}
	}
}