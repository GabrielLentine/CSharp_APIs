namespace CatagoloAPI.Pagination;
public abstract class QueryStringParameters
{
    // max page size = tamanho máximo da página (evita sobrecarregar o servidor), por padrão vem como 50
    const int MAX_PAGE_SIZE = 50;

    // page size = quatidade de registros por página
    private int _pageSize = MAX_PAGE_SIZE;

    // valor da página a ser recuperada, por padrão vem como 1
    public int PageNumber { get; set; } = 1;
    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = (value > MAX_PAGE_SIZE) ? MAX_PAGE_SIZE : value;
        }
    }
}
