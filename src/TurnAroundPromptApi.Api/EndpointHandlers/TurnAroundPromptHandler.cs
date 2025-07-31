using Microsoft.AspNetCore.Mvc;
using System.Net;
using TurnAroundPromptApi.Services.Implementation;
using TurnAroundPromptApi.Services.Interfaces;
using TurnAroundPromptApi.Services.Models;

namespace TurnAroundPromptApi.Api.EndpointHandlers
{
    public static class TurnAroundPromptHandler
    {
        public static async Task<IResult> InsertTurnAroundPrompts(
            [FromBody] List<string> request,
            [FromServices] IDynamoDBService dynamoDbLogsService, [FromServices] string validator)
        {

            var result = new ServiceResponse<string>();

            return result.StatusCode switch
            {
                HttpStatusCode.Created => Results.Created("/insert-turnaroundprompt", result),
                HttpStatusCode.BadRequest => Results.BadRequest(result),
                HttpStatusCode.InternalServerError => Results.Problem(detail: result.Message?.ToString(), statusCode: StatusCodes.Status500InternalServerError),
                _ => Results.Problem(statusCode: StatusCodes.Status500InternalServerError)
            };
        }

        public static async Task<IResult> GetTurnAroundPrompts(
        [FromServices] IDynamoDBService dynamoDbLogsService)
        {
            var result = await dynamoDbLogsService.GetItems(new { });

            return result.StatusCode switch
            {
                HttpStatusCode.OK => Results.Ok(result),
                HttpStatusCode.Created => Results.Created("/get-turnaroundprompts", result),
                HttpStatusCode.BadRequest => Results.BadRequest(result),
                HttpStatusCode.NotFound => Results.NotFound(result),
                HttpStatusCode.InternalServerError => Results.Problem(detail: result.Message?.ToString(), statusCode: StatusCodes.Status500InternalServerError),
                _ => Results.Problem(statusCode: StatusCodes.Status500InternalServerError)
            };
        }

        public static async Task<IResult> GetTurnAroundPrompt(
        string entityIdentifier,
        string commentIdentifier,
        [FromServices] IDynamoDBService dynamoDbLogsService)
        {
            var result = await dynamoDbLogsService.GetItem(new
            {
                EntityIdentifier = entityIdentifier,
                CommentIdentifier = commentIdentifier
            });

            return result.StatusCode switch
            {
                HttpStatusCode.OK => Results.Ok(result),
                HttpStatusCode.Created => Results.Created("/get-turnaroundprompt", result),
                HttpStatusCode.BadRequest => Results.BadRequest(result),
                HttpStatusCode.NotFound => Results.NotFound(result),
                HttpStatusCode.InternalServerError => Results.Problem(detail: result.Message?.ToString(), statusCode: StatusCodes.Status500InternalServerError),
                _ => Results.Problem(statusCode: StatusCodes.Status500InternalServerError)
            };
        }

        public static async Task<IResult> UpdateTurnAroundPrompt(
        [FromBody] CommentsItemBanking request,
        [FromServices] IDynamoDBService dynamoDbLogsService, [FromServices] IValidator<CommentsItemBanking> validator)
        {
            var errorMessage = await request.ValidateItemAsync(validator);

            var result = new ServiceResponse<CommentsItemBanking>();

            if (String.IsNullOrEmpty(errorMessage))
                result = await dynamoDbLogsService.UpdateItem(request);
            else
                result = new ServiceResponse<CommentsItemBanking>(false, errorMessage, new List<CommentsItemBanking>(), HttpStatusCode.BadRequest);

            return result.StatusCode switch
            {
                HttpStatusCode.OK => Results.Ok(result),
                HttpStatusCode.Created => Results.Created("/update-comment", result),
                HttpStatusCode.BadRequest => Results.BadRequest(result),
                HttpStatusCode.InternalServerError => Results.Problem(detail: result.Message?.ToString(), statusCode: StatusCodes.Status500InternalServerError),
                _ => Results.Problem(statusCode: StatusCodes.Status500InternalServerError)
            };
        }

        public static async Task<IResult> DeleteTurnAroundPrompt(
        string entityIdentifier,
        string commentIdentifier,
        [FromServices] DynamoDBService dynamoDbLogsService)
        {
            var result = await dynamoDbLogsService.DeleteItem(new CommentsItemBanking()
            {
                EntityIdentifier = entityIdentifier,
                CommentIdentifier = commentIdentifier
            });

            return result.StatusCode switch
            {
                HttpStatusCode.OK => Results.Ok(result),
                HttpStatusCode.BadRequest => Results.BadRequest(result),
                HttpStatusCode.NotFound => Results.NotFound(result),
                HttpStatusCode.InternalServerError => Results.Problem(detail: result.Message?.ToString(), statusCode: StatusCodes.Status500InternalServerError),
                _ => Results.Problem(statusCode: StatusCodes.Status500InternalServerError)
            };
        }
    }
}
