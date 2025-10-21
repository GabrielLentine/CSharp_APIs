using CatagoloAPI.Context;

namespace CatagoloAPI.Repositories;

public class UnityOfWork : IUnityOfWork
{
    private ICategoriaRepository? _categoriaRepo;
    private IProdutoRepository? _produtoRepo;
    public AppDbContext Context;

    public UnityOfWork(AppDbContext context)
    {
        Context = context;
    }

    public IProdutoRepository ProdutoRepository
    {
        // se eu não tiver uma instância de ProdutoRepository, eu crio; se tiver, eu uso a que já tem
        get { return _produtoRepo = _produtoRepo ?? new ProdutoRepository(Context); }

        // if(_produtoRepo == null) _produtoRepo = new ProdutoRepository(Context);
        // else return _produtoRepo;
    }

    public ICategoriaRepository CategoriaRepository
    {
        // se eu não tiver uma instância de CategoriaRepository, eu crio; se tiver, eu uso a que já tem
        get { return _categoriaRepo = _categoriaRepo ?? new CategoriaRepository(Context); }

        // if(_categoriaRepo == null) _categoriaRepo = new CategoriaRepository(Context);
        // else return _categoriaRepo;
    }

    public async Task CommitAsync()
    {
        await Context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await Context.DisposeAsync();
    }
}