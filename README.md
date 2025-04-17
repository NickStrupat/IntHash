# IdHash
A pair of integer hash functions that are each other's inverse. Low bias (~0.0208). For all 32-bit values, no collisions and no hash is equal to their input.

```csharp
var x = Random.Shared.Next(0, uint.MaxValue);
var y = IdHash.Hash(x);
var z = IdHash.Unhash(y);
Console.WriteLine(x == z); // True
```

Ported from https://github.com/skeeto/hash-prospector/blob/396dbe235c94dfc2e9b559fc965bcfda8b6a122c/README.md?plain=1#L237

https://en.wikipedia.org/wiki/Hash_function#Identity_hash_function