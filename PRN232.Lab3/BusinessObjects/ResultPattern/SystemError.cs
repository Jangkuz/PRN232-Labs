namespace BusinessObjects.ResultPattern;

public static class SystemError
{
    public static Result InvalidInput(string msg)
    {
        return new Result
        {
            HtmlStatus = 400,
            IsSuccess = false,
            Error = new ResultError
            {
                ErrorCode = "HB40001",
                Message = msg
            }
        };
    }
    public static Result InvalidToken(string msg)
    {
        return new Result
        {
            HtmlStatus = 401,
            IsSuccess = false,
            Error = new ResultError
            {
                ErrorCode = "HB40101",
                Message = msg
            }
        };
    }
    public static Result PermissionDeny(string msg)
    {
        return new Result
        {
            HtmlStatus = 403,
            IsSuccess = false,
            Error = new ResultError
            {
                ErrorCode = "HB40301",
                Message = msg

            }
        };
    }
    public static Result ResourceNotFound(string msg)
    {
        return new Result
        {
            HtmlStatus = 404,
            IsSuccess = false,
            Error = new ResultError
            {
                ErrorCode = "HB40401",
                Message = msg

            }
        };
    }
    public static Result InternalServerError(string msg)
    {
        return new Result
        {
            HtmlStatus = 500,
            IsSuccess = false,
            Error = new ResultError
            {
                ErrorCode = "HB50001",
                Message = msg

            }
        };
    }
}
