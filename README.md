# Utilizando containers em testes
Este repositório tem como objetivo exemplificar, de maneira simples, a utilização de containers para a execução de testes.

## Pacotes a serem instalados
- [Testcontainers](https://www.testcontainers.org/): O pacote responsável por fornecer a interface que permite, de maneira simples, configurar containers para serem utilizados em testes. 

## Cenário de uso
<p>Supondo que você tem uma aplicação que realiza operações que inserem/consultam registros em um banco de dados. Quando ocorre a implementação testes, por exemplo de integração, será necessário instanciar um host específico para isso e que esteja em execução somente durante a execução dos testes.</p>

## Exemplo no código
### [BaseTestFixture](https://github.com/martineli17/.net-test-containers/blob/master/Tests/Setup/BaseTestFixture.cs)
Nesta fixture, definimos e iniciamos o nosso servidor de testes. Dentre as configurações definidas, temos um método que é responsável por inicar um container do Redis para que os nossos testes sejam executados e validados corretamente.
Neste método, inserimos algumas informações básicas para o nosso container, como:
- WithImage: nome da imagem
- WithExposedPort: porta que o container expõe
- WithPortBinding: porta local (6378) para fazer o binding com a porta do container (6379)
- WithName: nome do container
- WithHostname: nome do host do container
- WithEnvironment: variáveis de ambiente

```cs
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
```
E, ainda na classe de Fixture, temos a chamada dos métodos para parar e excluir o container quando os testes foram finalizados. Essa chamada se encontra no método Dispose.
```cs
 public new void Dispose()
{
    base.Dispose();

    _redisContainer?.StopAsync().Wait();
    _redisContainer?.CleanUpAsync().Wait();
}
```

### [Classe de teste](https://github.com/martineli17/.net-test-containers/blob/master/Tests/RedisControllerTest.cs)
Na classe de teste, existe a referência da fixture que desejamos utilizar. Assim, podemos iniciar o container necessário para executar os testes.

```cs
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
    await _fixture.StartRedisContainerAsync();

    // Act
    var response = await _fixture.HttpClient.PostAsync("/redis", content);

    // Assert
    var dataSaved = await redisContext.GetAsync<object>(key);
    response.EnsureSuccessStatusCode();
    Assert.NotNull(dataSaved);
}
```

## Mais informações
Para mais informações, possibilidades de uso e especificações técnicas, acesse a página do [Testcontainers](https://www.testcontainers.org/)
