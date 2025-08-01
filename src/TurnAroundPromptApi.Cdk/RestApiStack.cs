using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Logs;
using Constructs;
using TurnAroundPromptApi.Cdk.Components;

namespace TurnAroundPromptApi.Cdk
{
    /// <summary>
    /// Main CDK stack for the TurnAroundPrompt REST API with DynamoDB integration
    /// </summary>
    public class RestApiStack : Stack
    {
        public RestApiStack(Constructs.Construct scope, string id, IStackProps? props = null) : base(scope, id, props)
        {
            // Create DynamoDB table
            var dynamoDbTable = new DynamoDbTable(this, "DynamoDbTable");

            // Create IAM role for API Gateway to access DynamoDB
            var dynamoRole = new Role(this, Constants.DynamoDbRoleName, new RoleProps
            {
                AssumedBy = new ServicePrincipal("apigateway.amazonaws.com"),
                RoleName = Constants.DynamoDbRoleName,
                Description = "Role for API Gateway to access DynamoDB"
            });

            dynamoDbTable.GrantFullAccess(dynamoRole);

            // Create API Gateway
            var api = new RestApi(this, Constants.ApiName, new RestApiProps
            {
                RestApiName = Constants.ApiName,
                Description = Constants.ApiDescription,
                DefaultCorsPreflightOptions = new CorsOptions
                {
                    AllowOrigins = Cors.ALL_ORIGINS,
                    AllowMethods = Cors.ALL_METHODS,
                    AllowHeaders = new[] { "Content-Type", "X-Amz-Date", "Authorization", "X-Api-Key" }
                },
                DeployOptions = new StageOptions
                {
                    StageName = "prod",
                    LoggingLevel = MethodLoggingLevel.INFO,
                    DataTraceEnabled = true,
                    MetricsEnabled = true
                }
            });

            // Create API integrations
            var apiIntegrations = new ApiIntegrations(this, "ApiIntegrations", dynamoRole);

            // Create Request Validators
            var getValidator = new RequestValidator(this, "GetValidator", new RequestValidatorProps
            {
                RestApi = api,
                RequestValidatorName = "GetMethodValidator",
                ValidateRequestParameters = true
            });

            var putValidator = new RequestValidator(this, "PutValidator", new RequestValidatorProps
            {
                RestApi = api,
                RequestValidatorName = "PutMethodValidator",
                ValidateRequestBody = true
            });

            var patchValidator = new RequestValidator(this, "PatchValidator", new RequestValidatorProps
            {
                RestApi = api,
                RequestValidatorName = "PatchMethodValidator",
                ValidateRequestBody = true
            });

            var deleteValidator = new RequestValidator(this, "DeleteValidator", new RequestValidatorProps
            {
                RestApi = api,
                RequestValidatorName = "DeleteMethodValidator",
                ValidateRequestParameters = true
            });

            // Create API keys
            var readOnlyApiKey = api.AddApiKey(Constants.ReadOnlyApiKeyName, new ApiKeyOptions
            {
                ApiKeyName = Constants.ReadOnlyApiKeyName,
                Value = Constants.ReadOnlyApiKeyValue
            });

            var adminApiKey = api.AddApiKey(Constants.AdminApiKeyName, new ApiKeyOptions
            {
                ApiKeyName = Constants.AdminApiKeyName,
                Value = Constants.AdminApiKeyValue
            });

            // Create usage plan
            var usagePlan = api.AddUsagePlan(Constants.UsagePlanName, new UsagePlanProps
            {
                Name = Constants.UsagePlanName,
                Description = Constants.UsagePlanDescription,
                Throttle = new ThrottleSettings
                {
                    RateLimit = 100,
                    BurstLimit = 200
                },
                Quota = new QuotaSettings
                {
                    Limit = 10000,
                    Period = Period.DAY
                }
            });

            // Add API stage to usage plan
            usagePlan.AddApiStage(new UsagePlanPerApiStage
            {
                Api = api,
                Stage = api.DeploymentStage
            });

            // Create resource and methods
            var resource = api.Root.AddResource(Constants.BasePath);
            var resourceWithId = resource.AddResource(Constants.IdPath);

            // GET /turnaroundprompt/{id} - ReadOnly and Admin access
            var getMethod = resourceWithId.AddMethod("GET", apiIntegrations.GetIntegration, new MethodOptions
            {
                ApiKeyRequired = true,
                RequestParameters = new Dictionary<string, bool>
                {
                    ["method.request.path.id"] = true
                },
                RequestValidator = getValidator
            });

            // PUT /turnaroundprompt - Admin access only
            var putMethod = resource.AddMethod("PUT", apiIntegrations.PutIntegration, new MethodOptions
            {
                ApiKeyRequired = true,
                RequestModels = new Dictionary<string, IModel>
                {
                    [Constants.JsonContentType] = new Model(this, "CreateTurnAroundPromptModel", new ModelProps
                    {
                        RestApi = api,
                        ModelName = "CreateTurnAroundPromptModel",
                        ContentType = Constants.JsonContentType,
                        Schema = JsonSchemas.CreateTurnAroundPromptSchema
                    })
                },
                RequestValidator = putValidator
            });

            // PATCH /turnaroundprompt - Admin access only
            var patchMethod = resource.AddMethod("PATCH", apiIntegrations.PatchIntegration, new MethodOptions
            {
                ApiKeyRequired = true,
                RequestModels = new Dictionary<string, IModel>
                {
                    [Constants.JsonContentType] = new Model(this, "UpdateTurnAroundPromptModel", new ModelProps
                    {
                        RestApi = api,
                        ModelName = "UpdateTurnAroundPromptModel",
                        ContentType = Constants.JsonContentType,
                        Schema = JsonSchemas.UpdateTurnAroundPromptSchema
                    })
                },
                RequestValidator = patchValidator
            });

            // DELETE /turnaroundprompt/{id} - Admin access only
            var deleteMethod = resourceWithId.AddMethod("DELETE", apiIntegrations.DeleteIntegration, new MethodOptions
            {
                ApiKeyRequired = true,
                RequestParameters = new Dictionary<string, bool>
                {
                    ["method.request.path.id"] = true
                },
                RequestValidator = deleteValidator
            });

            // Add API keys to usage plan
            usagePlan.AddApiKey(readOnlyApiKey);
            usagePlan.AddApiKey(adminApiKey);

            // Output the API Gateway information on the console for easy debugging and access

            // Output the API URL
            new CfnOutput(this, "ApiUrl", new CfnOutputProps
            {
                Value = api.Url,
                Description = "API Gateway URL",
                ExportName = $"{Stack.Of(this).StackName}-ApiUrl"
            });

            // Output the API ID
            new CfnOutput(this, "ApiId", new CfnOutputProps
            {
                Value = api.RestApiId,
                Description = "API Gateway ID",
                ExportName = $"{Stack.Of(this).StackName}-ApiId"
            });

            // Output the DynamoDB table name
            new CfnOutput(this, "TableName", new CfnOutputProps
            {
                Value = dynamoDbTable.Table.TableName,
                Description = "DynamoDB Table Name",
                ExportName = $"{Stack.Of(this).StackName}-TableName"
            });

            // Output API keys (for reference - in production, these should be stored securely)
            new CfnOutput(this, "ReadOnlyApiKey", new CfnOutputProps
            {
                Value = Constants.ReadOnlyApiKeyValue,
                Description = "Read-Only API Key",
                ExportName = $"{Stack.Of(this).StackName}-ReadOnlyApiKey"
            });

            new CfnOutput(this, "AdminApiKey", new CfnOutputProps
            {
                Value = Constants.AdminApiKeyValue,
                Description = "Admin API Key",
                ExportName = $"{Stack.Of(this).StackName}-AdminApiKey"
            });
        }
    }
} 