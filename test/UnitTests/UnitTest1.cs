
using System.Reflection;
using System.Runtime.CompilerServices;
using NickStrupat;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace UnitTests;

public class IdHashTests(ITestOutputHelper toh)
{
	// private Action<String> print = toh
	// 	.GetType()
	// 	.GetMethod("QueueTestOutput", BindingFlags.NonPublic | BindingFlags.Instance)
	// 	?.CreateDelegate<Action<String>>()!;
	
	[Fact]
	public void Hash_WhenCalledWithZero_DoesNotReturnZero()
	{
		var result = IdHash.Hash(0);
		Assert.NotEqual(0u, result);
	}
	
	[Fact]
	public void HashInverse_WhenCalledWithZero_DoesNotReturnZero()
	{
		var result = IdHash.HashInverse(0);
		Assert.NotEqual(0u, result);
	}
	
	[Fact]
	public void HashInverse_WhenCalledWithTheResultOfCallingHash_ReturnsTheOriginalValue()
	{
		const UInt32 percent = UInt32.MaxValue / 100;
		for (var i = 0ul; i <= UInt32.MaxValue; i++)
		{
			var (quotient, remainder) = Math.DivRem(i, percent);
			if (remainder == 0)
				toh.WriteLine(quotient + "%");
			
			var input = (UInt32)i;
			var hash = IdHash.Hash(input);
			if (input == hash) // Assert class methods are too slow
				Assert.Fail("Input and hash result are equal: " + input + " => " + hash);
			var inverse = IdHash.HashInverse(hash);
			if (input != inverse) // Assert class methods are too slow
				Assert.Fail("Input and inverse result are not equal: " + input + " => " + hash + " => " + inverse);
		}
	}
	
	[Fact]
	public void Hash_WhenCalledWithAllPossibleValues_ReturnsUniqueValues()
	{
		var _32BitValueCount = UInt32.MaxValue + 1ul;
		var bitStore = new Byte[_32BitValueCount / 8];
		const UInt32 tenPercent = UInt32.MaxValue / 10;
		for (var i = 0ul; i <= UInt32.MaxValue; i++)
		{
			var (quotient, remainder) = Math.DivRem(i, tenPercent);
			if (remainder == 0)
				toh.WriteLine(quotient + "%");
			
			var input = (UInt32)i;
			var hash = IdHash.Hash(input);
			var byteIndex = (Int32)(hash / 8);
			var bitIndex = (Byte)(hash & 7);
			ref var @byte = ref bitStore[byteIndex];
			var bit = (@byte >> bitIndex) & 1;
			if (bit == 1)
			{
				Assert.Fail($"Collision found at {i} => {hash}");
			}
			else
			{
				@byte |= (Byte)(1 << bitIndex);
			}
		}
		toh.WriteLine("No collision found");
		
		Assert.True(bitStore.All(x => x == 0xff), "Collision found");
	}
}

static class ITestOutputHelperExtensions
{
	public static void Write(this ITestOutputHelper toh, String message)
	{
		ArgumentNullException.ThrowIfNull(toh);
		if (toh is not TestOutputHelper x)
			throw new ArgumentException("This `ITestOutputHelper` is not an instance of the `TestOutputHelper` class.");
		ArgumentNullException.ThrowIfNull(message);
		var print = printDelegate.GetValue(toh, t => queueTestOutput.CreateDelegate<Action<String>>(t));
		print(message);
	}
	
	private static readonly ConditionalWeakTable<ITestOutputHelper, Action<String>> printDelegate = new();

	private static readonly MethodInfo queueTestOutput = typeof(TestOutputHelper)
		.GetMethod("QueueTestOutput", BindingFlags.NonPublic | BindingFlags.Instance)
		?? throw new InvalidOperationException("Could not find the QueueTestOutput method.");
}