using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;
using TurnAroundPromptApi.Services.Models;

namespace TurnAroundPromptApi.Services.DynamoDbExtensions
{
    public static class Extensions
    {
        public static async Task<ServiceResponse<T>> UpdateItemAsync<T>(this IDynamoDBContext context, T entity)
        {
            if (entity == null)
            {
                throw new Exception("Entity is not provided.");
            }

            await context.SaveAsync(entity);
            ServiceResponse<T> remainingItems = context.GetItemAsync(entity).Result;
            remainingItems.Message = "Resource Updated";
            remainingItems.StatusCode = HttpStatusCode.OK;
            return remainingItems;
        }


        public static async Task<ServiceResponse<T>> GetItemAsync<T>(this IDynamoDBContext context, T entity)
        {
            if (entity == null)
            {
                throw new Exception("Entity is not provided.");
            }

            T data = await context.LoadAsync(entity);
            List<T> dataList = new List<T>();
            if (data != null)
            {
                dataList.Add(data);
                return new ServiceResponse<T>(isValid: true, "Resource Found", dataList, HttpStatusCode.OK);
            }

            return new ServiceResponse<T>(isValid: false, "Resource Not Found", dataList, HttpStatusCode.NotFound);
        }

        public static async Task<bool> InsertItems<T>(this BatchWrite<T> batch, List<T> entities)
        {
            batch.AddPutItems(entities);
            await batch.ExecuteAsync(CancellationToken.None);
            return true;
        }

        public static async Task<ServiceResponse<T>> GetItemsAsync<T>(this IDynamoDBContext context, T entity)
        {
            if (entity == null)
            {
                throw new Exception("Entity type (generic class) is not provided.");
            }

            List<T> data = await context.ScanAsync<T>(new List<ScanCondition>()).GetRemainingAsync();

            if (data?.Any() ?? false)
            {
                return new ServiceResponse<T>(isValid: true, "Resources Found", data, HttpStatusCode.OK);
            }

            return new ServiceResponse<T>(isValid: false, "Resources Not Found", data, HttpStatusCode.NotFound);
        }

        public static async Task<ServiceResponse<T>> CreateInsertEntities<T>(this IDynamoDBContext context, List<T> entities, AmazonDynamoDBClient dynamoDBClient)
        {
            if (dynamoDBClient == null)
            {
                throw new Exception("AmazonDynamoDBClient is not provided.");
            }

            if (!entities.Any() || entities == null)
            {
                throw new Exception("Items for insertion are not provided.");
            }

            string tableName = entities.FirstOrDefault().GetType().Name;
            ListTablesResponse allTables = await dynamoDBClient.ListTablesAsync();
            if (allTables.TableNames.Any() && allTables.TableNames.Contains(tableName))
            {
                BatchWrite<T> batch2 = (BatchWrite<T>)context.CreateBatchWrite<T>();
                await batch2.InsertItems(entities);
            }
            else
            {
                List<PropertyInfo> attributes = GetAttributes(entities.FirstOrDefault());
                List<AttributeDefinition> tableAttributes = new List<AttributeDefinition>();
                for (int i = 1; i < 3; i++)
                {
                    Type attributeType = attributes[i].PropertyType;
                    _ = attributeType;
                    _ = attributeType;
                    _ = attributeType;
                    _ = attributeType;
                    _ = attributeType;
                    _ = attributeType;
                    string dynamoDbType;
                    if (false)
                    {
                        dynamoDbType = "N";
                    }
                    else
                    {
                        _ = attributeType;
                        dynamoDbType = ((0 == 0) ? "S" : "B");
                    }

                    tableAttributes.Add(new AttributeDefinition
                    {
                        AttributeName = attributes[i].Name,
                        AttributeType = dynamoDbType
                    });
                }

                CreateTableRequest request = new CreateTableRequest
                {
                    TableName = tableName,
                    AttributeDefinitions = tableAttributes,
                    KeySchema = new List<KeySchemaElement>
                     {
                         new KeySchemaElement
                         {
                         AttributeName = attributes[1].Name,
                         KeyType = "HASH"
                         },
                         new KeySchemaElement
                         {
                         AttributeName = attributes[2].Name,
                         KeyType = "RANGE"
                         }
                     },
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 1L,
                        WriteCapacityUnits = 1L
                    }
                };
                await dynamoDBClient.CreateTableAsync(request);
                bool tableCreated = false;
                while (!tableCreated)
                {
                    Thread.Sleep(2500);
                    Task<DescribeTableResponse> tableData = dynamoDBClient.DescribeTableAsync(new DescribeTableRequest(tableName));
                    if (tableData.Result.Table.TableStatus == TableStatus.ACTIVE)
                    {
                        tableCreated = true;
                    }
                }

                BatchWrite<T> batch = (BatchWrite<T>)context.CreateBatchWrite<T>();
                await batch.InsertItems(entities);
            }

            return new ServiceResponse<T>(isValid: true, "Resource is created.", entities, HttpStatusCode.Created);
        }

        public static List<PropertyInfo> GetAttributes<T>(T entity)
        {
            Type typeFromHandle = typeof(T);
            T val = (T)typeFromHandle.Assembly.CreateInstance(typeFromHandle.FullName);
            TypeInfo typeInfo = typeFromHandle.GetTypeInfo();
            return typeInfo.DeclaredProperties.ToList();
        }
    }
}
