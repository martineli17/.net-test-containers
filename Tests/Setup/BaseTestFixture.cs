using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tests
{
    public class BaseTestFixture : WebApplicationFactory<Program>, IDisposable
    {
        public readonly HttpClient HttpClient;
        private TestcontainersContainer _redisContainer;

        public BaseTestFixture()
        {
            HttpClient = CreateClient();
        }
      
        public async Task StartRedisContainerAsync()
        {
            _redisContainer = new TestcontainersBuilder<TestcontainersContainer>()
                                           .WithImage("redis:latest")
                                           .WithExposedPort(6379)
                                           .WithPortBinding(6378, 6379)
                                           .WithName("redis-test")
                                           .WithHostname("redis-test")
                                           .WithEnvironment("REDIS_PASSWORD", "test123")
                                           .Build();

            await _redisContainer.StartAsync();
        }

        public new void Dispose()
        {
            base.Dispose();
            _redisContainer?.StopAsync().Wait();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
        }
    }
}