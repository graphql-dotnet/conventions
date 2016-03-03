using System;
using System.Globalization;
using GraphQL.Conventions.Adapters.Types.Extensions;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Language.AST;
using GraphQL.Types;

namespace GraphQL.Conventions.Adapters.Types
{
    public class TimeSpanGraphType : ScalarGraphType
    {
        public TimeSpanGraphType()
        {
            Name = TypeNames.TimeSpan;
            Description = "Represents a time interval.";
        }

        public override object Serialize(object value)
        {
            return value?.ToString();
        }

        public override object ParseValue(object value)
        {
            var timespan = value?.ToString().StripQuotes();
            return string.IsNullOrWhiteSpace(timespan)
                ? null
                : (TimeSpan?)TimeSpan.Parse(timespan, CultureInfo.CurrentCulture);
        }

        public override object ParseLiteral(IValue value)
        {
            var str = value as StringValue;
            if (str != null)
            {
                return ParseValue(str.Value);
            }
            return null;
        }
    }
}
