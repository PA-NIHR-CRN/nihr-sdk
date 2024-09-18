using System;
using System.Runtime.Serialization;

namespace NIHR.Infrastructure.Exceptions
{
    [Serializable]
    public class MaintenanceModeException : Exception
    {
        public MaintenanceModeException() : base("System is in maintenance mode.")
        {
        }

        public MaintenanceModeException(string message) : base(message)
        {
        }

        public MaintenanceModeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MaintenanceModeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
