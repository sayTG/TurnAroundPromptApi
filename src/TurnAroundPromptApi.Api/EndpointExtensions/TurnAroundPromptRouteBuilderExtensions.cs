using TurnAroundPromptApi.Api.EndpointHandlers;
using TurnAroundPromptApi.Services.Models;

namespace TurnAroundPromptApi.Api.EndpointExtensions
{
    public static class TurnAroundPromptRouteBuilderExtensions
    {
        #region Comment Endpoints
        public static void AddCommentEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapPost("insert-items", TurnAroundPromptHandler.InsertTurnAroundPrompts)
            .WithName(nameof(TurnAroundPromptHandler.InsertTurnAroundPrompts))
            .Produces<ServiceResponse<string>>(statusCode: StatusCodes.Status201Created)
            .Produces<ServiceResponse<string>>(statusCode: StatusCodes.Status400BadRequest)
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError);

            endpointRouteBuilder.MapGet("get-items", TurnAroundPromptHandler.GetTurnAroundPrompts)
            .WithName(nameof(TurnAroundPromptHandler.GetTurnAroundPrompts))
            .Produces<ServiceResponse<string>>(statusCode: StatusCodes.Status200OK)
            .Produces<ServiceResponse<string>>(statusCode: StatusCodes.Status201Created)
            .Produces<ServiceResponse<string>>(statusCode: StatusCodes.Status400BadRequest)
            .Produces<ServiceResponse<string>>(statusCode: StatusCodes.Status404NotFound)
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError);

            endpointRouteBuilder.MapGet("get-item", TurnAroundPromptHandler.GetTurnAroundPrompt)
            .WithName(nameof(TurnAroundPromptHandler.GetTurnAroundPrompt))
            .Produces<ServiceResponse<string>>(statusCode: StatusCodes.Status200OK)
            .Produces<ServiceResponse<string>>(statusCode: StatusCodes.Status201Created)
            .Produces<ServiceResponse<string>>(statusCode: StatusCodes.Status400BadRequest)
            .Produces<ServiceResponse<string>>(statusCode: StatusCodes.Status404NotFound)
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError);

            endpointRouteBuilder.MapPut("update-item", TurnAroundPromptHandler.UpdateTurnAroundPrompt)
            .WithName(nameof(TurnAroundPromptHandler.UpdateTurnAroundPrompt))
            .Produces<ServiceResponse<string>>(statusCode: StatusCodes.Status200OK)
            .Produces<ServiceResponse<string>>(statusCode: StatusCodes.Status201Created)
            .Produces<ServiceResponse<string>>(statusCode: StatusCodes.Status400BadRequest)
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError);

            endpointRouteBuilder.MapDelete("delete-item", TurnAroundPromptHandler.DeleteTurnAroundPrompt)
            .WithName(nameof(TurnAroundPromptHandler.DeleteTurnAroundPrompt))
            .Produces<ServiceResponse<string>>(statusCode: StatusCodes.Status200OK)
            .Produces<ServiceResponse<string>>(statusCode: StatusCodes.Status400BadRequest)
            .Produces<ServiceResponse<string>>(statusCode: StatusCodes.Status404NotFound)
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError);
        }
        #endregion
    }
}
