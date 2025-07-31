using TurnAroundPromptApi.Api.EndpointHandlers;
using TurnAroundPromptApi.Services.Models;

namespace TurnAroundPromptApi.Api.EndpointExtensions
{
    public static class TurnAroundPromptRouteBuilderExtensions
    {
        public static void AddTurnAroundPromptEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            // GET /turnaroundprompt/{id} - Get a specific prompt by ID
            endpointRouteBuilder.MapGet("turnaroundprompt/{id}", TurnAroundPromptHandler.GetTurnAroundPrompt)
                .WithName(nameof(TurnAroundPromptHandler.GetTurnAroundPrompt))
                .Produces<TurnAroundPrompt>(statusCode: StatusCodes.Status200OK)
                .Produces<object>(statusCode: StatusCodes.Status400BadRequest)
                .Produces<object>(statusCode: StatusCodes.Status401Unauthorized)
                .Produces<object>(statusCode: StatusCodes.Status404NotFound)
                .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError);

            // PUT /turnaroundprompt - Create a new prompt
            endpointRouteBuilder.MapPut("turnaroundprompt", TurnAroundPromptHandler.CreateTurnAroundPrompt)
                .WithName(nameof(TurnAroundPromptHandler.CreateTurnAroundPrompt))
                .Produces<object>(statusCode: StatusCodes.Status201Created)
                .Produces<object>(statusCode: StatusCodes.Status400BadRequest)
                .Produces<object>(statusCode: StatusCodes.Status401Unauthorized)
                .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError);

            // PATCH /turnaroundprompt - Update an existing prompt
            endpointRouteBuilder.MapPatch("turnaroundprompt", TurnAroundPromptHandler.UpdateTurnAroundPrompt)
                .WithName(nameof(TurnAroundPromptHandler.UpdateTurnAroundPrompt))
                .Produces<object>(statusCode: StatusCodes.Status200OK)
                .Produces<object>(statusCode: StatusCodes.Status400BadRequest)
                .Produces<object>(statusCode: StatusCodes.Status401Unauthorized)
                .Produces<object>(statusCode: StatusCodes.Status404NotFound)
                .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError);

            // DELETE /turnaroundprompt/{id} - Soft delete a prompt
            endpointRouteBuilder.MapDelete("turnaroundprompt/{id}", TurnAroundPromptHandler.DeleteTurnAroundPrompt)
                .WithName(nameof(TurnAroundPromptHandler.DeleteTurnAroundPrompt))
                .Produces<object>(statusCode: StatusCodes.Status200OK)
                .Produces<object>(statusCode: StatusCodes.Status400BadRequest)
                .Produces<object>(statusCode: StatusCodes.Status401Unauthorized)
                .Produces<object>(statusCode: StatusCodes.Status404NotFound)
                .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
