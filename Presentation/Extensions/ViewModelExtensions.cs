

namespace OperationPrime.Presentation.Extensions;

/// <summary>
/// Extension methods for ViewModels to eliminate repetitive collection operations.
/// Follows DRY principles while maintaining Clean Architecture boundaries.
/// </summary>
public static class ViewModelExtensions
{

    /// <summary>
    /// Loads a collection using a generic pattern to eliminate repetitive foreach loops.
    /// Follows DRY principles for ObservableCollection population.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    /// <param name="collection">The collection to populate</param>
    /// <param name="source">The source data to load from</param>
    public static void LoadFrom<T>(this System.Collections.ObjectModel.ObservableCollection<T> collection, IEnumerable<T> source)
    {
        collection.Clear();
        foreach (var item in source)
        {
            collection.Add(item);
        }
    }


} 