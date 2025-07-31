using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnAroundPromptApi.Services.Models;

namespace TurnAroundPromptApi.Services.Interfaces
{
    public interface IDynamoDBService
    {
        #region functions

        Task<ServiceResponse<T>> InsertItems<T>(List<T> request);
        Task<ServiceResponse<T>> InsertItem<T>(T request);
        Task<ServiceResponse<T>> GetItems<T>(T request);
        Task<ServiceResponse<T>> DeleteItem<T>(T request);
        Task<ServiceResponse<T>> UpdateItem<T>(T request);
        Task<ServiceResponse<T>> GetItem<T>(T request);


        #endregion
    }
}
