using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Grid;

namespace Helpers.Tests
{
    [TestFixture]
    public class GridTests
    {
        [Test]
        public void Test1()
        {
            // Arrange
            string[] lines =
            {
                "abc",
                "def",
                "ghi"
            };
            var grid = lines.ToGrid();

            // Act
            var actual = grid[0][0].Value;

            // Assert
            Assert.That(actual, Is.EqualTo('a'));
        }
    }
}
