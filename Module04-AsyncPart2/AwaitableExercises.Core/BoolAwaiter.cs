using System;
using System.Runtime.CompilerServices;

namespace AwaitableExercises.Core
{
    public static class BoolExtensions
    {
        // the Bool type needs to be extended to support the OnCompleted method defined by INotifyCompletion
        public static BoolAwaiter GetAwaiter(this bool boolVar)
        {
            return new BoolAwaiter(boolVar);
        }
    }

    public class BoolAwaiter : INotifyCompletion
    {
        private readonly bool _boolVar = false;
        public void OnCompleted(Action continuation) => continuation();
        public bool IsCompleted => true;
        public bool GetResult() => _boolVar;

        public BoolAwaiter(bool pBoolVar)
        {
            _boolVar = pBoolVar;
        }
    }
}
