using Microsoft.AspNetCore.Http;

namespace TurnAroundPromptApi.Api.Services
{
    public interface IApiKeyAuthenticationService
    {
        bool IsValidApiKey(string? apiKey);
        bool IsAdminKey(string? apiKey);
        bool IsReadOnlyKey(string? apiKey);
    }

    public class ApiKeyAuthenticationService : IApiKeyAuthenticationService
    {
        private const string ReadOnlyKey = "readonly-key";
        private const string AdminKey = "admin-key";

        public bool IsValidApiKey(string? apiKey)
        {
            return IsReadOnlyKey(apiKey) || IsAdminKey(apiKey);
        }

        public bool IsAdminKey(string? apiKey)
        {
            return !string.IsNullOrEmpty(apiKey) && apiKey.Equals(AdminKey, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsReadOnlyKey(string? apiKey)
        {
            return !string.IsNullOrEmpty(apiKey) && apiKey.Equals(ReadOnlyKey, StringComparison.OrdinalIgnoreCase);
        }
    }

    public static class ApiKeyAuthenticationExtensions
    {
        public static string? GetApiKey(this HttpContext context)
        {
            return context.Request.Headers["x-api-key"].FirstOrDefault();
        }
    }
} 