using System.Net;

namespace CareNest_NewService.Domain.Commons.Base
{
    public abstract class BaseException
    {
        public static ErrorException BadRequestBadRequestResponse(string message)
        {
            return new ErrorException(new ErrorDetail
            {
                ErrorCode = "BAD_REQUEST",
                ErrorMessage = message,
                StatusCode = (int)HttpStatusCode.BadRequest
            });
        }

        public static ErrorException NotFoundResponse(string message)
        {
            return new ErrorException(new ErrorDetail
            {
                ErrorCode = "NOT_FOUND",
                ErrorMessage = message,
                StatusCode = (int)HttpStatusCode.NotFound
            });
        }

        public static ErrorException InternalServerErrorResponse(string message)
        {
            return new ErrorException(new ErrorDetail
            {
                ErrorCode = "INTERNAL_SERVER_ERROR",
                ErrorMessage = message,
                StatusCode = (int)HttpStatusCode.InternalServerError
            });
        }

        public class ErrorException : Exception
        {
            public ErrorDetail ErrorDetail { get; }
            public int StatusCode { get; }

            public ErrorException(ErrorDetail errorDetail) : base(errorDetail.ErrorMessage)
            {
                ErrorDetail = errorDetail;
                StatusCode = errorDetail.StatusCode;
            }
        }

        public class ErrorDetail
        {
            public string ErrorCode { get; set; } = string.Empty;
            public string ErrorMessage { get; set; } = string.Empty;
            public int StatusCode { get; set; }
        }
    }
}
