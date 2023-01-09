using Xunit;

namespace Tests
{
    [CollectionDefinition(nameof(BaseTestFixture))]
    public class BaseTestCollection : ICollectionFixture<BaseTestFixture> { }
}