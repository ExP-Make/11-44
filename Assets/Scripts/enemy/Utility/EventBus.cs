using System;
using System.Collections.Concurrent;

public static class EventBus
{
    private static readonly ConcurrentDictionary<string, Action<object[]>> events 
        = new ConcurrentDictionary<string, Action<object[]>>();

    public static void Subscribe(string eventName, Action<object[]> callback)
    {
        events.AddOrUpdate(eventName, callback, (key, existingVal) => existingVal + callback);
    }

    public static void Unsubscribe(string eventName, Action<object[]> callback)
    {
        if (events.TryGetValue(eventName, out var existingVal))
        {
            var newVal = existingVal - callback;
            if (newVal == null)
            {
                events.TryRemove(eventName, out _);
            }
            else
            {
                events.TryUpdate(eventName, newVal, existingVal);
            }
        }
    }

    public static void Broadcast(string eventName, params object[] args)
    {
        if (events.TryGetValue(eventName, out var action))
        {
            action?.Invoke(args);
        }
    }

    public static bool HasEvent(string eventName)
    {
        return events.ContainsKey(eventName);
    }

    public static void ClearAll()
    {
        events.Clear();
    }
} 