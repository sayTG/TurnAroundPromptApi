namespace TurnAroundPromptApi.Cdk
{
    public static class VtlTemplates
    {
        // Request template for GET operation - retrieves item by ID
        public static string GetRequestTemplate => @"{
            ""TableName"": """ + Constants.TableName + @""",
            ""Key"": {
                ""Id"": {
                    ""S"": ""$input.params('id')""
                }
            }
        }";

        //Response template for GET operation - maps DynamoDB response to JSON
        public static string GetResponseTemplate => @"#if($input.path('$.Item'))
        {
            ""id"": ""$input.path('$.Item.Id.S')"",
            ""name"": ""$input.path('$.Item.Name.S')"",
            ""status"": ""$input.path('$.Item.Status.S')"",
            ""deleted"": $input.path('$.Item.Deleted.BOOL')
        }
        #else
        {
            ""error"": ""Item not found""
        }
        #end";

        // Request template for PUT operation - creates new item
        public static string PutRequestTemplate => @"{
            ""TableName"": """ + Constants.TableName + @""",
            ""Item"": {
                ""Id"": {
                    ""S"": ""$input.path('$.id')""
                },
                ""Name"": {
                    ""S"": ""$input.path('$.name')""
                },
                ""Status"": {
                    ""S"": ""$input.path('$.status')""
                },
                ""Deleted"": {
                    ""BOOL"": false
                }
            },
            ""ConditionExpression"": ""attribute_not_exists(Id)""
        }";

        // Response template for PUT operation
        public static string PutResponseTemplate => @"{
            ""message"": ""Item created successfully""
        }";


        // Request template for PATCH operation - updates existing item
        public static string PatchRequestTemplate => @"{
            ""TableName"": """ + Constants.TableName + @""",
            ""Key"": {
                ""Id"": {
                    ""S"": ""$input.path('$.id')""
                }
            },
            ""UpdateExpression"": ""SET #name = :name, #status = :status"",
            ""ExpressionAttributeNames"": {
                ""#name"": ""Name"",
                ""#status"": ""Status""
            },
            ""ExpressionAttributeValues"": {
                "":name"": {
                    ""S"": ""$input.path('$.name')""
                },
                "":status"": {
                    ""S"": ""$input.path('$.status')""
                },
                "":deleted"": {
                    ""BOOL"": false
                }
            },
            ""ConditionExpression"": ""attribute_exists(Id) AND Deleted = :deleted""
        }";

        // Response template for PATCH operation
        public static string PatchResponseTemplate => @"{
            ""message"": ""Item updated successfully""
        }";

        //Request template for DELETE operation - soft delete using UpdateItem
        public static string DeleteRequestTemplate => @"{
            ""TableName"": """ + Constants.TableName + @""",
            ""Key"": {
                ""Id"": {
                    ""S"": ""$input.params('id')""
                }
            },
            ""UpdateExpression"": ""SET Deleted = :deleted"",
            ""ExpressionAttributeValues"": {
                "":deleted"": {
                    ""BOOL"": true
                },
                "":notDeleted"": {
                    ""BOOL"": false
                }
            },
            ""ConditionExpression"": ""attribute_exists(Id) AND Deleted = :notDeleted""
        }";

        // Response template for DELETE operation
        public static string DeleteResponseTemplate => @"{
            ""message"": ""Item soft deleted successfully""
        }";

        //Error response template for 400 Bad Request
        public static string BadRequestResponseTemplate => @"{
            ""error"": ""Bad Request"",
            ""message"": ""$input.path('$.errorMessage')""
        }";

        //Error response template for 404 Not Found
        public static string NotFoundResponseTemplate => @"{
            ""error"": ""Not Found"",
            ""message"": ""Item not found or already deleted""
        }";

        // Error response template for 500 Internal Server Error
        public static string InternalErrorResponseTemplate => @"{
            ""error"": ""Internal Server Error"",
            ""message"": ""An unexpected error occurred""
        }";
    }
}