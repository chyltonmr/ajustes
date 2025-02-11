Solução Correta
Em vez de tentar mockar ToListAsync(), usaremos a abordagem correta para simular um DbSet<T> com um IQueryable<T>:




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
        // Arrange: Criar dados simulados
        var dadosSimulados = new List<RelatorioContagemValorizacao>
        {
            new RelatorioContagemValorizacao { /* Defina dados simulados */ },
            new RelatorioContagemValorizacao { /* Defina dados simulados */ }
        }.AsQueryable(); // Converte para IQueryable

        // Criar um Mock de DbSet<RelatorioContagemValorizacao>
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

        // Criando mock para suportar chamadas assíncronas como ToListAsync()
        mockDbSet.As<IAsyncEnumerable<RelatorioContagemValorizacao>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<RelatorioContagemValorizacao>(dadosSimulados.GetEnumerator()));

        // Criando mock do contexto e configurando o DbSet
        var mockContext = new Mock<RelatorioContext>();
        mockContext.Setup(c => c.Set<RelatorioContagemValorizacao>()).Returns(mockDbSet.Object);

        // Criar instância do repositório
        var repository = new RelatorioValorizacaoFaltanteRepository(mockContext.Object, /* mock de config */);

        // Act: Chamar o método que estamos testando
        var resultado = await repository.ObterValorizacaoFaltanteAsync(new Filtro());

        // Assert: Verificar se os dados simulados são retornados corretamente
        Assert.NotNull(resultado);
        Assert.Equal(dadosSimulados.Count(), resultado.Count);
    }
}




-----------------------------------------
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;

public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    public TestAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        return new TestAsyncEnumerable<TEntity>(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new TestAsyncEnumerable<TElement>(expression);
    }

    public object Execute(Expression expression)
    {
        return _inner.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return _inner.Execute<TResult>(expression);
    }

    public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
    {
        return new TestAsyncEnumerable<TResult>(expression);
    }

    public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
    {
        return Task.FromResult(_inner.Execute<TResult>(expression));
    }
}

public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }

    public TestAsyncEnumerable(Expression expression) : base(expression) { }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }
}

public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public T Current => _inner.Current;

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(_inner.MoveNext());
    }
}
