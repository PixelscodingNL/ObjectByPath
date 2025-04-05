using Pixelscoding.ObjectByPath.Extensions;

namespace Pixelscoding.ObjectByPath.Comparers;

public sealed class NumericOrderer : IComparer<string>
{
	public int Compare(string? x, string? y)
	{
		if (x == null && y == null)
		{
			return 0;
		}

		if (x == null)
		{
			return -1;
		}

		if (y == null)
		{
			return 1;
		}

		var xSpan = x.AsSpan();
		var ySpan = y.AsSpan();

		var commonPrefixLength = xSpan.CommonPrefixLength(ySpan);

		while (commonPrefixLength > 0)
		{
			xSpan = xSpan[commonPrefixLength..];
			ySpan = ySpan[commonPrefixLength..];
			commonPrefixLength = xSpan.CommonPrefixLength(ySpan);
		}

		if (int.TryParse(xSpan, out var xNumber) && int.TryParse(ySpan, out var yNumber))
		{
			return xNumber.CompareTo(yNumber);
		}

		return xSpan.CompareTo(ySpan, StringComparison.Ordinal);
	}
}