namespace SmE_CommerceModels.ReturnResult
{
    public class Return<T>
    {
        public required int ErrorCode { get; set; }
        public T? Data { get; set; }
        public required string Message { get; set; }
        public int? TotalRecord { get; set; }
        public bool IsSuccess { get; set; } = false;
        public Exception? InternalErrorMessage { get; set; }
    }
}
