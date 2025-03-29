using Pixelscoding.ObjectByPath.Comparers;

namespace Pixelscoding.ObjectByPath.Extensions;

public static class DictionaryExtensions
{
	public static Dictionary<string, TValue> SortDictionaryDescendingKeynames<TValue>(this Dictionary<string, TValue> dictionary)
	{
		return dictionary
			.OrderByDescending(kv => kv.Key, new CustomComparer())
			.ToDictionary(kv => kv.Key, kv => kv.Value);
	}
}