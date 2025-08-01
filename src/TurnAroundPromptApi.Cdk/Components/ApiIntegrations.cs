using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.IAM;
using Constructs;

namespace TurnAroundPromptApi.Cdk.Components
{
    /// <summary>
    /// Reusable API Gateway integrations for DynamoDB operations
    /// </summary>
    public class ApiIntegrations : Constructs.Construct
    {
        public AwsIntegration GetIntegration { get; }
        public AwsIntegration PutIntegration { get; }
        public AwsIntegration PatchIntegration { get; }
        public AwsIntegration DeleteIntegration { get; }

        public ApiIntegrations(Constructs.Construct scope, string id, IRole dynamoDbRole) : base(scope, id)
        {
            // GET Integration - Read item by ID
            GetIntegration = new AwsIntegration(new AwsIntegrationProps
            {
                Service = "dynamodb",
                Action = "GetItem",
                IntegrationHttpMethod = "POST",
                Options = new IntegrationOptions
                {
                    CredentialsRole = dynamoDbRole,
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
                            SelectionPattern = ".*[ERROR].*",
                            ResponseTemplates = new Dictionary<string, string>
                            {
                                [Constants.JsonContentType] = VtlTemplates.NotFoundResponseTemplate
                            }
                        }
                    }
                }
            });

            // PUT Integration - Create new item
            PutIntegration = new AwsIntegration(new AwsIntegrationProps
            {
                Service = "dynamodb",
                Action = "PutItem",
                IntegrationHttpMethod = "POST",
                Options = new IntegrationOptions
                {
                    CredentialsRole = dynamoDbRole,
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
                        },
                        new IntegrationResponse
                        {
                            StatusCode = Constants.Status400,
                            SelectionPattern = ".*ConditionalCheckFailedException.*",
                            ResponseTemplates = new Dictionary<string, string>
                            {
                                [Constants.JsonContentType] = VtlTemplates.BadRequestResponseTemplate
                            }
                        }
                    }
                }
            });

            // PATCH Integration - Update existing item
            PatchIntegration = new AwsIntegration(new AwsIntegrationProps
            {
                Service = "dynamodb",
                Action = "UpdateItem",
                IntegrationHttpMethod = "POST",
                Options = new IntegrationOptions
                {
                    CredentialsRole = dynamoDbRole,
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

            // DELETE Integration - Soft delete using UpdateItem
            DeleteIntegration = new AwsIntegration(new AwsIntegrationProps
            {
                Service = "dynamodb",
                Action = "UpdateItem",
                IntegrationHttpMethod = "POST",
                Options = new IntegrationOptions
                {
                    CredentialsRole = dynamoDbRole,
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
        }
    }
} 