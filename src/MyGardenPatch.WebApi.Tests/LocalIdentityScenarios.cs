namespace MyGardenPatch.WebApi.Tests;

public class LocalIdentityScenarios : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public LocalIdentityScenarios(TestFixture fixture)
    {
        _fixture = fixture;
    }        

    [Fact]
    public async Task CanLoginWithNewIdentity()
    {
        var response = await _fixture
            .WithUser("Peter Parker", "peter.parker@email.com", "password1234!")
            .WithLogin("peter.parker@email.com", "password1234!")
            .Scenario(
                _ => 
                {
                    _.Get.Url("/api/about");

                    _.StatusCodeShouldBeOk();
                });

        dynamic about = response.ReadAsJson<dynamic>();

        ((string)about!.name).Should().Be("peter.parker@email.com");
    }
}
