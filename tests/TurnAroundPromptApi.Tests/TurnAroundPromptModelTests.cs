using System.ComponentModel.DataAnnotations;
using TurnAroundPromptApi.Services.Models;
using Xunit;

namespace TurnAroundPromptApi.Tests
{
    public class TurnAroundPromptModelTests
    {
        [Fact]
        public void TurnAroundPrompt_WithValidData_IsValid()
        {
            // Arrange
            var prompt = new TurnAroundPrompt
            {
                Id = "TAP-001",
                Name = "Test Prompt",
                Status = "active",
                Deleted = false
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(prompt, new ValidationContext(prompt), validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Theory]
        [InlineData("TAP-001")]
        [InlineData("TAP-123")]
        [InlineData("TAP-999")]
        public void TurnAroundPrompt_WithValidId_IsValid(string id)
        {
            // Arrange
            var prompt = new TurnAroundPrompt
            {
                Id = id,
                Name = "Test Prompt",
                Status = "active"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(prompt, new ValidationContext(prompt), validationResults, true);

            // Assert
            Assert.True(isValid);
        }

        [Theory]
        [InlineData("TAP-")]
        [InlineData("TAP-ABC")]
        [InlineData("TAP-12A")]
        [InlineData("TAP")]
        [InlineData("")]
        [InlineData(null)]
        public void TurnAroundPrompt_WithInvalidId_IsInvalid(string id)
        {
            // Arrange
            var prompt = new TurnAroundPrompt
            {
                Id = id,
                Name = "Test Prompt",
                Status = "active"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(prompt, new ValidationContext(prompt), validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Id"));
        }

        [Theory]
        [InlineData("active")]
        [InlineData("inactive")]
        [InlineData("pending")]
        [InlineData("completed")]
        public void TurnAroundPrompt_WithValidStatus_IsValid(string status)
        {
            // Arrange
            var prompt = new TurnAroundPrompt
            {
                Id = "TAP-001",
                Name = "Test Prompt",
                Status = status
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(prompt, new ValidationContext(prompt), validationResults, true);

            // Assert
            Assert.True(isValid);
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("")]
        [InlineData(null)]
        public void TurnAroundPrompt_WithInvalidStatus_IsInvalid(string status)
        {
            // Arrange
            var prompt = new TurnAroundPrompt
            {
                Id = "TAP-001",
                Name = "Test Prompt",
                Status = status
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(prompt, new ValidationContext(prompt), validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Status"));
        }

        [Theory]
        [InlineData("A")]
        [InlineData("Test Prompt")]
        public void TurnAroundPrompt_WithValidName_IsValid(string name)
        {
            // Arrange
            var prompt = new TurnAroundPrompt
            {
                Id = "TAP-001",
                Name = name,
                Status = "active"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(prompt, new ValidationContext(prompt), validationResults, true);

            // Assert
            Assert.True(isValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("AAAAAAAAAA")]
        public void TurnAroundPrompt_WithInvalidName_IsInvalid(string name)
        {
            // Arrange
            var prompt = new TurnAroundPrompt
            {
                Id = "TAP-001",
                Name = name,
                Status = "active"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(prompt, new ValidationContext(prompt), validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Name"));
        }
    }
} 