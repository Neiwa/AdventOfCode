namespace Helpers.Extensions;

public static class ArrayExtensions
{
    /// <summary>
    /// Creates a new array with the specified element replaced at the given index.
    /// </summary>
    /// <typeparam name="T">The value type of the elements in the array.</typeparam>
    /// <param name="array">The source array to clone and modify. Must not be <see langword="null"/>.</param>
    /// <param name="index">The zero-based index of the element to replace. Must be within the bounds of the array.</param>
    /// <param name="value">The new value to set at the specified index.</param>
    /// <returns>A new array with the specified element replaced by <paramref name="value"/>.</returns>
    public static T[] WithElement<T>(this T[] array, int index, T value) where T : struct
    {
        var newArray = (T[])array.Clone();
        newArray[index] = value;
        return newArray;
    }
}
