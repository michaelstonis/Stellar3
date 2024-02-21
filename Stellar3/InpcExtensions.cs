using System.ComponentModel;

namespace Stellar3;

internal static class InpcExtensions
{
    public static IEnumerable<PropertyChangedEventArgs> DistinctEvents(this IEnumerable<PropertyChangedEventArgs> events)
    {
        var eventArgsList = events.ToList();
        if (eventArgsList.Count <= 1)
        {
            return eventArgsList;
        }

        var seen = new HashSet<string>();
        var uniqueEvents = new Stack<PropertyChangedEventArgs>(eventArgsList.Count);

        for (var i = eventArgsList.Count - 1; i >= 0; i--)
        {
            var propertyName = eventArgsList[i].PropertyName;
            if (propertyName is not null && seen.Add(propertyName))
            {
                uniqueEvents.Push(eventArgsList[i]);
            }
        }

        // Stack enumerates in LIFO order
        return uniqueEvents;
    }
    
    public static IEnumerable<PropertyChangingEventArgs> DistinctEvents(this IEnumerable<PropertyChangingEventArgs> events)
    {
        var eventArgsList = events.ToList();
        if (eventArgsList.Count <= 1)
        {
            return eventArgsList;
        }

        var seen = new HashSet<string>();
        var uniqueEvents = new Stack<PropertyChangingEventArgs>(eventArgsList.Count);

        for (var i = eventArgsList.Count - 1; i >= 0; i--)
        {
            var propertyName = eventArgsList[i].PropertyName;
            if (propertyName is not null && seen.Add(propertyName))
            {
                uniqueEvents.Push(eventArgsList[i]);
            }
        }

        // Stack enumerates in LIFO order
        return uniqueEvents;
    }
}