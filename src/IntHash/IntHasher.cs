using System;

namespace NickStrupat;

/// <summary>
/// Hashes 32-bit integers like <see cref="IntHash"/>, with a seed XORed into the input so different seeds give
/// different hashes for the same value.
/// </summary>
/// <remarks>
/// The seed is not a secret key: one known input and its hash reveal it
/// (<c>seed = IntHash.HashInverse(hash) ^ input</c>). Unlike <see cref="IntHash.Hash(UInt32)"/>, a seeded hash can
/// equal its input.
/// </remarks>
/// <param name="seed">The value XORed into every input before hashing</param>
public readonly struct Int32Hasher(UInt32 seed)
{
	/// <inheritdoc cref="Int32Hasher(UInt32)"/>
	public Int32Hasher(Int32 seed) : this((UInt32)seed) {}

	/// <summary>Hashes <paramref name="x"/> with the seed. Reversed by <see cref="HashInverse(UInt32)"/>.</summary>
	/// <param name="x">The integer to hash</param>
	/// <returns>The hashed integer</returns>
	public UInt32 Hash(UInt32 x) => IntHash.Hash(x ^ seed);

	/// <summary>Reverses <see cref="Hash(UInt32)"/> for the same seed.</summary>
	/// <param name="x">A value returned by <see cref="Hash(UInt32)"/></param>
	/// <returns>The integer that <see cref="Hash(UInt32)"/> hashed to <paramref name="x"/></returns>
	public UInt32 HashInverse(UInt32 x) => IntHash.HashInverse(x) ^ seed;

	/// <summary>Hashes <paramref name="x"/> with the seed. Reversed by <see cref="HashInverse(Int32)"/>.</summary>
	/// <param name="x">The integer to hash</param>
	/// <returns>The hashed integer</returns>
	public Int32 Hash(Int32 x) => IntHash.Hash(x ^ (Int32)seed);

	/// <summary>Reverses <see cref="Hash(Int32)"/> for the same seed.</summary>
	/// <param name="x">A value returned by <see cref="Hash(Int32)"/></param>
	/// <returns>The integer that <see cref="Hash(Int32)"/> hashed to <paramref name="x"/></returns>
	public Int32 HashInverse(Int32 x) => IntHash.HashInverse(x) ^ (Int32)seed;
}

/// <summary>
/// Hashes 64-bit integers like <see cref="IntHash"/>, with a seed XORed into the input so different seeds give
/// different hashes for the same value.
/// </summary>
/// <remarks>
/// The seed is not a secret key: one known input and its hash reveal it
/// (<c>seed = IntHash.HashInverse(hash) ^ input</c>).
/// </remarks>
/// <param name="seed">The value XORed into every input before hashing</param>
public readonly struct Int64Hasher(UInt64 seed)
{
	/// <inheritdoc cref="Int64Hasher(UInt64)"/>
	public Int64Hasher(Int64 seed) : this((UInt64)seed) {}

	/// <summary>Hashes <paramref name="x"/> with the seed. Reversed by <see cref="HashInverse(UInt64)"/>.</summary>
	/// <param name="x">The integer to hash</param>
	/// <returns>The hashed integer</returns>
	public UInt64 Hash(UInt64 x) => IntHash.Hash(x ^ seed);

	/// <summary>Reverses <see cref="Hash(UInt64)"/> for the same seed.</summary>
	/// <param name="x">A value returned by <see cref="Hash(UInt64)"/></param>
	/// <returns>The integer that <see cref="Hash(UInt64)"/> hashed to <paramref name="x"/></returns>
	public UInt64 HashInverse(UInt64 x) => IntHash.HashInverse(x) ^ seed;

	/// <summary>Hashes <paramref name="x"/> with the seed. Reversed by <see cref="HashInverse(Int64)"/>.</summary>
	/// <param name="x">The integer to hash</param>
	/// <returns>The hashed integer</returns>
	public Int64 Hash(Int64 x) => IntHash.Hash(x ^ (Int64)seed);

	/// <summary>Reverses <see cref="Hash(Int64)"/> for the same seed.</summary>
	/// <param name="x">A value returned by <see cref="Hash(Int64)"/></param>
	/// <returns>The integer that <see cref="Hash(Int64)"/> hashed to <paramref name="x"/></returns>
	public Int64 HashInverse(Int64 x) => IntHash.HashInverse(x) ^ (Int64)seed;
}
