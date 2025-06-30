using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.ResultPattern;

public class Result
{
    public int HtmlStatus { get; set; }
    public bool IsSuccess { get; set; }
    public ResultError Error { get; set; } = default!;
}

public class Result<T> : Result
{
    public T? Data { get; set; }
}

public class ResultError
{
    public string ErrorCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public static class ResultExtention
{
    public static Result<T> ToGeneric<T>(this Result result)
    {
        return new Result<T>
        {
            IsSuccess = result.IsSuccess,
            HtmlStatus = result.HtmlStatus,
            Error = result.Error
        };
    }
}
