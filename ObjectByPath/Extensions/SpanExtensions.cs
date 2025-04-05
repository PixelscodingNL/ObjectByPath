namespace Pixelscoding.ObjectByPath.Extensions;

using System;

public static class SpanExtensions
{
	public static int CommonPrefixLength<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other)
		where T : IEquatable<T>
	{
		int minLength = Math.Min(span.Length, other.Length);

		for (int i = 0; i < minLength; i++)
		{
			if (!span[i].Equals(other[i]))
			{
				return i;
			}
		}

		return minLength;
	}
}