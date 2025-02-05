namespace Pixelscoding.ObjectByPath.Comparers;

internal class CustomComparer : IComparer<string>
{
	public int Compare(string x, string y)
	{
		var xParts = x.Split('.');
		var yParts = y.Split('.');

		for (int i = 0; i < Math.Min(xParts.Length, yParts.Length); i++)
		{
			var xPart = xParts[i];
			var yPart = yParts[i];

			int xIndex = xPart.IndexOf('[');
			int yIndex = yPart.IndexOf('[');

			if (xIndex != -1 && yIndex != -1)
			{
				var xSubPart = xPart[..xIndex];
				var ySubPart = yPart[..yIndex];

				int result = string.Compare(xSubPart, ySubPart, StringComparison.InvariantCultureIgnoreCase);
				if (result != 0)
				{
					return result;
				}

				var xXValue = int.Parse(xPart.Substring(xIndex + 1, xPart.Length - xIndex - 2));
				var yXValue = int.Parse(yPart.Substring(yIndex + 1, yPart.Length - yIndex - 2));

				result = xXValue.CompareTo(yXValue);
				if (result != 0)
				{
					return result;
				}
			}
		}

		return x.Length.CompareTo(y.Length);
	}
}