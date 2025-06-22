namespace Inc.TeamAssistant.Gateway.Auth;

internal static class TelegramAuthProperties
{
    public const string ReturnUrl = "return_url";
    
    public static class Profile
    {
        public const string Id = "id";
        public const string FirstName = "first_name";
        public const string AuthDate = "auth_date";
        public const string Hash = "hash";
        public const string LastName = "last_name";
        public const string Username = "username";
        public const string PhotoUrl = "photo_url";
        public const int Count = 7;
    }
    
    public static Dictionary<string, string> ToFieldSet(
        long id,
        string firstName,
        string? lastName,
        string? username,
        string? photoUrl,
        string authDate,
        string hash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(authDate);
        ArgumentException.ThrowIfNullOrWhiteSpace(hash);
        
        var fields = new Dictionary<string, string>(Profile.Count)
        {
            [Profile.Id] = id.ToString(),
            [Profile.FirstName] = firstName
        };
        
        if (lastName is not null)
            fields.Add(Profile.LastName, lastName);
        if (username is not null)
            fields.Add(Profile.Username, username);
        if (photoUrl is not null)
            fields.Add(Profile.PhotoUrl, photoUrl);
        
        fields.Add(Profile.AuthDate, authDate);
        fields.Add(Profile.Hash, hash);

        return fields;
    }
}