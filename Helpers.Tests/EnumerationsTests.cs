namespace Helpers.Tests
{
    public class EnumerationsTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [TestCase("", "")]
        [TestCase("a", ",a")]
        [TestCase("ab", ",a,b,ba")]
        [TestCase("abc", ",a,b,c,cb,ca,ba,cba")]
        [TestCase("abcd", ",a,b,c,d,dc,db,da,cb,ca,ba,dcb,dca,dba,cba,dcba")]
        public void GetAllSubSets_GetsCorrectSubSets(string value, string expected)
        {
            // Arrange
            var set = value.ToCharArray();

            var subSets = expected.Split(',').Select(s => s.ToCharArray().ToHashSet());

            // Act
            var actual = Enumerations.GetAllSubSets(set);

            // Assert
            Assert.That(actual, Is.EquivalentTo(subSets));
        }
    }
}