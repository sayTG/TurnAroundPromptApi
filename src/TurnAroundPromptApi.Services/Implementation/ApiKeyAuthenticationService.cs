using TurnAroundPromptApi.Services.Interfaces;

namespace TurnAroundPromptApi.Services.Implementation
{
    public class ApiKeyAuthenticationService : IApiKeyAuthenticationService
    {
        private const string ReadOnlyKey = "readonly-key";
        private const string AdminKey = "admin-key";

        public bool IsValidApiKey(string apiKey)
        {
            return IsReadOnlyKey(apiKey) || IsAdminKey(apiKey);
        }

        public bool IsAdminKey(string apiKey)
        {
            return !string.IsNullOrEmpty(apiKey) && apiKey.Equals(AdminKey, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsReadOnlyKey(string apiKey)
        {
            return !string.IsNullOrEmpty(apiKey) && apiKey.Equals(ReadOnlyKey, StringComparison.OrdinalIgnoreCase);
        }
    }
}