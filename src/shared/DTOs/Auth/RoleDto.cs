namespace DTOs.Auth;

public class RoleDto
{
    public int Id { get; set; }
    public int? TenantId { get; set; } // ADD: For tenant isolation (nullable for system roles)
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsSystemRole { get; set; }
    public bool IsDefault { get; set; }
    public int UserCount { get; set; }
    public List<string> Permissions { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateRoleDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDefault { get; set; } // ADD: Missing property
    public List<string> Permissions { get; set; } = new();
}

public class UpdateRoleDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDefault { get; set; } // ADD: Missing property
    public List<string> Permissions { get; set; } = new();
}

public class AssignRoleDto
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
}

// REMOVED: Duplicate PermissionDto (you already have it in PermissionDto.cs)

public class RolePermissionUpdateDto
{
    public List<string> Permissions { get; set; } = new();
}
