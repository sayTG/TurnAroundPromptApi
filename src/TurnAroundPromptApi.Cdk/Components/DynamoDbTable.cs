using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Constructs;

namespace TurnAroundPromptApi.Cdk.Components
{
    public class DynamoDbTable : Constructs.Construct
    {
        public Table Table { get; }

        public DynamoDbTable(Constructs.Construct scope, string id, IStackProps? props = null) : base(scope, id)
        {
            Table = new Table(this, "TurnAroundPromptTable", new TableProps
            {
                TableName = Constants.TableName,
                PartitionKey = new Amazon.CDK.AWS.DynamoDB.Attribute
                {
                    Name = Constants.PartitionKeyName,
                    Type = AttributeType.STRING
                },
                BillingMode = BillingMode.PAY_PER_REQUEST,
                RemovalPolicy = RemovalPolicy.RETAIN,
                PointInTimeRecovery = true,
                Encryption = TableEncryption.AWS_MANAGED
            });

            Tags.Of(Table).Add("Environment", "Development");
            Tags.Of(Table).Add("Project", "TurnAroundPromptApi");
            Tags.Of(Table).Add("Component", "DynamoDB");
        }

        public void GrantReadData(IRole role)
        {
            Table.GrantReadData(role);
        }

        public void GrantWriteData(IRole role)
        {
            Table.GrantWriteData(role);
        }

        public void GrantFullAccess(IRole role)
        {
            Table.GrantFullAccess(role);
        }
    }
} 