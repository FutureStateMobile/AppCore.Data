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
    }
}
