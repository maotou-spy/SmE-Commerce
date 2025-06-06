﻿namespace SmE_CommerceModels.ReturnResult;

public class Return<T>
{
    public required string StatusCode { get; init; } = "999";
    public T? Data { get; init; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalRecord { get; init; }
    public Dictionary<string, List<string>?>? ValidationErrors { get; set; }
    public bool IsSuccess { get; init; }
    public Exception? InternalErrorMessage { get; init; }
}
