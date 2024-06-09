#nullable enable

namespace Extensions
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	/// The Alias Method is a simple and efficient way to sample from a discrete probability distribution.
	/// </summary>
	public class AliasMethod
	{
		private readonly float[] m_ScaledProbabilities;
		private readonly int[] m_Aliases;
		private readonly Random m_Random;

		/// <summary>
		/// Initializes a new instance of the <see cref="AliasMethod"/> class.
		/// </summary>
		/// <param name="probabilities">Span containing the probabilities. The sum of the probabilities must be 1.0.</param>
		/// <param name="random">Optional random number generator instance.</param>
		public AliasMethod(ReadOnlySpan<float> probabilities, [Optional] Random? random)
		{
			var size = probabilities.Length;
			m_ScaledProbabilities = new float[size];
			m_Aliases = new int[size];
			Span<int> small = stackalloc int[size];
			Span<int> large = stackalloc int[size];
			var smallCount = 0;
			var largeCount = 0;
			for (var i = 0; i < size; ++i)
			{
				m_ScaledProbabilities[i] = probabilities[i] * size;
				if (m_ScaledProbabilities[i] < 1.0f)
				{
					small[smallCount++] = i;
				}
				else
				{
					large[largeCount++] = i;
				}
			}
			while (smallCount > 0 && largeCount > 0)
			{
				var less = small[--smallCount];
				var more = large[--largeCount];
				m_ScaledProbabilities[more] += m_ScaledProbabilities[less] - 1.0f;
				m_Aliases[less] = more;
				if (m_ScaledProbabilities[more] < 1.0f)
				{
					small[smallCount++] = more;
				}
				else
				{
					large[largeCount++] = more;
				}
			}
			while (smallCount > 0)
			{
				m_ScaledProbabilities[small[--smallCount]] = 1.0f;
			}
			while (largeCount > 0)
			{
				m_ScaledProbabilities[large[--largeCount]] = 1.0f;
			}
#if NET6_0_OR_GREATER
			m_Random = random ?? Random.Shared;
#else
			m_Random = random ?? new();
#endif
		}

		/// <summary>
		/// Returns the next index based on the probabilities.
		/// </summary>
		/// <returns>The index at which the probability is sampled.</returns>
		public int NextIndex()
		{
			var index = m_Random.Next(m_ScaledProbabilities.Length);
			var toss = m_Random.NextDouble() < m_ScaledProbabilities[index];
			return toss ? index : m_Aliases[index];
		}
	}
}