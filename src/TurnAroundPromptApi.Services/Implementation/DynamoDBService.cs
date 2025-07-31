using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using System.Net;
using TurnAroundPromptApi.Services.Interfaces;
using TurnAroundPromptApi.Services.Models;

namespace TurnAroundPromptApi.Services.Implementation
{
    public class DynamoDBService : IDynamoDBService
    {
        private readonly IDynamoDBContext _context;
        private readonly AmazonDynamoDBClient _dynamoDbClient;
        private const string TableName = "TurnaroundPromptTable";

        public DynamoDBService(IDynamoDBContext context, AmazonDynamoDBClient dynamoDbClient)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dynamoDbClient = dynamoDbClient ?? throw new ArgumentNullException(nameof(dynamoDbClient));
        }

        public async Task<ServiceResponse<T>> InsertItem<T>(T request)
        {
            try
            {
                if (request == null)
                {
                    return new ServiceResponse<T>(false, "Request cannot be null", new List<T>(), HttpStatusCode.BadRequest);
                }

                await _context.SaveAsync(request);
                var insertedItem = await _context.LoadAsync<T>(request);
                
                return new ServiceResponse<T>(true, "Item created successfully", new List<T> { insertedItem }, HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<T>(false, $"Error creating item: {ex.Message}", new List<T>(), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ServiceResponse<T>> GetItem<T>(T request)
        {
            try
            {
                if (request == null)
                {
                    return new ServiceResponse<T>(false, "Request cannot be null", new List<T>(), HttpStatusCode.BadRequest);
                }

                var item = await _context.LoadAsync<T>(request);
                
                if (item == null)
                {
                    return new ServiceResponse<T>(false, "Item not found", new List<T>(), HttpStatusCode.NotFound);
                }

                // Check if item is soft deleted
                if (item is TurnAroundPrompt prompt && prompt.Deleted)
                {
                    return new ServiceResponse<T>(false, "Item not found", new List<T>(), HttpStatusCode.NotFound);
                }

                return new ServiceResponse<T>(true, "Item found", new List<T> { item }, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<T>(false, $"Error retrieving item: {ex.Message}", new List<T>(), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ServiceResponse<T>> UpdateItem<T>(T request)
        {
            try
            {
                if (request == null)
                {
                    return new ServiceResponse<T>(false, "Request cannot be null", new List<T>(), HttpStatusCode.BadRequest);
                }

                // Check if item exists before updating
                var existingItem = await _context.LoadAsync<T>(request);
                if (existingItem == null)
                {
                    return new ServiceResponse<T>(false, "Item not found", new List<T>(), HttpStatusCode.NotFound);
                }

                // Check if item is soft deleted
                if (existingItem is TurnAroundPrompt existingPrompt && existingPrompt.Deleted)
                {
                    return new ServiceResponse<T>(false, "Item not found or already deleted", new List<T>(), HttpStatusCode.NotFound);
                }

                await _context.SaveAsync(request);
                var updatedItem = await _context.LoadAsync<T>(request);
                
                return new ServiceResponse<T>(true, "Item updated successfully", new List<T> { updatedItem }, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<T>(false, $"Error updating item: {ex.Message}", new List<T>(), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ServiceResponse<T>> DeleteItem<T>(T request)
        {
            try
            {
                if (request == null)
                {
                    return new ServiceResponse<T>(false, "Request cannot be null", new List<T>(), HttpStatusCode.BadRequest);
                }

                // Check if item exists before deleting
                var existingItem = await _context.LoadAsync<T>(request);
                if (existingItem == null)
                {
                    return new ServiceResponse<T>(false, "Item not found", new List<T>(), HttpStatusCode.NotFound);
                }

                // Check if item is already soft deleted
                if (existingItem is TurnAroundPrompt existingPrompt && existingPrompt.Deleted)
                {
                    return new ServiceResponse<T>(false, "Item not found or already deleted", new List<T>(), HttpStatusCode.NotFound);
                }

                // For soft delete, we update the item to mark it as deleted
                if (existingItem is TurnAroundPrompt prompt)
                {
                    prompt.Deleted = true;
                    await _context.SaveAsync(prompt);
                    var resultList = new List<T>();
                    if (prompt is T typedPrompt)
                    {
                        resultList.Add(typedPrompt);
                    }
                    return new ServiceResponse<T>(true, "Item soft deleted successfully", resultList, HttpStatusCode.OK);
                }

                // For hard delete (if needed)
                await _context.DeleteAsync(request);
                return new ServiceResponse<T>(true, "Item deleted successfully", new List<T>(), HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<T>(false, $"Error deleting item: {ex.Message}", new List<T>(), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ServiceResponse<T>> GetItems<T>(T request)
        {
            try
            {
                // Scan all items of type T
                var items = await _context.ScanAsync<T>(new List<ScanCondition>()).GetRemainingAsync();
                
                // Filter out soft deleted items if it's a TurnAroundPrompt
                if (typeof(T) == typeof(TurnAroundPrompt))
                {
                    items = items.Where(item => item is TurnAroundPrompt prompt && !prompt.Deleted).ToList();
                }
                
                if (items?.Any() == true)
                {
                    return new ServiceResponse<T>(true, "Items found", items, HttpStatusCode.OK);
                }

                return new ServiceResponse<T>(false, "No items found", new List<T>(), HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<T>(false, $"Error retrieving items: {ex.Message}", new List<T>(), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ServiceResponse<T>> InsertItems<T>(List<T> request)
        {
            try
            {
                if (request == null || !request.Any())
                {
                    return new ServiceResponse<T>(false, "Request cannot be null or empty", new List<T>(), HttpStatusCode.BadRequest);
                }

                var batch = _context.CreateBatchWrite<T>();
                batch.AddPutItems(request);
                await batch.ExecuteAsync();

                return new ServiceResponse<T>(true, "Items created successfully", request, HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<T>(false, $"Error creating items: {ex.Message}", new List<T>(), HttpStatusCode.InternalServerError);
            }
        }

        // Helper method to ensure table exists
        public async Task EnsureTableExistsAsync()
        {
            try
            {
                var describeTableRequest = new DescribeTableRequest { TableName = TableName };
                await _dynamoDbClient.DescribeTableAsync(describeTableRequest);
            }
            catch (ResourceNotFoundException)
            {
                // Table doesn't exist, create it
                var createTableRequest = new CreateTableRequest
                {
                    TableName = TableName,
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition
                        {
                            AttributeName = "Id",
                            AttributeType = ScalarAttributeType.S
                        }
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "Id",
                            KeyType = KeyType.HASH
                        }
                    },
                    BillingMode = BillingMode.PAY_PER_REQUEST
                };

                await _dynamoDbClient.CreateTableAsync(createTableRequest);

                // Wait for table to be active
                var describeRequest = new DescribeTableRequest { TableName = TableName };
                while (true)
                {
                    var response = await _dynamoDbClient.DescribeTableAsync(describeRequest);
                    if (response.Table.TableStatus == TableStatus.ACTIVE)
                        break;
                    
                    await Task.Delay(1000);
                }
            }
        }
    }
}
