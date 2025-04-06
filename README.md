# IdHash
Pair of integer hash functions that are inverses of each other. No collisions on 32-bit inputs.

```csharp
var x = Random.Shared.Next(0, uint.MaxValue);
var y = IdHash.Hash(x);
var z = IdHash.Unhash(y);
Console.WriteLine(x == z); // True
```

Ported from https://github.com/skeeto/hash-prospector/blob/396dbe235c94dfc2e9b559fc965bcfda8b6a122c/README.md?plain=1#L237

https://en.wikipedia.org/wiki/Hash_function#Identity_hash_function