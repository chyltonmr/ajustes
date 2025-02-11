using Moq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class TesteRelatorioValorizacaoFaltanteRepository
{
    [Fact]
    public async Task ObterValorizacaoFaltanteAsync_DeveRetornarDadosSimulados()
    {
        // 🔹 Arrange: Criar dados simulados para retorno
        var dadosSimulados = new List<RelatorioContagemValorizacao>
        {
            new RelatorioContagemValorizacao { /* Defina dados simulados */ },
            new RelatorioContagemValorizacao { /* Defina dados simulados */ }
        }.AsQueryable();

        // 🔹 Criar um Mock de DbSet<RelatorioContagemValorizacao>
        var mockDbSet = new Mock<DbSet<RelatorioContagemValorizacao>>();

        // 🔹 Configurar IQueryable para simular Entity Framework
        mockDbSet.As<IQueryable<RelatorioContagemValorizacao>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<RelatorioContagemValorizacao>(dadosSimulados.Provider));

        mockDbSet.As<IQueryable<RelatorioContagemValorizacao>>()
            .Setup(m => m.Expression).Returns(dadosSimulados.Expression);
        mockDbSet.As<IQueryable<RelatorioContagemValorizacao>>()
            .Setup(m => m.ElementType).Returns(dadosSimulados.ElementType);
        mockDbSet.As<IQueryable<RelatorioContagemValorizacao>>()
            .Setup(m => m.GetEnumerator()).Returns(() => dadosSimulados.GetEnumerator());

        // 🔹 Criar suporte para consultas assíncronas (ToListAsync)
        mockDbSet.As<IAsyncEnumerable<RelatorioContagemValorizacao>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<RelatorioContagemValorizacao>(dadosSimulados.GetEnumerator()));

        // 🔹 Configurar o retorno do FromSqlRaw<T>
        mockDbSet.Setup(m => m.Provider.Execute(It.IsAny<System.Linq.Expressions.Expression>()))
            .Returns(dadosSimulados.FirstOrDefault());

        // 🔹 Mock do contexto para retornar o DbSet correto
        var mockContext = new Mock<RelatorioContext>();
        mockContext.Setup(c => c.Set<RelatorioContagemValorizacao>()).Returns(mockDbSet.Object);

        // 🔹 Mock do método `ComAConsulta` (agora é público)
        var mockRepository = new Mock<RelatorioValorizacaoFaltanteRepository>(mockContext.Object, /* mock de config */);
        mockRepository.Setup(repo => repo.ComAConsulta(It.IsAny<string>()))
                      .Returns("SELECT * FROM RelatorioContagemValorizacao"); // Retorna uma query válida

        // 🔹 Criar instância real do repositório
        var repository = mockRepository.Object;

        // 🔹 Act: Chamar o método que estamos testando
        var resultado = await repository.ObterValorizacaoFaltanteAsync(new Filtro());

        // 🔹 Assert: Verificar se os dados simulados são retornados corretamente
        Assert.NotNull(resultado);
        Assert.Equal(dadosSimulados.Count(), resultado.Count);
    }
}
