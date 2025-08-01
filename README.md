# TurnAroundPromptApi

A comprehensive REST API solution for managing TurnAroundPrompt entities with both .NET Web API and AWS CDK infrastructure implementations.

## Architecture

This project provides two distinct implementations:

1. **.NET Web API Solution** - A ASP.NET Core Web API with DynamoDB integration
2. **AWS CDK Infrastructure** - Serverless API Gateway with direct DynamoDB integration using VTL templates

### Prerequisites

- .NET 9.0 SDK
- AWS CLI configured
- Node.js (for CDK)
- AWS CDK CLI

### Option 1: .NET Web API

```bash
# Navigate to the API project
cd src/TurnAroundPromptApi.Api

# Restore dependencies
dotnet restore

# Run the application
dotnet run

# The API will be available at https://localhost:5001, you can check the lauchsettings.json
```

### Option 2: AWS CDK Infrastructure

```bash
# Navigate to the src directory
cd src

# Install CDK dependencies
npm install -g aws-cdk

# Build the CDK solution
dotnet build

# Synthesize CloudFormation template
cdk synth

# Deploy to AWS (make sure you change the region)
cdk deploy
```

## API Endpoints

Both implementations provide the same REST API endpoints:

| Method   | Endpoint                 | Description               | Access           |
| -------- | ------------------------ | ------------------------- | ---------------- |
| `GET`    | `/turnaroundprompt/{id}` | Retrieve a prompt by ID   | ReadOnly + Admin |
| `PUT`    | `/turnaroundprompt`      | Create a new prompt       | Admin only       |
| `PATCH`  | `/turnaroundprompt`      | Update an existing prompt | Admin only       |
| `DELETE` | `/turnaroundprompt/{id}` | Soft delete a prompt      | Admin only       |


### .NET Web API

- Custom API key authentication service
- Two API keys: `readonly-key` and `admin-key`
- Role-based access control

### AWS CDK Infrastructure

- API Gateway API keys
- Usage plans with throttling
- Direct DynamoDB integration with VTL templates

## Data Model

```json
{
  "id": "TAP-123",
  "name": "Sample Prompt",
  "status": "active|inactive|pending|completed",
  "deleted": false
}
```


### AWS CDK Infrastructure Features

- **Modular design** with reusable components
- **VTL templates** for request/response mapping
- **JSON schema validation** for API Gateway
- **Conditional expressions** for safe operations
- **Usage plans** with throttling and quotas
- **CORS support** for cross-origin requests

### Run Tests

```bash
# Navigate to tests directory
cd tests/TurnAroundPromptApi.Tests

# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```


## Configuration

### .NET Web API

```json
{
  "AWS": {
    "Region": "us-east-1",
    "Profile": "default"
  },
  "DynamoDB": {
    "TableName": "TurnaroundPromptTable"
  }
}
```

### AWS CDK

- Region: `us-east-1111111` (non-existent for safety)
- Table: `TurnaroundPromptTable`
- API Keys: Pre-configured for testing

## Modular Components

The CDK solution is highly modularized:

- **Constants.cs** - Centralized configuration values
- **JsonSchemas.cs** - API Gateway request validation schemas
- **VtlTemplates.cs** - Velocity Template Language templates
- **Components/DynamoDbTable.cs** - Reusable DynamoDB table construct
- **Components/ApiIntegrations.cs** - Reusable API Gateway integrations
- **RestApiStack.cs** - Main stack orchestrating all components


### .NET Web API

```bash
# Build for production
dotnet publish -c Release

# Deploy to your preferred hosting platform
# (Azure, AWS, Docker, etc.)
```

### AWS CDK

```bash
# Deploy to AWS
cdk deploy

# Destroy resources
cdk destroy
```

## Security Features

- **API Key Authentication** - Secure access control
- **Input Validation** - JSON schema and data annotations
- **Soft Delete** - Data preservation
- **Conditional Expressions** - Safe DynamoDB operations
- **CORS Configuration** - Cross-origin security
- **IAM Roles** - Least privilege access

## API Key Usage

### ReadOnly Key (`readonly-key`)

- Access: GET operations only
- Rate limit: 50 requests/second
- Burst limit: 100 requests

### Admin Key (`admin-key`)

- Access: All operations
- Rate limit: 100 requests/second
- Burst limit: 200 requests

## Notes

- The CDK solution uses region `us-east-1111111` to prevent accidental deployment
- Both implementations support the same data model and API contracts
- The .NET solution is ready for immediate use
- The CDK solution demonstrates best practices for serverless architecture
- All code is thoroughly documented and follows C# conventions

