using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Stellar3;

public class RxObject : INotifyPropertyChanged, INotifyPropertyChanging
{
    private readonly Lazy<Notifications> _notification = new(() => new Notifications());
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RxObject"/> class.
    /// </summary>
    protected RxObject()
    {
        Changed = 
            Observable
                .Create<PropertyChangedEventArgs, RxObject>(
                    this,
                    (observer, @this) =>
                    {
                        PropertyChangedEventHandler handler =
                            (_, e) =>
                            {
                                observer.OnNext(e);
                            };

                        @this.PropertyChanged += handler;
                        return Disposable.Create(state => state.@this.PropertyChanged -= state.handler, (@this, handler));
                    });

        Changing = 
            Observable
                .Create<PropertyChangingEventArgs, RxObject>(
                    this,
                    (observer, @this) =>
                    {
                        PropertyChangingEventHandler handler =
                            (_, e) =>
                            {
                                observer.OnNext(e);
                            };

                        @this.PropertyChanging += handler;
                        return Disposable.Create(state => state.@this.PropertyChanging -= state.handler, (@this, handler));
                    });
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public Observable<PropertyChangedEventArgs> Changed { get; }
    
    /// <inheritdoc/>
    public Observable<PropertyChangingEventArgs> Changing { get; }


    /// <inheritdoc/>
    public bool AreChangeNotificationsEnabled() => !_notification.IsValueCreated || Interlocked.Read(ref _notification.Value.ChangeNotificationsSuppressed) == 0;

    /// <inheritdoc/>
    public bool AreChangeNotificationsDelayed() => _notification.IsValueCreated && Interlocked.Read(ref _notification.Value.ChangeNotificationsDelayed) > 0;

    /// <inheritdoc/>
    public IDisposable SuppressChangeNotifications()
    {
        _ = Interlocked.Increment(ref _notification.Value.ChangeNotificationsSuppressed);
        
        return Disposable.Create(x => Interlocked.Decrement(ref x.Value.ChangeNotificationsSuppressed), _notification);
    }

    /// <inheritdoc/>
    public IDisposable DelayChangeNotifications()
    {
        _ = Interlocked.Increment(ref _notification.Value.ChangeNotificationsDelayed);

        return Disposable.Create(
            notification =>
            {
                if (Interlocked.Decrement(ref notification.Value.ChangeNotificationsDelayed) != 0)
                {
                    return;
                }
                
                foreach (var distinctEvent in notification.Value.PropertyChangingEvents.DistinctEvents())
                {
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(distinctEvent.PropertyName));
                }

                foreach (var distinctEvent in notification.Value.PropertyChangedEvents.DistinctEvents())
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(distinctEvent.PropertyName));
                }

                notification.Value.PropertyChangingEvents.Clear();
                notification.Value.PropertyChangedEvents.Clear();
            }, 
            _notification);
    }

    /// <summary>
    /// Raises the property changing event.
    /// </summary>
    /// <param name="args">The argument.</param>
    protected void RaisePropertyChanging(PropertyChangingEventArgs args)
    {
        if (!AreChangeNotificationsEnabled())
        {
            return;
        }

        if (AreChangeNotificationsDelayed())
        {
            _notification.Value.PropertyChangingEvents.Add(args);
            return;
        }

        PropertyChanging?.Invoke(this, args);
    }

    /// <summary>
    /// Raises the property changed event.
    /// </summary>
    /// <param name="args">The argument.</param>
    protected void RaisePropertyChanged(PropertyChangedEventArgs args)
    {
        if (!AreChangeNotificationsEnabled())
        {
            return;
        }

        if (AreChangeNotificationsDelayed())
        {
            _notification.Value.PropertyChangedEvents.Add(args);
            return;
        }

        PropertyChanged?.Invoke(this, args);
    }

    /// <summary>
    /// Raises a property changed event.
    /// </summary>
    /// <param name="propertyName">The name of the property that has changed.</param>
    protected virtual void RaisePropertyChanging([CallerMemberName] string propertyName = "") => RaisePropertyChanging(new PropertyChangingEventArgs(propertyName));

    /// <summary>
    /// Raises a property changed event.
    /// </summary>
    /// <param name="propertyName">The name of the property that has changed.</param>
    protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = "") => RaisePropertyChanged(new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// RaiseAndSetIfChanged fully implements a Setter for a read-write
    /// property on a ReactiveObject, using CallerMemberName to raise the notification
    /// and the ref to the backing field to set the property.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="backingField">A Reference to the backing field for this
    /// property.</param>
    /// <param name="newValue">The new value.</param>
    /// <param name="propertyName">The name of the property, usually
    /// automatically provided through the CallerMemberName attribute.</param>
    protected void RaiseAndSetIfChanged<T>(
        ref T backingField,
        T newValue,
        [CallerMemberName] string? propertyName = null)
    {
        if (propertyName is null)
        {
            throw new ArgumentNullException(nameof(propertyName));
        }

        if (EqualityComparer<T>.Default.Equals(backingField, newValue))
        {
            return;
        }

        RaisePropertyChanging(propertyName);
        backingField = newValue;
        RaisePropertyChanged(propertyName);
    }
}