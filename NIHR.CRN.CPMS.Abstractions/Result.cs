namespace NIHR.CRN.CPMS.Abstractions
{
    public struct Result<TValue>
    {
        public bool IsSuccess { get; }
        public TValue Value { get; }
        public string ErrorString { get; }

        private Result(bool success, TValue value, string errorString)
        {
            IsSuccess = success;
            Value = value;
            ErrorString = errorString;
        }

        public static Result<TValue> Fail(string error)
        {
            return new Result<TValue>(false, default, error);
        }

        public static Result<TValue> Success(TValue user)
        {
            return new Result<TValue>(true, user, null);
        }
    }
}