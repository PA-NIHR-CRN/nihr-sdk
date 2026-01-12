namespace NIHR.CRN.CPMS.Abstractions
{
    public class AuthResult<TUser>
    {
        public bool IsSuccess { get; }
        public TUser User { get; }
        public string ErrorString { get; }

        private AuthResult(bool success, TUser user, string errorString)
        {
            IsSuccess = success;
            User = user;
            ErrorString = errorString;
        }

        public static AuthResult<TUser> Fail(string error)
        {
            return new AuthResult<TUser>(false, default, error);
        }

        public static AuthResult<TUser> Success(TUser user)
        {
            return new AuthResult<TUser>(true, user, null);
        }
    }
}