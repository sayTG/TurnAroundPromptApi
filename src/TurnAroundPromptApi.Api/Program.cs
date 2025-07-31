using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using TurnAroundPromptApi.Api.EndpointExtensions;
using TurnAroundPromptApi.Services.Implementation;
using TurnAroundPromptApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure AWS DynamoDB
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddScoped<IDynamoDBContext, DynamoDBContext>();
builder.Services.AddScoped<IDynamoDBService, DynamoDBService>();

// Add API Key Authentication Service
builder.Services.AddScoped<IApiKeyAuthenticationService, ApiKeyAuthenticationService>();

var app = builder.Build();

// Ensure DynamoDB table exists on startup
try
{
    using var scope = app.Services.CreateScope();
    var dynamoDbService = scope.ServiceProvider.GetRequiredService<IDynamoDBService>();
    
    // Cast to concrete type to access EnsureTableExistsAsync
    if (dynamoDbService is DynamoDBService concreteService)
    {
        await concreteService.EnsureTableExistsAsync();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Could not verify DynamoDB table: {ex.Message}");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Add TurnAroundPrompt endpoints
app.AddTurnAroundPromptEndpoints();

app.Run();

