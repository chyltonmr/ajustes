using Moq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System.Threading;

public class TesteRelatorioValorizacaoFaltanteRepository
{
    [Fact]
    public async Task ObterValorizacaoFaltanteAsync_DeveRetornarDadosSimulados()
    {
        // Arrange: Criar dados simulados
        var dadosSimulados = new List<RelatorioContagemValorizacao>
        {
            new RelatorioContagemValorizacao { /* Defina valores simulados */ },
            new RelatorioContagemValorizacao { /* Defina valores simulados */ }
        }.AsQueryable(); // Converte para IQueryable

        // Criar um mock de DbSet<RelatorioContagemValorizacao>
        var mockDbSet = new Mock<DbSet<RelatorioContagemValorizacao>>();

        // Configuração do IQueryable para simular Entity Framework
        mockDbSet.As<IQueryable<RelatorioContagemValorizacao>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<RelatorioContagemValorizacao>(dadosSimulados.Provider));

        mockDbSet.As<IQueryable<RelatorioContagemValorizacao>>()
            .Setup(m => m.Expression).Returns(dadosSimulados.Expression);
        mockDbSet.As<IQueryable<RelatorioContagemValorizacao>>()
            .Setup(m => m.ElementType).Returns(dadosSimulados.ElementType);
        mockDbSet.As<IQueryable<RelatorioContagemValorizacao>>()
            .Setup(m => m.GetEnumerator()).Returns(() => dadosSimulados.GetEnumerator());

        // Mock de IAsyncEnumerable para suportar chamadas assíncronas como ToListAsync()
        mockDbSet.As<IAsyncEnumerable<RelatorioContagemValorizacao>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<RelatorioContagemValorizacao>(dadosSimulados.GetEnumerator()));

        // Mock de FromSqlRaw() para retornar o mockado DbSet
        mockDbSet
            .Setup(m => m.FromSqlRaw(It.IsAny<string>(), It.IsAny<object[]>()))
            .Returns(mockDbSet.Object);

        // Criando mock do contexto e configurando o DbSet
        var mockContext = new Mock<RelatorioContext>();
        mockContext.Setup(c => c.Set<RelatorioContagemValorizacao>()).Returns(mockDbSet.Object);

        // Mockando a função que retorna a string SQL (caso esteja sendo usada no código real)
        var queryMock = "SELECT * FROM RelatorioContagemValorizacao";
        var mockRepository = new Mock<RelatorioValorizacaoFaltanteRepository>(mockContext.Object, /* mock de config */);
        mockRepository.Setup(repo => repo.ComAConsulta(It.IsAny<string>())).Returns(queryMock);

        // Criar instância real do repositório injetando o mock correto
        var repository = mockRepository.Object;

        // Act: Chamar o método que estamos testando
        var resultado = await repository.ObterValorizacaoFaltanteAsync(new Filtro());

        // Assert: Verificar se os dados simulados são retornados corretamente
        Assert.NotNull(resultado);
        Assert.Equal(dadosSimulados.Count(), resultado.Count);
    }
}
