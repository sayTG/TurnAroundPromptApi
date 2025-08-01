using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.DynamoDB;
using Constructs;

namespace TurnAroundPromptApi.Cdk
{
    public class RestApiStack : Stack
    {
        public RestApiStack(Constructs.Construct scope, string id, IStackProps? props = null) : base(scope, id, props)
        {
            // Create DynamoDB table
            var table = new Table(this, "TurnaroundPromptTable", new TableProps
            {
                TableName = Constants.TableName,
                PartitionKey = new Amazon.CDK.AWS.DynamoDB.Attribute
                {
                    Name = Constants.PartitionKeyName,
                    Type = AttributeType.STRING
                },
                BillingMode = BillingMode.PAY_PER_REQUEST,
                RemovalPolicy = RemovalPolicy.DESTROY
            });

            // Create IAM role for API Gateway to access DynamoDB
            var dynamoRole = new Role(this, Constants.DynamoDbRoleName, new RoleProps
            {
                AssumedBy = new ServicePrincipal("apigateway.amazonaws.com"),
                RoleName = Constants.DynamoDbRoleName,
                Description = "Role for API Gateway to access DynamoDB"
            });

            table.GrantFullAccess(dynamoRole);

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
                }
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

            // Create separate usage plans for different API key types
            var readOnlyUsagePlan = api.AddUsagePlan(Constants.ReadOnlyUsagePlanName, new UsagePlanProps
            {
                Name = Constants.ReadOnlyUsagePlanName,
                Description = Constants.ReadOnlyUsagePlanDescription,
                Throttle = new ThrottleSettings
                {
                    RateLimit = 50,  // Lower rate limit for read-only
                    BurstLimit = 100
                },
                Quota = new QuotaSettings
                {
                    Limit = 5000,    // Lower daily quota for read-only
                    Period = Period.DAY
                }
            });

            var adminUsagePlan = api.AddUsagePlan(Constants.AdminUsagePlanName, new UsagePlanProps
            {
                Name = Constants.AdminUsagePlanName,
                Description = Constants.AdminUsagePlanDescription,
                Throttle = new ThrottleSettings
                {
                    RateLimit = 200,  // Higher rate limit for admin
                    BurstLimit = 400
                },
                Quota = new QuotaSettings
                {
                    Limit = 20000,   // Higher daily quota for admin
                    Period = Period.DAY
                }
            });

            // Add API stage to both usage plans
            readOnlyUsagePlan.AddApiStage(new UsagePlanPerApiStage
            {
                Api = api,
                Stage = api.DeploymentStage
            });

            adminUsagePlan.AddApiStage(new UsagePlanPerApiStage
            {
                Api = api,
                Stage = api.DeploymentStage
            });

            // Add API keys to their respective usage plans
            readOnlyUsagePlan.AddApiKey(readOnlyApiKey);
            adminUsagePlan.AddApiKey(adminApiKey);

            // Create resources
            var resource = api.Root.AddResource(Constants.BasePath);
            var resourceWithId = resource.AddResource(Constants.IdPath);

            // Create API integrations using modular VTL templates
            var apiIntegrations = CreateApiIntegrations(dynamoRole);

            // Create all API methods
            CreateApiMethods(resource, resourceWithId, apiIntegrations);

            // Create stack outputs
            CreateStackOutputs(api, table);
        }

        // Creates all API integrations using modular VTL templates
        private (AwsIntegration Get, AwsIntegration Put, AwsIntegration Patch, AwsIntegration Delete) CreateApiIntegrations(IRole dynamoRole)
        {
            // GET Integration - ReadOnly and Admin access
            var getIntegration = new AwsIntegration(new AwsIntegrationProps
            {
                Service = "dynamodb",
                Action = "GetItem",
                IntegrationHttpMethod = "POST",
                Options = new IntegrationOptions
                {
                    CredentialsRole = dynamoRole,
                    RequestTemplates = new Dictionary<string, string>
                    {
                        [Constants.JsonContentType] = VtlTemplates.GetRequestTemplate
                    },
                    IntegrationResponses = new[]
                    {
                        new IntegrationResponse
                        {
                            StatusCode = Constants.Status200,
                            ResponseTemplates = new Dictionary<string, string>
                            {
                                [Constants.JsonContentType] = VtlTemplates.GetResponseTemplate
                            }
                        },
                        new IntegrationResponse
                        {
                            StatusCode = Constants.Status404,
                            SelectionPattern = ".*Item.*null.*",
                            ResponseTemplates = new Dictionary<string, string>
                            {
                                [Constants.JsonContentType] = VtlTemplates.NotFoundResponseTemplate
                            }
                        }
                    }
                }
            });

            // PUT Integration - Admin access only
            var putIntegration = new AwsIntegration(new AwsIntegrationProps
            {
                Service = "dynamodb",
                Action = "PutItem",
                IntegrationHttpMethod = "POST",
                Options = new IntegrationOptions
                {
                    CredentialsRole = dynamoRole,
                    RequestTemplates = new Dictionary<string, string>
                    {
                        [Constants.JsonContentType] = VtlTemplates.PutRequestTemplate
                    },
                    IntegrationResponses = new[]
                    {
                        new IntegrationResponse
                        {
                            StatusCode = Constants.Status201,
                            ResponseTemplates = new Dictionary<string, string>
                            {
                                [Constants.JsonContentType] = VtlTemplates.PutResponseTemplate
                            }
                        }
                    }
                }
            });

            // PATCH Integration - Admin access only
            var patchIntegration = new AwsIntegration(new AwsIntegrationProps
            {
                Service = "dynamodb",
                Action = "UpdateItem",
                IntegrationHttpMethod = "POST",
                Options = new IntegrationOptions
                {
                    CredentialsRole = dynamoRole,
                    RequestTemplates = new Dictionary<string, string>
                    {
                        [Constants.JsonContentType] = VtlTemplates.PatchRequestTemplate
                    },
                    IntegrationResponses = new[]
                    {
                        new IntegrationResponse
                        {
                            StatusCode = Constants.Status200,
                            ResponseTemplates = new Dictionary<string, string>
                            {
                                [Constants.JsonContentType] = VtlTemplates.PatchResponseTemplate
                            }
                        },
                        new IntegrationResponse
                        {
                            StatusCode = Constants.Status404,
                            SelectionPattern = ".*ConditionalCheckFailedException.*",
                            ResponseTemplates = new Dictionary<string, string>
                            {
                                [Constants.JsonContentType] = VtlTemplates.NotFoundResponseTemplate
                            }
                        }
                    }
                }
            });

            // DELETE Integration - Admin access only (soft delete)
            var deleteIntegration = new AwsIntegration(new AwsIntegrationProps
            {
                Service = "dynamodb",
                Action = "UpdateItem",
                IntegrationHttpMethod = "POST",
                Options = new IntegrationOptions
                {
                    CredentialsRole = dynamoRole,
                    RequestTemplates = new Dictionary<string, string>
                    {
                        [Constants.JsonContentType] = VtlTemplates.DeleteRequestTemplate
                    },
                    IntegrationResponses = new[]
                    {
                        new IntegrationResponse
                        {
                            StatusCode = Constants.Status200,
                            ResponseTemplates = new Dictionary<string, string>
                            {
                                [Constants.JsonContentType] = VtlTemplates.DeleteResponseTemplate
                            }
                        },
                        new IntegrationResponse
                        {
                            StatusCode = Constants.Status404,
                            SelectionPattern = ".*ConditionalCheckFailedException.*",
                            ResponseTemplates = new Dictionary<string, string>
                            {
                                [Constants.JsonContentType] = VtlTemplates.NotFoundResponseTemplate
                            }
                        }
                    }
                }
            });

            return (getIntegration, putIntegration, patchIntegration, deleteIntegration);
        }

        // Creates all API methods
        private void CreateApiMethods(Amazon.CDK.AWS.APIGateway.IResource resource, Amazon.CDK.AWS.APIGateway.IResource resourceWithId, (AwsIntegration Get, AwsIntegration Put, AwsIntegration Patch, AwsIntegration Delete) integrations)
        {
            // GET /turnaroundprompt/{id} - ReadOnly and Admin access
            resourceWithId.AddMethod("GET", integrations.Get, new MethodOptions
            {
                ApiKeyRequired = true,
                RequestParameters = new Dictionary<string, bool>
                {
                    ["method.request.path.id"] = true
                }
            });

            // PUT /turnaroundprompt - Admin access only
            resource.AddMethod("PUT", integrations.Put, new MethodOptions
            {
                ApiKeyRequired = true
            });

            // PATCH /turnaroundprompt - Admin access only
            resource.AddMethod("PATCH", integrations.Patch, new MethodOptions
            {
                ApiKeyRequired = true
            });

            // DELETE /turnaroundprompt/{id} - Admin access only (soft delete)
            resourceWithId.AddMethod("DELETE", integrations.Delete, new MethodOptions
            {
                ApiKeyRequired = true,
                RequestParameters = new Dictionary<string, bool>
                {
                    ["method.request.path.id"] = true
                }
            });
        }

        // Creates stack outputs for console visibility
        private void CreateStackOutputs(RestApi api, Table table)
        {
            // Output the API Gateway information
            new CfnOutput(this, "ApiUrl", new CfnOutputProps
            {
                Value = api.Url,
                Description = "API Gateway URL",
                ExportName = $"{Stack.Of(this).StackName}-ApiUrl"
            });

            new CfnOutput(this, "TableName", new CfnOutputProps
            {
                Value = table.TableName,
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