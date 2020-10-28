﻿using System;
using System.Linq;
using System.Collections.Generic;
using Umbraco.Core.CodeAnnotations;

namespace Umbraco.Core
{
    /// <summary>
    /// Provides a way to stop the data flow of a logical call context (i.e. CallContext or AsyncLocal) from within
    /// a SafeCallContext and then have the original data restored to the current logical call context.
    /// </summary>
    /// <remarks>
    /// Some usages of this might be when spawning async thread or background threads in which the current logical call context will be flowed but
    /// you don't want it to flow there yet you don't want to clear it either since you want the data to remain on the current thread.
    /// </remarks>
    [UmbracoVolatile]
    public class SafeCallContext : IDisposable
    {
        private static readonly List<Func<object>> EnterFuncs = new List<Func<object>>();
        private static readonly List<Action<object>> ExitActions = new List<Action<object>>();
        private static int _count;
        private readonly List<object> _objects;
        private bool _disposed;

        public static void Register(Func<object> enterFunc, Action<object> exitAction)
        {
            if (enterFunc == null) throw new ArgumentNullException(nameof(enterFunc));
            if (exitAction == null) throw new ArgumentNullException(nameof(exitAction));

            lock (EnterFuncs)
            {
                if (_count > 0) throw new InvalidOperationException("Cannot register while some SafeCallContext instances exist.");
                EnterFuncs.Add(enterFunc);
                ExitActions.Add(exitAction);
            }
        }

        // tried to make the UmbracoDatabase serializable but then it leaks to weird places
        // in ReSharper and so on, where Umbraco.Core is not available. Tried to serialize
        // as an object instead but then it comes *back* deserialized into the original context
        // as an object and of course it breaks everything. Cannot prevent this from flowing,
        // and ExecutionContext.SuppressFlow() works for threads but not domains. and we'll
        // have the same issue with anything that toys with logical call context...
        //
        // so this class lets anything that uses the logical call context register itself,
        // providing two methods:
        // - an enter func that removes and returns whatever is in the logical call context
        // - an exit action that restores the value into the logical call context
        // whenever a SafeCallContext instance is created, it uses these methods to capture
        // and clear the logical call context, and restore it when disposed.
        //
        // in addition, a static Clear method is provided - which uses the enter funcs to
        // remove everything from logical call context - not to be used when the app runs,
        // but can be useful during tests
        //
        // note
        // see System.Transactions
        // pre 4.5.1, the TransactionScope would not flow in async, and then introduced
        // an option to store in the LLC so that it flows
        // they are using a conditional weak table to store the data, and what they store in
        // LLC is the key - which is just an empty MarshalByRefObject that is created with
        // the transaction scope - that way, they can "clear current data" provided that
        // they have the key - but they need to hold onto a ref to the scope... not ok for us

        public static void Clear()
        {
            lock (EnterFuncs)
            {
                foreach (var enter in EnterFuncs)
                    enter();
            }
        }

        public SafeCallContext()
        {
            lock (EnterFuncs)
            {
                _count++;
                _objects = EnterFuncs.Select(x => x()).ToList();
            }
        }

        public void Dispose()
        {
            if (_disposed) throw new ObjectDisposedException("this");
            _disposed = true;
            lock (EnterFuncs)
            {
                for (var i = 0; i < ExitActions.Count; i++)
                    ExitActions[i](_objects[i]);
                _count--;
            }
        }

        // for unit tests ONLY
        internal static void Reset()
        {
            lock (EnterFuncs)
            {
                if (_count > 0) throw new InvalidOperationException("Cannot reset while some SafeCallContext instances exist.");
                EnterFuncs.Clear();
                ExitActions.Clear();
            }
        }
    }
}
