namespace UserProfile.Models.Constant;

public static class UserProfileConstants
{
    public static readonly int MigrationCountRetry = 6;
    public static readonly string ServerSecretHeader = "Backend-Secret";
    
    // Захардкоженное значение даты в строковом формате
    public static readonly string FixedDate = new DateTime(2024, 10, 10).ToString("yyyy-MM-dd");
}