namespace MyGardenPatch.Tests.Users.Queries
{
    public class GetLoggedInIdentityQueryTests : TestBase
    {
        [Fact]
        public async Task GetsCurrentLoggedInUser()
        {
            SetCurrentUser(UserTestData.PeterParker);

            var query = new GetLoggedInIdentityQuery();

            var result = await ExecuteQueryAsync(query);

            result!.EmailAddress.Should().Be(UserTestData.PeterParker.EmailAddress);
            result!.Name.Should().Be(UserTestData.PeterParker.Name);
            result!.UserId.Should().Be(UserTestData.PeterParker.Id);
        }

    }
}
