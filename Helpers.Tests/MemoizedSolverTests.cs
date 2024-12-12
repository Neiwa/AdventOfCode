using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Tests
{
    public class MemoizedSolverTests
    {

        public record State(int Number);

        public record Result(int Number);
        [Test]
        public void Solve_Fibonacci()
        {
            var solver = new MemoizedSolver<State, Result>();

            var result = solver.Solve((callback, state) =>
            {
                if (state.Number == 0)
                {
                    return new Result(0);
                }
                if (state.Number == 1)
                {
                    return new Result(1);
                }

                return new Result(callback(new State(state.Number - 2)).Number + callback(new State(state.Number - 1)).Number);

            }, new State(6));
            
            Assert.That(result.Number, Is.EqualTo(8));
        }

        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 1)]
        [TestCase(3, 2)]
        [TestCase(6, 8)]
        [TestCase(10, 55)]
        [TestCase(15, 610)]
        [TestCase(50, 12586269025)]
        [TestCase(90, 2880067194370816120)]
        public void Solve_Fibonacci2(long input, long expected)
        {
            var solver = new MemoizedSolver<long, long>();

            var result = solver.Solve((callback, state) =>
            {
                if (state == 0)
                {
                    return 0;
                }
                if (state == 1)
                {
                    return 1;
                }

                return callback(state - 2) + callback(state - 1);

            }, input);

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 1)]
        [TestCase(3, 2)]
        [TestCase(6, 8)]
        [TestCase(10, 55)]
        [TestCase(15, 610)]
        [TestCase(30, 832040)]
        public void Solve_Fibonacci3(long input, long expected)
        {
            var solver = new MemoizedSolver<long, long>();

            long fib (long state)
            {
                if (state == 0)
                {
                    return 0;
                }
                if (state == 1)
                {
                    return 1;
                }

                return fib(state - 2) + fib(state - 1);

            }

            Assert.That(fib(input), Is.EqualTo(expected));
        }
    }
}
