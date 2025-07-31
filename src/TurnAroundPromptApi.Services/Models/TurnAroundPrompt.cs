using System.ComponentModel.DataAnnotations;
using Amazon.DynamoDBv2.DataModel;

namespace TurnAroundPromptApi.Services.Models
{
    [DynamoDBTable("TurnaroundPromptTable")]
    public class TurnAroundPrompt
    {
        [DynamoDBHashKey]
        [Required]
        [RegularExpression(@"^TAP-\d+$", ErrorMessage = "ID must match pattern TAP-{number}")]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 255 characters")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(TurnAroundPromptStatus), ErrorMessage = "Status must be one of: active, inactive, pending, completed")]
        public string Status { get; set; } = string.Empty;

        public bool Deleted { get; set; } = false;
    }

    public enum TurnAroundPromptStatus
    {
        active,
        inactive,
        pending,
        completed
    }
} 