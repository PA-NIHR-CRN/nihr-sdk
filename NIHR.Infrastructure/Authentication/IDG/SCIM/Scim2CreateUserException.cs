using System;
using System.Runtime.Serialization;

namespace NIHR.Infrastructure.Authentication.IDG.SCIM
{
    [Serializable]
    public class Scim2CreateUserException : Exception
    {
        public Scim2CreateUserException()
        {
        }

        public Scim2CreateUserException(string message) : base(message)
        {
        }

        public Scim2CreateUserException(string message, Exception? innerException) : base(message, innerException)
        {
        }

        protected Scim2CreateUserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}