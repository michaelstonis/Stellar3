using System.ComponentModel;
using ObservableCollections;

namespace Stellar3;

internal class Notifications
{
    public readonly ObservableList<PropertyChangedEventArgs> PropertyChangedEvents = new();
    public readonly ObservableList<PropertyChangingEventArgs> PropertyChangingEvents = new();
    public long ChangeNotificationsDelayed;
    public long ChangeNotificationsSuppressed;
}