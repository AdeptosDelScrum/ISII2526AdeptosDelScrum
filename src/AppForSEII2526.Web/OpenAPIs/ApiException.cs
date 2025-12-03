using System;
using System.Collections.Generic;

namespace AppForSEII2526.Web.API
{
    public partial class ApiException : Exception
    {
        public int StatusCode { get; }
        public string Response { get; }
        public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; }

        public ApiException(string message, int statusCode, string response,
            IReadOnlyDictionary<string, IEnumerable<string>> headers, Exception innerException)
            : base(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + (response ?? ""), innerException)
        {
            StatusCode = statusCode;
            Response = response;
            Headers = headers;
        }
    }

    public partial class ApiException<T> : ApiException
    {
        public T Result { get; }

        public ApiException(string message, int statusCode, string response,
            IReadOnlyDictionary<string, IEnumerable<string>> headers, T result, Exception innerException)
            : base(message, statusCode, response, headers, innerException)
        {
            Result = result;
        }
    }
}
