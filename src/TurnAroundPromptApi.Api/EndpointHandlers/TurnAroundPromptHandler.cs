using Microsoft.AspNetCore.Mvc;
using System.Net;
using TurnAroundPromptApi.Api.Services;
using TurnAroundPromptApi.Services.Implementation;
using TurnAroundPromptApi.Services.Interfaces;
using TurnAroundPromptApi.Services.Models;

namespace TurnAroundPromptApi.Api.EndpointHandlers
{
    public static class TurnAroundPromptHandler
    {
        /// <summary>
        /// GET /turnaroundprompt/{id} - Get a specific prompt by ID
        /// </summary>
        public static async Task<IResult> GetTurnAroundPrompt(
            string id,
            [FromServices] IDynamoDBService dynamoDbService,
            [FromServices] IApiKeyAuthenticationService authService,
            HttpContext httpContext)
        {
            // Check API key authentication
            var apiKey = httpContext.GetApiKey();
            if (!authService.IsValidApiKey(apiKey))
            {
                return Results.Unauthorized();
            }

            // Validate ID format
            if (string.IsNullOrEmpty(id) || !System.Text.RegularExpressions.Regex.IsMatch(id, @"^TAP-\d+$"))
            {
                return Results.BadRequest(new { error = "Invalid ID format. Must match pattern TAP-{number}" });
            }

            var request = new TurnAroundPrompt { Id = id };
            var result = await dynamoDbService.GetItem(request);

            return result.StatusCode switch
            {
                HttpStatusCode.OK => Results.Ok(result.Data.FirstOrDefault()),
                HttpStatusCode.NotFound => Results.NotFound(new { error = "Item not found" }),
                HttpStatusCode.BadRequest => Results.BadRequest(result.Message),
                _ => Results.Problem(detail: result.Message?.ToString(), statusCode: StatusCodes.Status500InternalServerError)
            };
        }

        /// <summary>
        /// PUT /turnaroundprompt - Create a new prompt
        /// </summary>
        public static async Task<IResult> CreateTurnAroundPrompt(
            [FromBody] TurnAroundPrompt request,
            [FromServices] IDynamoDBService dynamoDbService,
            [FromServices] IApiKeyAuthenticationService authService,
            HttpContext httpContext)
        {
            // Check API key authentication - Admin only
            var apiKey = httpContext.GetApiKey();
            if (!authService.IsAdminKey(apiKey))
            {
                return Results.Unauthorized();
            }

            // Validate request
            if (request == null)
            {
                return Results.BadRequest(new { error = "Request body is required" });
            }

            // Set deleted to false for new items
            request.Deleted = false;

            var result = await dynamoDbService.InsertItem(request);

            return result.StatusCode switch
            {
                HttpStatusCode.Created => Results.Created($"/turnaroundprompt/{request.Id}", new { message = "Item created successfully" }),
                HttpStatusCode.BadRequest => Results.BadRequest(result.Message),
                _ => Results.Problem(detail: result.Message?.ToString(), statusCode: StatusCodes.Status500InternalServerError)
            };
        }

        /// <summary>
        /// PATCH /turnaroundprompt - Update an existing prompt
        /// </summary>
        public static async Task<IResult> UpdateTurnAroundPrompt(
            [FromBody] TurnAroundPrompt request,
            [FromServices] IDynamoDBService dynamoDbService,
            [FromServices] IApiKeyAuthenticationService authService,
            HttpContext httpContext)
        {
            // Check API key authentication - Admin only
            var apiKey = httpContext.GetApiKey();
            if (!authService.IsAdminKey(apiKey))
            {
                return Results.Unauthorized();
            }

            // Validate request
            if (request == null)
            {
                return Results.BadRequest(new { error = "Request body is required" });
            }

            // Ensure we don't change the deleted flag through PATCH
            request.Deleted = false;

            var result = await dynamoDbService.UpdateItem(request);

            return result.StatusCode switch
            {
                HttpStatusCode.OK => Results.Ok(new { message = "Item updated successfully" }),
                HttpStatusCode.NotFound => Results.NotFound(new { error = "Item not found or already deleted" }),
                HttpStatusCode.BadRequest => Results.BadRequest(result.Message),
                _ => Results.Problem(detail: result.Message?.ToString(), statusCode: StatusCodes.Status500InternalServerError)
            };
        }

        /// <summary>
        /// DELETE /turnaroundprompt/{id} - Soft delete a prompt
        /// </summary>
        public static async Task<IResult> DeleteTurnAroundPrompt(
            string id,
            [FromServices] IDynamoDBService dynamoDbService,
            [FromServices] IApiKeyAuthenticationService authService,
            HttpContext httpContext)
        {
            // Check API key authentication - Admin only
            var apiKey = httpContext.GetApiKey();
            if (!authService.IsAdminKey(apiKey))
            {
                return Results.Unauthorized();
            }

            // Validate ID format
            if (string.IsNullOrEmpty(id) || !System.Text.RegularExpressions.Regex.IsMatch(id, @"^TAP-\d+$"))
            {
                return Results.BadRequest(new { error = "Invalid ID format. Must match pattern TAP-{number}" });
            }

            // Create a request for soft delete
            var request = new TurnAroundPrompt 
            { 
                Id = id,
                Deleted = true // Mark as deleted
            };

            var result = await dynamoDbService.UpdateItem(request);

            return result.StatusCode switch
            {
                HttpStatusCode.OK => Results.Ok(new { message = "Item soft deleted successfully" }),
                HttpStatusCode.NotFound => Results.NotFound(new { error = "Item not found or already deleted" }),
                HttpStatusCode.BadRequest => Results.BadRequest(result.Message),
                _ => Results.Problem(detail: result.Message?.ToString(), statusCode: StatusCodes.Status500InternalServerError)
            };
        }
    }
}
