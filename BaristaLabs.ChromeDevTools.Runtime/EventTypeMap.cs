namespace BaristaLabs.ChromeDevTools.Runtime
{
    using System;
    using System.Collections.Generic;

    public static class EventTypeMap
    {
        private readonly static IDictionary<string, Type> s_methodNameEventTypeDictionary;
        private readonly static IDictionary<Type, string> s_eventTypeMethodNameDictionary;

        static EventTypeMap()
        {
            s_methodNameEventTypeDictionary = new Dictionary<string, Type>()
            {
                { "Runtime.consoleAPICalled", typeof(Runtime.ConsoleAPICalledEvent) },
                { "Runtime.exceptionRevoked", typeof(Runtime.ExceptionRevokedEvent) },
                { "Runtime.exceptionThrown", typeof(Runtime.ExceptionThrownEvent) },
                { "Runtime.executionContextCreated", typeof(Runtime.ExecutionContextCreatedEvent) },
                { "Runtime.executionContextDestroyed", typeof(Runtime.ExecutionContextDestroyedEvent) },
                { "Runtime.executionContextsCleared", typeof(Runtime.ExecutionContextsClearedEvent) },
                { "Runtime.inspectRequested", typeof(Runtime.InspectRequestedEvent) },
            };

            s_eventTypeMethodNameDictionary = new Dictionary<Type, string>()
            {
                { typeof(Runtime.ConsoleAPICalledEvent), "Runtime.consoleAPICalled" },
                { typeof(Runtime.ExceptionRevokedEvent), "Runtime.exceptionRevoked" },
                { typeof(Runtime.ExceptionThrownEvent), "Runtime.exceptionThrown" },
                { typeof(Runtime.ExecutionContextCreatedEvent), "Runtime.executionContextCreated" },
                { typeof(Runtime.ExecutionContextDestroyedEvent), "Runtime.executionContextDestroyed" },
                { typeof(Runtime.ExecutionContextsClearedEvent), "Runtime.executionContextsCleared" },
                { typeof(Runtime.InspectRequestedEvent), "Runtime.inspectRequested" },
            };
        }

        /// <summary>
        /// Gets the event type corresponding to the specified method name.
        /// </summary>
        public static bool TryGetTypeForMethodName(string methodName, out Type eventType)
        {
            return s_methodNameEventTypeDictionary.TryGetValue(methodName, out eventType);
        }

        /// <summary>
        /// Gets the method name corresponding to the specified event type.
        /// </summary>
        public static bool TryGetMethodNameForType<TEvent>(out string methodName)
            where TEvent : IEvent
        {
            return s_eventTypeMethodNameDictionary.TryGetValue(typeof(TEvent), out methodName);
        }
    }
}