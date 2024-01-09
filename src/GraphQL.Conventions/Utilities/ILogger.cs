using System;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    public interface ILogger
    {
        void Trace(string message);

        void Info(string message);

        void Warning(string message, Exception exception = null);

        void Error(string message, Exception exception = null);
    }
}
