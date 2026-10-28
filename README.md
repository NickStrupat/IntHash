# IntHash
A pair of integer hash functions that are each other's inverse. Low bias (~0.0208). For all 32-bit values, no collisions and no hash is equal to its input.

```csharp
using NickStrupat;

var x = (UInt32)Random.Shared.NextInt64(0, UInt32.MaxValue + 1L);
var y = IntHash.Hash(x);
var z = IntHash.HashInverse(y);
Console.WriteLine(x == z); // True
```

Overloads exist for `Int32`, `UInt32`, `Int64` and `UInt64`.

## Seeded hashing

`Int32Hasher` and `Int64Hasher` XOR a seed into the input, so different seeds give different hashes for the same value.

```csharp
var hasher = new Int32Hasher(seed: 0xDEADBEEFu);
var hashed = hasher.Hash(42u);
Console.WriteLine(hasher.HashInverse(hashed)); // 42
```

## GUID encoding

`AsGuidHash` encodes an integer as an [RFC 9562](https://www.rfc-editor.org/rfc/rfc9562#name-uuid-version-8) version 8 GUID. `TryFromGuidHash` decodes it, and returns `false` for any GUID that wasn't produced by the matching `AsGuidHash` overload, including GUIDs of the other width.

```csharp
var guid = 42L.AsGuidHash();
if (guid.TryFromGuidHash(out Int64 id))
    Console.WriteLine(id); // 42
```

## Not encryption

These functions obfuscate IDs; they don't protect them. The algorithm is public, so anyone with this library can invert a hash or decode a GUID. A seed doesn't help either: one known input/output pair reveals it (`seed = IntHash.HashInverse(hash) ^ input`).

Ported from https://github.com/skeeto/hash-prospector/blob/396dbe235c94dfc2e9b559fc965bcfda8b6a122c/README.md?plain=1#L237

https://en.wikipedia.org/wiki/Hash_function#Identity_hash_function
