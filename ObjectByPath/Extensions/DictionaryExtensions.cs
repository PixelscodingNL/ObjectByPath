using Pixelscoding.ObjectByPath.Comparers;

namespace Pixelscoding.ObjectByPath.Extensions;

public static class DictionaryExtensions
{
	public static Dictionary<string, TValue> SortDictionaryDescendingKeynames<TValue>(this Dictionary<string, TValue> dictionary)
	{
		return dictionary
			.OrderBy(kv => kv.Key, new CustomComparer())
			.Reverse()
			.ToDictionary(kv => kv.Key, kv => kv.Value);
	}
}