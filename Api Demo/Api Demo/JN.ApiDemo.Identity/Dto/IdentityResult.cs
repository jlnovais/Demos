using System.Collections.Generic;

namespace JN.ApiDemo.Identity.Dto
{
    public enum ErrorType
    {
        NotFound = -1,
        InvalidParameters = -2,
        Other = -99
    }

    public class IdentityResult
    {
        public bool Success { get; set; } = false;

        public IEnumerable<string> Errors { get; set; }

        public int ErrorCode { get; set; }
        public ErrorType ErrorType { get; set; }

        public IdentityResult()
        {

        }

        public IdentityResult(string error, int errorCode = 0, ErrorType errorType = ErrorType.Other)
        {
            Errors = new[] { error };
            ErrorCode = errorCode;
            ErrorType = errorType;
        }

    }

    public class IdentityResult<T>: IdentityResult
    {
        public T Object { get; set; }

        public IdentityResult()
        {
        }

        public IdentityResult(string error, int errorCode = 0, ErrorType errorType = ErrorType.Other) : base(error,
            errorCode, errorType)
        {
        }
    }
}
