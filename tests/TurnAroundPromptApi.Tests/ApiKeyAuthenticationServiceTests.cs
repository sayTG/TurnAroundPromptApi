using TurnAroundPromptApi.Api.Services;
using Xunit;

namespace TurnAroundPromptApi.Tests
{
    public class ApiKeyAuthenticationServiceTests
    {
        private readonly IApiKeyAuthenticationService _authService;

        public ApiKeyAuthenticationServiceTests()
        {
            _authService = new ApiKeyAuthenticationService();
        }

        [Fact]
        public void IsValidApiKey_WithReadOnlyKey_ReturnsTrue()
        {
            // Arrange
            var apiKey = "readonly-key";

            // Act
            var result = _authService.IsValidApiKey(apiKey);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidApiKey_WithAdminKey_ReturnsTrue()
        {
            // Arrange
            var apiKey = "admin-key";

            // Act
            var result = _authService.IsValidApiKey(apiKey);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidApiKey_WithInvalidKey_ReturnsFalse()
        {
            // Arrange
            var apiKey = "invalid-key";

            // Act
            var result = _authService.IsValidApiKey(apiKey);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidApiKey_WithNullKey_ReturnsFalse()
        {
            // Act
            var result = _authService.IsValidApiKey(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsAdminKey_WithAdminKey_ReturnsTrue()
        {
            // Arrange
            var apiKey = "admin-key";

            // Act
            var result = _authService.IsAdminKey(apiKey);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsAdminKey_WithReadOnlyKey_ReturnsFalse()
        {
            // Arrange
            var apiKey = "readonly-key";

            // Act
            var result = _authService.IsAdminKey(apiKey);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsReadOnlyKey_WithReadOnlyKey_ReturnsTrue()
        {
            // Arrange
            var apiKey = "readonly-key";

            // Act
            var result = _authService.IsReadOnlyKey(apiKey);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsReadOnlyKey_WithAdminKey_ReturnsFalse()
        {
            // Arrange
            var apiKey = "admin-key";

            // Act
            var result = _authService.IsReadOnlyKey(apiKey);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("ADMIN-KEY")]
        [InlineData("Admin-Key")]
        [InlineData("ADMIN-KEY")]
        public void IsAdminKey_WithCaseInsensitiveAdminKey_ReturnsTrue(string apiKey)
        {
            // Act
            var result = _authService.IsAdminKey(apiKey);

            // Assert
            Assert.True(result);
        }
    }
} 