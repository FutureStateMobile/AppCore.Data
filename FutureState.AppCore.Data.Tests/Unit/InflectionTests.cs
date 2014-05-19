using FutureState.AppCore.Data.Helpers;
using NUnit.Framework;

namespace FutureState.AppCore.Data.Tests.Unit
{
    [TestFixture]
    public class InflectionTests
    {
        [Test]
        public void ShouldPluralizeCorrectly()
        {
            var result = "Goose".Pluralize();
            Assert.That(result, Is.EqualTo("Geese"));
        }
        [Test]
        public void ShouldSingularizeCorrectly()
        {
            var result = "Geese".Singularize();
            Assert.That(result, Is.EqualTo("Goose"));
        }

        [Test]
        public void ShouldPluralizeJoinedMedia()
        {
            var result = "UserMedia".Pluralize();
            Assert.That(result, Is.EqualTo("UserMedia"));
        }

        [Test]
        public void ShouldSingularizeJoinedMedia()
        {
            var result = "UserMedia".Singularize();
            Assert.That(result, Is.EqualTo("UserMedia")); 
        }
    }
}
