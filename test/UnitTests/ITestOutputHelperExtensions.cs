using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit.v3;

namespace UnitTests;

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