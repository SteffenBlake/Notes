using Microsoft.Extensions.DependencyInjection;
using Notes.Data;

namespace Notes.Business.Tests;

public abstract class TestBase<T1> 
    where T1 : notnull
{
    protected T1? PrimaryService { get; set; }
    protected NotesDbContext? Db { get; set; }

    [SetUp]
    public void Setup()
    {
        Build(Engine.Compile());
    }

    [TearDown]
    public void TearDown()
    {
        Db?.Dispose();
    }

    protected virtual void Build(IServiceProvider services)
    {
        PrimaryService = services.GetRequiredService<T1>();
        Db = services.GetRequiredService<NotesDbContext>();
    }
}

public abstract class TestBase<T1, T2> : TestBase<T1> 
    where T1 : notnull
    where T2 : notnull
{
    protected T2? ServiceA { get; set; }

    protected override void Build(IServiceProvider services)
    {
        base.Build(services);
        ServiceA = services.GetRequiredService<T2>();
    }
}

public abstract class TestBase<T1, T2, T3> : TestBase<T1, T2>
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
{
    protected T3? ServiceB { get; set; }

    protected override void Build(IServiceProvider services)
    {
        base.Build(services);
        ServiceB = services.GetRequiredService<T3>();
    }
}

public abstract class TestBase<T1, T2, T3, T4> : TestBase<T1, T2, T3>
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
    where T4 : notnull
{
    protected T4? ServiceC { get; set; }

    protected override void Build(IServiceProvider services)
    {
        base.Build(services);
        ServiceC = services.GetRequiredService<T4>();
    }
}