// src/shared/DTOs/Common/PagedResultDto.cs
namespace DTOs.Common;

public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }  // ✅ Keep existing name
    public int PageSize { get; set; }
    
    // ✅ Keep existing computed property
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    // 🔧 ADD: Alias property for backend services that expect "Page"
    public int Page 
    { 
        get => PageNumber; 
        set => PageNumber = value; 
    }

    public PagedResultDto() { }

    public PagedResultDto(List<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
