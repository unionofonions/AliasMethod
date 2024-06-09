#nullable enable

namespace Example
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;

	public class Program
	{
		static private void Main()
		{
			var values = new string[] { "First", "Second", "Third", "Fourth" };
			var probabilities = new float[] { 0.1f, 0.2f, 0.3f, 0.4f };
			var countMap = new Dictionary<string, int>();
			var random = new Random();
			for (var i = 0; i < values.Length; ++i)
			{
				countMap.Add(values[i], 0);
			}
			var stopwatch = Stopwatch.StartNew();
			var aliasMethod = new Extensions.AliasMethod(probabilities, random);
			const int samples = 10_000_000;
			for (var i = 0; i < samples; ++i)
			{
				var index = aliasMethod.NextIndex();
				++countMap[values[index]];
			}
			stopwatch.Stop();
			var j = 0;
			foreach (var pair in countMap)
			{
				Console.WriteLine($"[{pair.Key}] Result: {pair.Value / (float)samples:P4}, Expected: {probabilities[j++]:P4}");
			}
			Console.WriteLine($"\n{samples:N0} iterations done in {stopwatch.ElapsedMilliseconds} ms.");
		}
	}
}