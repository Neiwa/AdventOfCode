using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class MemoizedSolver<TState, TResult>
    {
        private readonly Dictionary<TState, TResult> _memoized = [];

        public TResult Solve(Func<Func<TState, TResult>, TState, TResult> function, TState initialState)
        {
            TResult callback(TState state)
            {
                return solve(function, state);
            }

            TResult solve(Func<Func<TState, TResult>, TState, TResult> function, TState state)
            {
                if (_memoized.TryGetValue(state, out var memoResult))
                {
                    return memoResult;
                }

                var result = function(callback, state);
                _memoized[state] = result;
                return result;
            }

            return solve(function, initialState);
        }
    }
}
