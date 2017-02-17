using System;
using System.Reflection;

namespace GraphQL.Conventions.Execution
{
    public class FieldResolutionException : Exception
    {
        public FieldResolutionException(Exception exception)
            : base(DeriveMessage(exception), ExtractException(exception.InnerException ?? exception))
        {
        }

        private static string DeriveMessage(Exception exception)
        {
            var innerException = ExtractException(exception.InnerException ?? exception);
            return innerException != null && exception.Message != innerException.Message
                ? $"{exception.Message.TrimEnd('.')}. {innerException.Message}"
                : exception.Message;
        }

        private static Exception ExtractException(Exception exception)
        {
            Exception innerException;
            while ((innerException = ExtractInnerException(exception)) != null)
            {
                exception = innerException;
            }
            return exception;
        }

        private static Exception ExtractInnerException(Exception exception)
        {
            if (exception is TargetInvocationException)
            {
                return exception.InnerException;
            }
            else if (exception is AggregateException &&
                ((AggregateException)exception).InnerExceptions.Count == 1)
            {
                return exception.InnerException;
            }
            return null;
        }
    }
}
