namespace CatagoloAPI.Pagination;
public class PagedList<T> : List<T> where T : class
{
    // página atual
    public int CurrentPage { get; set; }

    // total de páginas existentes
    public int TotalPages { get; set; }

    // número de itens exibidos por página
    public int PageSize { get; set; }

    // total de itens no banco de dados
    public int TotalCount { get; set; }

    // página anterior e próxima página
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;

        // Math.Celing arredonda pra cima (Math.Ceiling(4.1) = 5)
        TotalPages = (int)Math.Ceiling(count / (double)pageSize); // pageSize p/ double pra evitar divisão inteira (que truncaria o resultado)
        // isso garante que, se há itens sobrando (como 3 extras na última página), uma nova página será criada para acomodá-los
        // depois, o resultado é convertido de volta para int, pois TotalPages é um inteiro

        AddRange(items);
    }

    // IQueryable é mais eficiente p/ consultas em uma fonte de dados, que pode ser consultado diretamente. Ele suporta consultas diferidas
    // e permite que consultas sejam traduzidas em consultas SQL quando estamos trabalhando com provedor de banco de dados (EF Core).
    public static PagedList<T> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}
