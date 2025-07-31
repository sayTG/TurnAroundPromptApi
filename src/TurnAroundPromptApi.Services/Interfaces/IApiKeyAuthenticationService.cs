namespace TurnAroundPromptApi.Services.Interfaces
{
    public interface IApiKeyAuthenticationService
    {
        bool IsValidApiKey(string apiKey);
        bool IsAdminKey(string apiKey);
        bool IsReadOnlyKey(string apiKey);
    }
}
