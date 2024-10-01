namespace SmE_CommerceModels.ReturnResult
{
    public class Return<T>
    {
        public T? Data { get; set; }
        public required string Message { get; set; }
        public int TotalRecord { get; set; } = 0;
        public Exception? InternalErrorMessage { get; set; }
        public bool IsSuccess { get; set; } = false;
    }
}