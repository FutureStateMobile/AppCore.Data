using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using FutureState.AppCore.Data.Tests.Helpers.Fixtures;
using FutureState.AppCore.Data.Tests.Helpers.Models;
using NUnit.Framework;

namespace FutureState.AppCore.Data.Tests.Integration
{
    [TestFixture]
    public class InflectionTests : IntegrationTestBase
    {
        [Test, TestCaseSource(nameof(DbProviders))]
        public void Should_Do_Crud_On_Model_Object_With_Inflected_Pluralization(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));
            
            // Execute Create
            var firstGoose = GooseFixture.GetFirstGoose();
            var gooseToUpdate = GooseFixture.GetGooseToUpdate();
            var gooseToDelete = GooseFixture.GetGooseToDelete();

            db.Create(firstGoose);
            db.Create(gooseToUpdate);
            db.Create(gooseToDelete);
            var actualGoose = db.Query<GooseModel>().Where(u => u.Id == firstGoose.Id).Select().FirstOrDefault();
            
            // Assert Create
            actualGoose.Should().NotBeNull();
            actualGoose.ShouldBeEquivalentTo(firstGoose);
            
            // Execute Find IEnumerable
            var actualGeese = db.Query<GooseModel>().Where(x => x.Name.Contains("irst")).Select();
            actualGeese.Should().NotBeNullOrEmpty();
            
            // Execute Find List
            var actualGeese2 = db.Query<GooseModel>().Where(x => x.Name.Contains("Goose")).Select().ToList();
            actualGeese2.Should().HaveCount(3);
            
            // Execute Update
            gooseToUpdate.Name = "Canada Goose";
            db.Update(gooseToUpdate);
            
            var actualUpdatedGoose = db.Query<GooseModel>().Where(x => x.Id == gooseToUpdate.Id).Select().FirstOrDefault();
            actualUpdatedGoose.Should().NotBeNull();
            actualUpdatedGoose.ShouldBeEquivalentTo(gooseToUpdate);
            
            // Execute Delete
            db.Query<GooseModel>().Where(u => u.Id == gooseToDelete.Id).Delete();
            var actualDeletedGoose = db.Query<GooseModel>().Where(x => x.Id == gooseToDelete.Id).Select().FirstOrDefault();

            actualDeletedGoose.Should().BeNull();
            
            db.Query<GooseModel>().Truncate();
            
            var emptyResults = db.Query<GooseModel>().Select();
            emptyResults.Should().BeEmpty();
        }

    }
}