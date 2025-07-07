using System;

namespace NickStrupat;

public sealed class IntHasher(UInt32 seed)
{
	public IntHasher(Int32 seed) : this((UInt32)seed) {}
	
	public UInt32 Hash(UInt32 x) => IntHash.Hash(x ^ seed);
	public UInt32 HashInverse(UInt32 x) => IntHash.HashInverse(x ^ seed);
	
	public Int32 Hash(Int32 x) => IntHash.Hash(x ^ (Int32)seed);
	public Int32 HashInverse(Int32 x) => IntHash.HashInverse(x ^ (Int32)seed);
	
	public UInt64 Hash(UInt64 x) => IntHash.Hash(x ^ seed);
	public UInt64 HashInverse(UInt64 x) => IntHash.HashInverse(x ^ seed);
	
	public Int64 Hash(Int64 x) => IntHash.Hash(x ^ seed);
	public Int64 HashInverse(Int64 x) => IntHash.HashInverse(x ^ seed);
}