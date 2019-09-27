using System;

namespace DapperExtensions.Exceptions
{
    public class WaitQueueFullException : Exception
    {
        public WaitQueueFullException() : base("The waiting queue for the connection is full.", null) { }
    }
}
