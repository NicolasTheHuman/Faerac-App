using System;

[Serializable]
public class ApiResult<T>
{
    public bool Success;
    public T Data;
    public string ErrorMessage;
    public long HttpCode;

    public static ApiResult<T> Ok(T data, long code)
    {
        return new ApiResult<T>
        {
            Success = true,
            Data = data,
            HttpCode = code
        };
    }

    public static ApiResult<T> Fail(string error, long code)
    {
        return new ApiResult<T>
        {
            Success = false,
            ErrorMessage = error,
            HttpCode = code
        };
    }
}

[Serializable]
public class ApiErrorResponse
{
    public string message;
}
