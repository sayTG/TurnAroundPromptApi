using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TurnAroundPromptApi.Services.Models
{
    public record ServiceResponse<T>
    {
        #region Constructors

        public ServiceResponse(bool isValid, object? message, IEnumerable<T> data, HttpStatusCode statusCode)
        {
            IsValid = isValid;
            Message = message;
            Data = data;
            StatusCode = statusCode;
        }

        public ServiceResponse() { }

        #endregion

        #region Properties

        public bool IsValid { get; set; }
        public object Message { get; set; }
        public IEnumerable<T> Data { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        #endregion
    }
}
