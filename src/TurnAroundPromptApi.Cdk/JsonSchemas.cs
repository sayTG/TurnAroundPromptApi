using Amazon.CDK.AWS.APIGateway;

namespace TurnAroundPromptApi.Cdk
{
    public static class JsonSchemas
    {
        // Schema for TurnAroundPrompt entity
        public static JsonSchema TurnAroundPromptSchema => new JsonSchema
        {
            Type = JsonSchemaType.OBJECT,
            Properties = new Dictionary<string, IJsonSchema>
            {
                ["id"] = new JsonSchema
                {
                    Type = JsonSchemaType.STRING,
                    Pattern = "^TAP-\\d+$",
                    Description = "Unique identifier for the turnaround prompt"
                },
                ["name"] = new JsonSchema
                {
                    Type = JsonSchemaType.STRING,
                    MinLength = 1,
                    MaxLength = 255,
                    Description = "Name of the turnaround prompt"
                },
                ["status"] = new JsonSchema
                {
                    Type = JsonSchemaType.STRING,
                    Enum = new[] { "active", "inactive", "pending", "completed" },
                    Description = "Status of the turnaround prompt"
                },
                ["deleted"] = new JsonSchema
                {
                    Type = JsonSchemaType.BOOLEAN,
                    Default = false,
                    Description = "Soft delete flag"
                }
            },
            Required = new[] { "id", "name", "status" },
            AdditionalProperties = false
        };

        //Schema for creating a new TurnAroundPrompt (PUT request)
        public static JsonSchema CreateTurnAroundPromptSchema => new JsonSchema
        {
            Type = JsonSchemaType.OBJECT,
            Properties = new Dictionary<string, IJsonSchema>
            {
                ["id"] = new JsonSchema
                {
                    Type = JsonSchemaType.STRING,
                    Pattern = "^TAP-\\d+$",
                    Description = "Unique identifier for the turnaround prompt"
                },
                ["name"] = new JsonSchema
                {
                    Type = JsonSchemaType.STRING,
                    MinLength = 1,
                    MaxLength = 255,
                    Description = "Name of the turnaround prompt"
                },
                ["status"] = new JsonSchema
                {
                    Type = JsonSchemaType.STRING,
                    Enum = new[] { "active", "inactive", "pending", "completed" },
                    Description = "Status of the turnaround prompt"
                }
            },
            Required = new[] { "id", "name", "status" },
            AdditionalProperties = false
        };

        // Schema for updating a TurnAroundPrompt (PATCH request)
        public static JsonSchema UpdateTurnAroundPromptSchema => new JsonSchema
        {
            Type = JsonSchemaType.OBJECT,
            Properties = new Dictionary<string, IJsonSchema>
            {
                ["id"] = new JsonSchema
                {
                    Type = JsonSchemaType.STRING,
                    Pattern = "^TAP-\\d+$",
                    Description = "Unique identifier for the turnaround prompt"
                },
                ["name"] = new JsonSchema
                {
                    Type = JsonSchemaType.STRING,
                    MinLength = 1,
                    MaxLength = 255,
                    Description = "Name of the turnaround prompt"
                },
                ["status"] = new JsonSchema
                {
                    Type = JsonSchemaType.STRING,
                    Enum = new[] { "active", "inactive", "pending", "completed" },
                    Description = "Status of the turnaround prompt"
                }
            },
            Required = new[] { "id" },
            AdditionalProperties = false
        };
    }
} 