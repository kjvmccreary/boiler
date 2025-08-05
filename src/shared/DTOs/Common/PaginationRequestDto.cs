// FILE: src/shared/DTOs/Common/PaginationRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace DTOs.Common;

public class PaginationRequestDto
{
    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public string SortDirection { get; set; } = "asc"; // asc or desc
}
