using System;
using System.Runtime.Serialization;

namespace NIHR.Infrastructure.Authentication
{
    [Serializable]
    public class PasswordPolicyException : Exception
    {
        public PasswordPolicyException()
        {
        }

        public PasswordPolicyException(string message) : base(message)
        {
        }

        public PasswordPolicyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PasswordPolicyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}