using Microsoft.AspNetCore.Http;

namespace TurnAroundPromptApi.Services.Extensions
{
    public static class ApiKeyAuthenticationExtensions
    {
        public static string GetApiKey(this HttpContext context)
        {
            return context.Request.Headers["x-api-key"].FirstOrDefault();
        }
    }
}
