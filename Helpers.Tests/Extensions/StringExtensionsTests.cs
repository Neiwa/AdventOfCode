using Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Tests.Extensions
{
    public class StringExtensionsTests
    {
        [Test]
        public void Substrings_ReturnsAllSubstrings()
        {
            string input = "abc";
            List<string> expected = [
                "a",
                "ab",
                "b",
                "bc",
                "c"
                ];

            var actual = input.Substrings();

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void Slicings_ReturnsAllSlicings()
        {
            string input = "abcd";
            List<List<string>> expected = [
                ["a", "bcd"],
                ["a", "b", "cd"],
                ["a", "b", "c", "d"],
                ["a", "bc", "d"],
                ["ab", "cd"],
                ["ab", "c", "d"],
                ["abc", "d"],
                ];

            var actual = input.Slicings();

            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}
