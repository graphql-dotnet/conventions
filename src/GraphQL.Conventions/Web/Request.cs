using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace GraphQL.Conventions.Web
{
    /// <summary>
    /// Representation of a web request.
    /// </summary>
    public class Request
    {
        private static readonly IRequestDeserializer RequestDeserializer = new RequestDeserializer();
        private static readonly Regex RegexSuperfluousWhitespace =
            new Regex(@"[ \t\r\n]{1,}", RegexOptions.Multiline | RegexOptions.Compiled);
        private readonly QueryInput _queryInput;

        public static Request New(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var requestBody = new StreamReader(stream).ReadToEnd();
            return New(requestBody);
        }

        public static Request New(string requestBody)
        {
            try
            {
                var queryInput = RequestDeserializer.GetQueryFromRequestBody(requestBody);
                return New(queryInput);
            }
            catch (Exception ex)
            {
                return InvalidInput(ex);
            }
        }

        public static Request New(QueryInput queryInput)
        {
            return new Request(queryInput);
        }

        public static Request InvalidInput(Exception exception)
        {
            return new Request(exception);
        }

        private Request()
        {
            QueryId = Guid.NewGuid().ToString();
        }

        private Request(QueryInput queryInput)
            : this()
        {
            _queryInput = queryInput;

            if (string.IsNullOrWhiteSpace(_queryInput.QueryString))
            {
                Error = new ArgumentException($"Empty query string");
            }
        }

        private Request(Exception exception)
            : this()
        {
            Error = exception;
        }

        public string QueryId { get; private set; }

        /// <summary>
        /// The GraphQL query part of the request.
        /// </summary>
        public string QueryString => _queryInput?.QueryString ?? string.Empty;

        /// <summary>
        /// The variables passed in to the GraphQL query.
        /// </summary>
        public Dictionary<string, object> Variables => _queryInput?.Variables;

        /// <summary>
        /// An optional operation name, determining which operation of the GraphQL query to run.
        /// </summary>
        public string OperationName => _queryInput?.OperationName;

        public bool IsValid => Error == null;

        public Exception Error { get; private set; }

        public string MinifiedQueryString => MinifyString(QueryString);

        public string MinifiedVariablesString => MinifyString(JsonConvert.SerializeObject(Variables));

        private static string MinifyString(string input)
        {
            if (input == null)
            {
                return string.Empty;
            }
            return RegexSuperfluousWhitespace.Replace(input, @" ").Trim();
        }
    }
}
