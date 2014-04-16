using System.Collections.Generic;
using System.Linq;
using FutureState.AppCore.Data.Extensions;
using NUnit.Framework;

namespace FutureState.AppCore.Data.Tests.Unit.Extensions
{
    [TestFixture]
    public class PropertyEqualityComparerTests
    {
        [SetUp]
        public void SetUp()
        {
            _oldList = new List<Foo>
                {
                    new Foo {Bar = "bar one"},
                    new Foo {Bar = "bar two"},
                    new Foo {Bar = "bar three"},
                    new Foo {Bar = "bar four"}
                };

            _newList = new List<Foo>
                {
                    new Foo {Bar = "bar two"},
                    new Foo {Bar = "bar three"},
                    new Foo {Bar = "bar four"},
                    new Foo {Bar = "bar five"}
                };
        }

        public class Foo
        {
            public string Bar { get; set; }
            public bool IsDeleted { get; set; }
        }

        private List<Foo> _oldList;
        private List<Foo> _newList;


        [Test]
        public void ShouldExcludeNewListFromOldList()
        {
            // Setup
            const string expectedBar = "bar one";

            // Execute
            IEnumerable<Foo> updatedFoo = _oldList.Exclude(_newList, foo => foo.Bar);

            // Assert
            Assert.AreEqual(updatedFoo.FirstOrDefault().Bar, expectedBar);
        }


        [Test]
        public void ShouldExcludeOldListFromNewList()
        {
            // Setup
            const string expectedBar = "bar five";

            // Execute
            IEnumerable<Foo> updatedFoo = _newList.Exclude(_oldList, foo => foo.Bar);

            // Assert
            Assert.AreEqual(updatedFoo.FirstOrDefault().Bar, expectedBar);
        }

        [Test]
        public void ShouldNotExludeAnythingWhenComparingAListToAnEmptyList()
        {
            // Setup
            var expectedList = new List<Foo>
                {
                    new Foo {Bar = "bar two"},
                    new Foo {Bar = "bar three"},
                    new Foo {Bar = "bar four"},
                    new Foo {Bar = "bar five"}
                };

            // Execute
            IEnumerable<Foo> actualList = expectedList.Exclude(new List<Foo>(), foo => foo.Bar);

            // Assert
            Assert.AreEqual(expectedList.Count, actualList.Count());
        }
    }
}