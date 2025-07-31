# TurnAroundPromptApi

A simple REST API for managing turnaround prompts with DynamoDB storage and API key authentication.

## What is this?

This is a .NET Web API that lets you create, read, update, and delete turnaround prompts. Each prompt has an ID, name, status, and can be soft-deleted. The API uses DynamoDB for storage and requires API keys for access.

## Quick Start

### Prerequisites

- .NET 9.0 SDK
- AWS CLI configured
- DynamoDB table (see setup below)

### Setup

1. **Run the API**
   ```
   cd src/TurnAroundPromptApi.Api
   dotnet run
   ```

## API Keys

The API uses two API keys for different access levels:

- **readonly-key**: Can only read prompts (GET requests)
- **admin-key**: Can perform all operations (GET, PUT, PATCH, DELETE)

## API Endpoints

### Get a prompt

```
GET /turnaroundprompt/{id}
Headers: x-api-key: readonly-key
```

### Create a prompt

```
PUT /turnaroundprompt
Headers: x-api-key: admin-key
Body: {"id": "TAP-001", "name": "My Prompt", "status": "active"}
```

### Update a prompt

```
PATCH /turnaroundprompt
Headers: x-api-key: admin-key
Body: {"id": "TAP-001", "name": "Updated Prompt", "status": "completed"}
```

### Delete a prompt (soft delete)

```
DELETE /turnaroundprompt/{id}
Headers: x-api-key: admin-key
```

## Data Model

Each prompt has:

- **Id**: Must match pattern `TAP-{number}` (e.g., TAP-001, TAP-123)
- **Name**: 1-255 characters
- **Status**: One of: active, inactive, pending, completed
- **Deleted**: Boolean flag for soft deletes

## Testing

Use the included HTTP file (`TurnAroundPromptApi.Api.http`) to test the API endpoints, or run the test project:

```
cd tests/TurnAroundPromptApi.Tests
dotnet test
```

## Configuration

Update `appsettings.json` with your AWS region and profile:

```json
{
  "AWS": {
    "Region": "{Region}",
    "Profile": "{Profile}"
  }
}
```

## Notes

- Items are soft-deleted (marked as deleted rather than removed)
- API keys are required for all requests
- Invalid requests return appropriate HTTP status codes
- The API runs on HTTPS with development certificates
