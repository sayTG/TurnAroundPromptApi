using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using TurnAroundPromptApi.Services.DynamoDbExtensions;
using TurnAroundPromptApi.Services.Interfaces;
using TurnAroundPromptApi.Services.Models;

namespace TurnAroundPromptApi.Services.Implementation
{
    public class DynamoDBService : IDynamoDBService
    {

        #region Properties

        private readonly IDynamoDBContext _context;
        private readonly AmazonDynamoDBClient _dynamoDbClient;

        #endregion

        #region Constructors

        public DynamoDBService(IDynamoDBContext context, AmazonDynamoDBClient dynamoDbClient)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dynamoDbClient = dynamoDbClient ?? throw new ArgumentNullException(nameof(dynamoDbClient));
        }

        #endregion

        #region Methods

        public async Task<ServiceResponse<T>> InsertItems<T>(List<T> request)
        {
            var result = await _context.CreateInsertEntities(request, _dynamoDbClient);

            return new ServiceResponse<T>(result.IsValid, result.Message, result.Data, result.StatusCode);
        }

        public async Task<ServiceResponse<T>> InsertItem<T>(T request)
        {
            var result = await _context.CreateInsertEntities(new List<T> { request }, _dynamoDbClient);

            return new ServiceResponse<T>(result.IsValid, result.Message, result.Data, result.StatusCode);
        }

        public async Task<ServiceResponse<T>> GetItems<T>(T request)
        {
            var result = await _context.GetItemsAsync(request);

            return new ServiceResponse<T>(result.IsValid, result.Message, result.Data, result.StatusCode);
        }

        public async Task<ServiceResponse<T>> GetItem<T>(T request)
        {
            var result = await _context.GetItemAsync(request);

            return new ServiceResponse<T>(result.IsValid, result.Message, result.Data, result.StatusCode);
        }

        public async Task<ServiceResponse<T>> UpdateItem<T>(T request)
        {
            var result = await _context.UpdateItemAsync(request);

            return new ServiceResponse<T>(result.IsValid, result.Message, result.Data, result.StatusCode);
        }

        public async Task<ServiceResponse<T>> DeleteItem<T>(T request)
        {
            var result = await _context.DeleteItemAsync(request);

            return new ServiceResponse<T>(result.IsValid, result.Message, result.Data, result.StatusCode);
        }

        #endregion
    }
}
