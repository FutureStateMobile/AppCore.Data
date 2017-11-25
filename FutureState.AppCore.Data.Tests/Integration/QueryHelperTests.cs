using System;
using FluentAssertions;
using FutureState.AppCore.Data.Tests.Helpers.Models;
using NUnit.Framework;

namespace FutureState.AppCore.Data.Tests.Integration
{
    [TestFixture]
    public class QueryHelperTests : IntegrationTestBase
    {
        [Test, TestCaseSource(nameof(DbProviders))]
        public void Should_Throw_When_First_Is_Empty(IDbProvider db)
        {
            // execute
            Action method = () => db.Query<GooseModel>().First();

            // assert
            method.ShouldThrow<InvalidOperationException>();
        }
        [Test, TestCaseSource(nameof(DbProviders))]
        public void Should_Return_Null_When_FirstOrDefault_Is_Empty(IDbProvider db)
        {
            // execute
            var result = db.Query<GooseModel>().FirstOrDefault();

            // assert
            result.Should().BeNull();
        }

        [Test, TestCaseSource(nameof(DbProviders))]
        public void Should_Throw_When_Single_Is_Empty(IDbProvider db)
        {
            // execute
            Action method = () => db.Query<GooseModel>().Single();

            // assert
            method.ShouldThrow<InvalidOperationException>();
        }
        [Test, TestCaseSource(nameof(DbProviders))]
        public void Should_Return_Null_When_SingleOrDefault_Is_Empty(IDbProvider db)
        {
            // execute
            var result = db.Query<GooseModel>().SingleOrDefault();

            // assert
            result.Should().BeNull();
        }

        [Test, TestCaseSource(nameof(DbProviders))]
        public void Should_Throw_When_Last_Is_Empty(IDbProvider db)
        {
            // execute
            Action method = () => db.Query<GooseModel>().Last();

            // assert
            method.ShouldThrow<InvalidOperationException>();
        }

        [Test, TestCaseSource(nameof(DbProviders))]
        public void Should_Return_Null_When_LastOrDefault_Is_Empty(IDbProvider db)
        {
            // execute
            var result = db.Query<GooseModel>().LastOrDefault();

            // assert
            result.Should().BeNull();
        }

        [Test, TestCaseSource(nameof(DbProviders))]
        public void Should_Return_Zero_Records_When_ToList_Is_Empty(IDbProvider db)
        {
            // execute
            var result = db.Query<GooseModel>().ToList();

            // assert
            result.Count.Should().Be(0);
        }
    }
}