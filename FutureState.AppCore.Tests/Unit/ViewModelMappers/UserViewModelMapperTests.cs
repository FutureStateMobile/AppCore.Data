using FutureState.AppCore.Tests.Integration.Repositories.Fixtures;
using FutureState.AppCore.ViewModelMappers;
using FutureState.AppCore.ViewModels;
using NUnit.Framework;

namespace FutureState.AppCore.Tests.Unit.ViewModelMappers
{
    [TestFixture]
    public class UserViewModelMapperTests
    {
        [Test]
        public void ShouldMapUserModelToUserViewModel()
        {
            // Setup
            IUserViewModelMapper viewModelMapper = new UserViewModelMapper();

            // Execute
            UserViewModel actualViewModel = viewModelMapper.BuildViewModelFrom(UserFixture.FirstUser);

            // Verify
            Assert.AreEqual(UserFixture.FirstUser.Id, actualViewModel.Id);
            Assert.AreEqual(UserFixture.FirstUser.FirstName, actualViewModel.FirstName);
            Assert.AreEqual(UserFixture.FirstUser.LastName, actualViewModel.LastName);
            Assert.AreEqual(UserFixture.FirstUser.Email, actualViewModel.Email);

            // FirstUser both created and updated himself. let's make sure that's correct. 
            Assert.AreEqual(UserFixture.FirstUser.Id, actualViewModel.CreatedBy);
            Assert.AreEqual(UserFixture.FirstUser.Id, actualViewModel.UpdatedBy);
        }
    }
}