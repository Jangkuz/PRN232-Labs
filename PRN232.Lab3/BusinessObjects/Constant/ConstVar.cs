namespace BusinessObjects.Constant;

public static class Policy
{
    public const string AdminOrMod = "AdminOrMod";
    public const string AnyWithToken = "AnyWithToken";
}

public static class Role
{
    public const int Admin = 1;
    public const int Moderator = 2;
    public const int Developer = 3;
    public const int Memeber = 4;
}

public static class ClaimName
{
    public const string Role = "Role";
    public const string AccountId = "AccountId";
}
