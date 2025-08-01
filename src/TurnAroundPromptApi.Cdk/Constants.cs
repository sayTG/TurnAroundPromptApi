namespace TurnAroundPromptApi.Cdk
{
    public static class Constants
    {
        // API Configuration
        public const string ApiName = "TurnAroundPromptApi";
        public const string ApiDescription = "REST API for managing turnaround prompts with DynamoDB integration";
        
        // DynamoDB Configuration
        public const string TableName = "TurnaroundPromptTable";
        public const string PartitionKeyName = "Id";
        
        // API Keys
        public const string ReadOnlyApiKeyName = "ReadOnlyKey";
        public const string ReadOnlyApiKeyValue = "readonly-key";
        public const string AdminApiKeyName = "AdminKey";
        public const string AdminApiKeyValue = "admin-key";
        
        // Usage Plan
        public const string UsagePlanName = "BasicPlan";
        public const string UsagePlanDescription = "Basic usage plan for TurnAroundPrompt API";
        
        // Role
        public const string DynamoDbRoleName = "DynamoDBRole";
        
        // Resource Names
        public const string GetMethodName = "GetTurnAroundPrompt";
        public const string PutMethodName = "CreateTurnAroundPrompt";
        public const string PatchMethodName = "UpdateTurnAroundPrompt";
        public const string DeleteMethodName = "DeleteTurnAroundPrompt";
        
        // Endpoint Paths
        public const string BasePath = "turnaroundprompt";
        public const string IdPath = "{id}";
        
        // Content Types
        public const string JsonContentType = "application/json";
        
        // HTTP Status Codes
        public const string Status200 = "200";
        public const string Status201 = "201";
        public const string Status400 = "400";
        public const string Status404 = "404";
        public const string Status500 = "500";
    }
} 