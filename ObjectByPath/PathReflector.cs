using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Pixelscoding.ObjectByPath;

public static class PathReflector
{
	/// <summary>
	///     Get the value of the target specified by the property path converted to the <typeparamref name="TResult" /> type.
	/// </summary>
	/// <typeparam name="TResult"> Type of the result value.</typeparam>
	/// <param name="target"> Object to set properties on.</param>
	/// <param name="path"> Property path on the <paramref name="target" /> that should be set with the value.</param>
	/// <returns> Value of the property.</returns>
	public static TResult Get<TResult>(object target, string path)
	{
		var value = Get(target, path);

		if (value != null)
		{
			var valueType = value.GetType();
			var resultType = typeof(TResult);

			if (!(value is Array))
			{
				// Verify that the value is assignable to the property.
				if (valueType != resultType)
				{
					value = Convert.ChangeType(value, resultType);
				}
			}

			return (TResult)value;
		}

		return default;
	}

	/// <summary>
	///     Get the value of the target specified by the property path.
	/// </summary>
	/// <param name="target"> Object to set properties on.</param>
	/// <param name="path"> Property path on the <paramref name="target" /> that should be set with the <paramref name="value" />.</param>
	/// <returns> Value of the property.</returns>
	public static object Get(object target, string path)
	{
		var currentTarget = target;
		var pathElements = path.Split('.');
		Expression instance = Expression.Constant(target);

		foreach (var element in pathElements)
		{
			var pathElement = element;

			// Handle possible array properties.
			if (IsArray(pathElement))
			{
				// Try to determine the index specified in the path element.
				var index = GetArrayIndex(pathElement);

				if (index.HasValue)
				{
					pathElement = pathElement.Remove(pathElement.IndexOf('['));

					// Get the array property as a expression.
					instance = Expression.Property(instance, pathElement);

					if (instance is MemberExpression memberExpression)
					{
						if (memberExpression.Member is PropertyInfo propertyInfo)
						{
							if (propertyInfo.CanRead)
							{
								currentTarget = propertyInfo.GetValue(currentTarget);
							}
						}
					}

					// Get the array property as a expression that can be used to access the array.
					instance = Expression.ArrayIndex(instance, Expression.Constant(index));

					if (!(currentTarget is Array array))
					{
						continue;
					}

					currentTarget = array.GetValue(index.Value);
				}
			}
			else
			{
				instance = Expression.Property(instance, pathElement);

				// Try to find the property on the target.
				if (instance is MemberExpression memberExpression)
				{
					if (memberExpression.Member is PropertyInfo propertyInfo)
					{
						if (propertyInfo.CanRead)
						{
							currentTarget = propertyInfo.GetValue(currentTarget);
						}
					}
				}
			}
		}

		return currentTarget;
	}

	/// <summary>
	///     Set the value on the target specified by the property path.
	/// </summary>
	/// <typeparam name="TObject"> Type of the target object.</typeparam>
	/// <param name="target"> Object to set properties on.</param>
	/// <param name="path"> Property path on the <paramref name="target" /> that should be set with the <paramref name="value" />.</param>
	/// <param name="value"> Object to set on the property specified in the <paramref name="path" />.</param>
	/// <param name="trySet"> When true, try to set value, if property does not exist then do nothing. If false an exception will be thrown. </param>
	public static void Set<TObject>(TObject target, string path, object? value, bool trySet = false)
	{
		if (target == null)
		{
			throw new ArgumentNullException(nameof(target));
		}

		if (string.IsNullOrWhiteSpace(path))
		{
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
		}
		
		if (value is null)
		{
			return;
		}

		object currentTarget = target;
		var pathElements = path.Split('.');
		Expression instance = Expression.Parameter(typeof(TObject));

		foreach (var element in pathElements)
		{
			var pathElement = element;

			// Handle possible array properties.
			if (IsArray(pathElement))
			{
				// Try to determine the index specified in the path element.
				var index = GetArrayIndex(pathElement);

				if (index.HasValue)
				{
					pathElement = pathElement.Remove(pathElement.IndexOf('['));

					// Get the array property as a expression.
					instance = Expression.Property(instance, pathElement);
					var propertyType = instance.Type.GetElementType();

					if (instance is MemberExpression memberExpression)
					{
						if (memberExpression.Member is PropertyInfo propertyInfo)
						{
							if (propertyInfo.CanWrite)
							{
								// Initialize a empty array of the specified property type.
								if (propertyInfo.GetValue(currentTarget) == null)
								{
									propertyInfo.SetValue(currentTarget, Array.CreateInstance(propertyType ?? throw new InvalidOperationException(), index.Value + 1));
								}
							}

							currentTarget = propertyInfo.GetValue(currentTarget);
						}
					}

					// Get the array property as a expression that can be used to access the array.
					instance = Expression.ArrayIndex(instance, Expression.Constant(index));

					// Initialize the array indexes that are null.
					if (!(currentTarget is Array array))
					{
						continue;
					}

					for (var j = 0; j < index + 1; j++)
					{
						if (array.GetValue(j) == null)
						{
							array.SetValue(New(propertyType), j);
						}
					}

					currentTarget = array.GetValue(index.Value);
				}
			}
			else
			{
				var pathInfo = instance.Type.GetProperty(pathElement);
				if (pathInfo is null)
				{
					if (trySet)
					{
						return;
					}

					if (!trySet)
					{
						throw new Exception($"Property {pathElement} does not exists.");
					}
				}

				instance = Expression.Property(instance, pathElement);

				// Try to find the property on the target.
				if (instance is MemberExpression memberExpression)
				{
					if (memberExpression.Member is PropertyInfo propertyInfo)
					{
						if (propertyInfo.PropertyType.IsClass && propertyInfo.PropertyType != typeof(string) && propertyInfo.PropertyType != typeof(Delegate))
						{
							if (propertyInfo.GetValue(currentTarget) == null)
							{
								if (propertyInfo.CanWrite)
								{
									propertyInfo.SetValue(currentTarget, New(instance.Type));
								}
							}

							currentTarget = propertyInfo.GetValue(currentTarget);
						}
					}
				}
			}
		}

		// Set the value on the current target.
		if (instance is MemberExpression expression)
		{
			if (expression.Member is PropertyInfo propertyInfo)
			{
				var valueType = value.GetType();
				var propertyType = propertyInfo.PropertyType;
				var underlyingType = Nullable.GetUnderlyingType(propertyType);

				if (underlyingType != null)
				{
					propertyType = underlyingType;
				}

				if (propertyInfo.CanWrite)
				{
					// Verify that the value is assignable to the property.
					if (propertyType == valueType)
					{
						propertyInfo.SetValue(currentTarget, value);
						return;
					}

					// Parse enums
					if (propertyType.IsEnum && valueType == typeof(string))
					{
						var parsed = Enum.Parse(propertyType, value.ToString());
						propertyInfo.SetValue(currentTarget, parsed);
						return;
					}

					// Parse bool
					if (propertyType == typeof(bool))
					{
						if (valueType == typeof(int))
						{
							var parsed = Convert.ToInt32(value);
							propertyInfo.SetValue(currentTarget, parsed == 1);
							return;
						}
						if (valueType == typeof(string))
						{
							var parsed = value.ToString()?.ToLower();
							var boolValue = parsed is "1" or "true";
							propertyInfo.SetValue(currentTarget, boolValue);
							return;
						}
					}

					var convertedValue = Convert.ChangeType(value, propertyType);

					if (convertedValue != null)
					{
						propertyInfo.SetValue(currentTarget, convertedValue);
					}
				}
			}
		}
	}

	/// <summary>
	///     Create a new object of the type.
	/// </summary>
	/// <param name="type"> <see cref="Type" /> of the object to create.</param>
	/// <returns></returns>
	private static object New(Type type)
	{
		return Expression.Lambda(Expression.New(type)).Compile().DynamicInvoke();
	}

	/// <summary>
	///     Determine if the path element identifies an array property.
	/// </summary>
	/// <param name="pathElement"></param>
	/// <returns></returns>
	private static bool IsArray(string pathElement)
	{
		return pathElement.Contains('[') && pathElement.Contains(']');
	}

	/// <summary>
	///     Get the array index of a path element.
	/// </summary>
	/// <param name="pathElement"></param>
	/// <returns></returns>
	private static int? GetArrayIndex(string pathElement)
	{
		var index = pathElement.Substring(pathElement.IndexOf('['));
		index = Regex.Match(index, @"\d+", RegexOptions.Compiled)?.Value;

		return !string.IsNullOrWhiteSpace(index) ? Convert.ToInt32(index) : default(int?);
	}
}