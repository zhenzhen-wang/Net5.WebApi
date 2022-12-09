using System;
using System.Runtime.Serialization;

namespace Mitac.Core.Utilities
{
    public class CustomizeException : Exception
    {
        public override string Message { get; } //根据基类提供的message重新context.Exception.Message
        public CustomizeException(string message) : base(message)
        {
            this.Message = message;
        }
    }
}