namespace DTOs.Auth;

public class PermissionDto
{
    public int Id { get; set; } // ADD: To match your Permission entity
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true; // ADD: To match your Permission entity
}

public class PermissionCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public List<PermissionDto> Permissions { get; set; } = new();
}
