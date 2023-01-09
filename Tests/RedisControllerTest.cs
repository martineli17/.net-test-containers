using App.Controllers;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    [Collection(nameof(BaseTestFixture))]
    public class RedisControllerTest
    {
        private readonly BaseTestFixture _fixture;

        public RedisControllerTest(BaseTestFixture fixture)
        {
            _fixture = fixture;
            _fixture.StartRedisContainerAsync().Wait();
        }

        [Fact(DisplayName = "RedisController - Add | Should Add A New Item")]
        public async Task RedisController_Add_ShouldAddANewItem()
        {
            // Arrange
            var redisContext = (IRedisContext)_fixture.Services.GetService(typeof(IRedisContext));
            var key = Guid.NewGuid().ToString();
            var request = new RedisAddDto
            {
                Content = new { Name = "Test 01" },
                Key = key
            };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            // Act
            var response = await _fixture.HttpClient.PostAsync("/redis", content);

            // Assert
            var dataSaved = await redisContext.GetAsync<object>(key);
            response.EnsureSuccessStatusCode();
            Assert.NotNull(dataSaved);
        }
    }
}