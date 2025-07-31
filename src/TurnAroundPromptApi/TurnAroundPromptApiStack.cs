using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Constructs;

namespace TurnAroundPromptApi
{
    public class TurnAroundPromptApiStack : Stack
    {
        internal TurnAroundPromptApiStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // 1. DynamoDB Table
            var table = new Table(this, "TurnaroundPromptTable", new TableProps
            {
                PartitionKey = new Attribute { Name = "id", Type = AttributeType.STRING },
                BillingMode = BillingMode.PAY_PER_REQUEST,
                RemovalPolicy = RemovalPolicy.DESTROY
            });

            // 2. API Gateway
            var api = new RestApi(this, "TurnaroundPromptApi", new RestApiProps
            {
                RestApiName = "TurnaroundPrompt Service",
                DeployOptions = new StageOptions { StageName = "prod" }
            });

            // 3. API Keys
            var readOnlyKey = api.AddApiKey("ReadOnlyKey", new ApiKeyOptions
            {
                ApiKeyName = "apiKey1",
                Value = "readonly-key"
            });

            var adminKey = api.AddApiKey("AdminKey", new ApiKeyOptions
            {
                ApiKeyName = "apiKey2",
                Value = "admin-key"
            });

            var usagePlan = api.AddUsagePlan("UsagePlan", new UsagePlanProps
            {
                Name = "BasicUsagePlan"
            });

            usagePlan.AddApiKey(readOnlyKey);
            usagePlan.AddApiKey(adminKey);
            usagePlan.AddApiStage(new UsagePlanPerApiStage
            {
                Api = api,
                Stage = api.DeploymentStage
            });

            // 4. IAM Role
            var dynamoRole = new Role(this, "DynamoRole", new RoleProps
            {
                AssumedBy = new ServicePrincipal("apigateway.amazonaws.com")
            });

            table.GrantReadWriteData(dynamoRole);

            // 5. Define API Endpoints
            var prompt = api.Root.AddResource("turnaroundprompt");
            var promptById = prompt.AddResource("{id}");

            // Next: Add integrations (GET, PUT, PATCH, DELETE)
        }
    }
}
