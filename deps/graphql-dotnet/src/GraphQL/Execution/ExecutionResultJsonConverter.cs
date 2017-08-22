using System;
using System.Linq;
using Newtonsoft.Json;

namespace GraphQL
{
    public class ExecutionResultJsonConverter : JsonConverter
    {
        public static bool EnableCompatibilityMode { get; set; } // Temporarily output error messages with label "error"

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is ExecutionResult)
            {
                var result = (ExecutionResult) value;

                writer.WriteStartObject();

                writeData(result, writer, serializer);
                writeErrors(result.Errors, writer, serializer, result.ExposeExceptions);
                writeExtra(result, writer, serializer);

                writer.WriteEndObject();
            }
        }

        private void writeData(ExecutionResult result, JsonWriter writer, JsonSerializer serializer)
        {
            var data = result.Data;

            if (result.Errors?.Any() == true && data == null)
            {
                return;
            }

            writer.WritePropertyName("data");
            serializer.Serialize(writer, data);
        }

        private void writeErrors(ExecutionErrors errors, JsonWriter writer, JsonSerializer serializer, bool exposeExceptions)
        {
            if (errors == null || !errors.Any())
            {
                return;
            }

            writer.WritePropertyName("errors");

            writer.WriteStartArray();

            errors.Apply(error =>
            {
                writer.WriteStartObject();

                if (EnableCompatibilityMode)
                {
                    writer.WritePropertyName("error");
                    serializer.Serialize(writer, error.Message);
                }

                writer.WritePropertyName("message");
                if (exposeExceptions)
                {
                    serializer.Serialize(writer, error.ToString()); // return StackTrace (including all inner exceptions)
                }
                else
                {
                    serializer.Serialize(writer, error.Message);
                }

                if (error.Locations != null)
                {
                    writer.WritePropertyName("locations");
                    writer.WriteStartArray();
                    error.Locations.Apply(location =>
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("line");
                        serializer.Serialize(writer, location.Line);
                        writer.WritePropertyName("column");
                        serializer.Serialize(writer, location.Column);
                        writer.WriteEndObject();
                    });
                    writer.WriteEndArray();
                }

                if (error.Path != null && error.Path.Count > 0)
                {
                    writer.WritePropertyName("path");
                    serializer.Serialize(writer, error.Path);
                }

                if (!string.IsNullOrWhiteSpace(error.Code))
                {
                    writer.WritePropertyName("code");
                    serializer.Serialize(writer, error.Code);
                }

                if (error.Data != null && error.Data.Count > 0)
                {
                    writer.WritePropertyName("data");
                    writer.WriteStartObject();
                    error.Data.Apply(entry =>
                    {
                        writer.WritePropertyName(entry.Key);
                        serializer.Serialize(writer, entry.Value);
                    });
                    writer.WriteEndObject();
                }

                writer.WriteEndObject();
            });

            writer.WriteEndArray();
        }

        private void writeExtra(ExecutionResult result, JsonWriter writer, JsonSerializer serializer)
        {
            if (result.Extra == null || result.Extra.Count == 0)
            {
                return;
            }

            writer.WritePropertyName("extra");
            writer.WriteStartObject();
            result.Extra.Apply(kvp =>
            {
                writer.WritePropertyName(kvp.Key);
                serializer.Serialize(writer, kvp.Value);
            });
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ExecutionResult);
        }
    }
}
