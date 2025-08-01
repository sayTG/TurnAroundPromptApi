using Amazon.CDK;
using TurnAroundPromptApi.Cdk;

namespace TurnAroundPromptApi.Cdk.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = new Amazon.CDK.App();

            new RestApiStack(app, "TurnAroundPromptApiStack", new StackProps
            {
                Env = new Amazon.CDK.Environment
                {
                    Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
                    Region = "us-east-1111111"
                },
                Description = "TurnAroundPrompt API with DynamoDB integration using AWS CDK"
            });

            app.Synth();
        }
    }
}
