using System;

namespace Helpers.Tests
{
    [TestFixture]
    public class RectangleTests
    {
        [TestCase(0, 0, 0, 0, 1, 1)]
        [TestCase(0, 0, 1, 1, 2, 2)]
        [TestCase(1, 1, 2, 2, 2, 2)]
        [TestCase(0, 0, 5, 0, 6, 1)]
        [TestCase(0, 0, 0, 5, 1, 6)]
        public void Constructor_TwoPoints(int firstX, int firstY, int bottomRightX, int bottomRightY, int expectedWidth, int expectedHeight)
        {
            // Arrange
            Point firstPoint = new Point(firstX, firstY);
            Point bottomRightPoint = new Point(bottomRightX, bottomRightY);

            // Act
            var actual = new Rectangle(firstPoint, bottomRightPoint);

            // Assert
            Assert.That(actual.Width, Is.EqualTo(expectedWidth));
            Assert.That(actual.Height, Is.EqualTo(expectedHeight));
        }
    }
}