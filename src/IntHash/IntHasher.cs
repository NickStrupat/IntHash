using System;

namespace NickStrupat;

public readonly struct Int32Hasher(UInt32 seed)
{
	public Int32Hasher(Int32 seed) : this((UInt32)seed) {}
	
	public UInt32 Hash(UInt32 x) => IntHash.Hash(x ^ seed);
	public UInt32 HashInverse(UInt32 x) => IntHash.HashInverse(x ^ seed);
	
	public Int32 Hash(Int32 x) => IntHash.Hash(x ^ (Int32)seed);
	public Int32 HashInverse(Int32 x) => IntHash.HashInverse(x ^ (Int32)seed);
}

public readonly struct Int64Hasher(UInt64 seed)
{
	public Int64Hasher(Int64 seed) : this((UInt64)seed) {}
	
	public UInt64 Hash(UInt64 x) => IntHash.Hash(x ^ seed);
	public UInt64 HashInverse(UInt64 x) => IntHash.HashInverse(x ^ seed);
	
	public Int64 Hash(Int64 x) => IntHash.Hash(x ^ (Int32)seed);
	public Int64 HashInverse(Int64 x) => IntHash.HashInverse(x ^ (Int32)seed);
}