// FILE: src/shared/DTOs/User/UserPreferencesDto.cs
namespace DTOs.User;

public class UserPreferencesDto
{
    // Notification preferences
    public bool EmailNotifications { get; set; } = true;
    public bool SmsNotifications { get; set; } = false;
    public bool PushNotifications { get; set; } = true;
    public bool SecurityAlerts { get; set; } = true;

    // Display preferences
    public string Theme { get; set; } = "light";
    public string Language { get; set; } = "en";
    public string TimeZone { get; set; } = "UTC";
    public string DateFormat { get; set; } = "MM/dd/yyyy";
    public string TimeFormat { get; set; } = "12h";

    // Privacy preferences
    public bool ShowOnlineStatus { get; set; } = true;
    public bool AllowProfileDiscovery { get; set; } = true;
    public bool ShareActivityStatus { get; set; } = false;

    // UI preferences
    public string DefaultLandingPage { get; set; } = "dashboard";
    public int ItemsPerPage { get; set; } = 10;
    public bool CompactView { get; set; } = false;
    public bool ShowTooltips { get; set; } = true;

    // Feature preferences
    public bool AutoSave { get; set; } = true;
    public bool EnableKeyboardShortcuts { get; set; } = true;
    public bool ShowAdvancedFeatures { get; set; } = false;
}
