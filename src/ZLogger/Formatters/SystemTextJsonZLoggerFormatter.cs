using System;
using System.Buffers;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Microsoft.Extensions.Logging;

namespace ZLogger.Formatters
{
    public static class ZLoggerOptionsSystemTextJsonExtensions
    {
        public static ZLoggerOptions UseJsonFormatter(this ZLoggerOptions options, Action<SystemTextJsonZLoggerFormatter>? jsonConfigure = null)
        {
            return options.UseFormatter(() =>
            {
                var formatter = new SystemTextJsonZLoggerFormatter();
                jsonConfigure?.Invoke(formatter);
                return formatter;
            });
        }
    }

    public class SystemTextJsonZLoggerFormatter : IZLoggerFormatter
    {
        static readonly JsonEncodedText CategoryNameText = JsonEncodedText.Encode(nameof(LogInfo.CategoryName));
        static readonly JsonEncodedText TimestampText = JsonEncodedText.Encode(nameof(LogInfo.Timestamp));
        static readonly JsonEncodedText LogLevelText = JsonEncodedText.Encode(nameof(LogInfo.LogLevel));
        static readonly JsonEncodedText EventIdText = JsonEncodedText.Encode(nameof(LogInfo.EventId));
        static readonly JsonEncodedText EventIdNameText = JsonEncodedText.Encode("EventIdName");
        static readonly JsonEncodedText ExceptionText = JsonEncodedText.Encode(nameof(LogInfo.Exception));

        static readonly JsonEncodedText NameText = JsonEncodedText.Encode("Name");
        static readonly JsonEncodedText MessageText = JsonEncodedText.Encode("Message");
        static readonly JsonEncodedText StackTraceText = JsonEncodedText.Encode("StackTrace");
        static readonly JsonEncodedText InnerExceptionText = JsonEncodedText.Encode("InnerException");

        static readonly JsonEncodedText Trace = JsonEncodedText.Encode(nameof(LogLevel.Trace));
        static readonly JsonEncodedText Debug = JsonEncodedText.Encode(nameof(LogLevel.Debug));
        static readonly JsonEncodedText Information = JsonEncodedText.Encode(nameof(LogLevel.Information));
        static readonly JsonEncodedText Warning = JsonEncodedText.Encode(nameof(LogLevel.Warning));
        static readonly JsonEncodedText Error = JsonEncodedText.Encode(nameof(LogLevel.Error));
        static readonly JsonEncodedText Critical = JsonEncodedText.Encode(nameof(LogLevel.Critical));
        static readonly JsonEncodedText None = JsonEncodedText.Encode(nameof(LogLevel.None));

        public JsonEncodedText MessagePropertyName { get; set; } = JsonEncodedText.Encode("Message");
        public JsonEncodedText PayloadPropertyName { get; set; } = JsonEncodedText.Encode("Payload");
        public Action<Utf8JsonWriter, LogInfo> MetadataFormatter { get; set; } = DefaultMetadataFormatter;

        public JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        Utf8JsonWriter? jsonWriter;
        
        public void FormatLogEntry<TEntry>(IBufferWriter<byte> writer, TEntry entry) where TEntry : IZLoggerEntry
        {
            jsonWriter?.Reset(writer);
            jsonWriter ??= new Utf8JsonWriter(writer);

            jsonWriter.WriteStartObject();
            
            MetadataFormatter.Invoke(jsonWriter, entry.LogInfo);
            jsonWriter.WritePropertyName(MessagePropertyName);
            entry.WriteJsonMessage(jsonWriter);
            
            throw new NotImplementedException();
        }

        public void FormatLogEntry<TEntry, TPayload>(
            IBufferWriter<byte> writer,
            TEntry entry,
            TPayload? payload,
            ReadOnlySpan<byte> utf8Message)
            where TEntry : IZLoggerEntry
        {
            jsonWriter?.Reset(writer);
            jsonWriter ??= new Utf8JsonWriter(writer);

            jsonWriter.WriteStartObject();
            MetadataFormatter.Invoke(jsonWriter, entry.LogInfo);
            jsonWriter.WriteString(MessagePropertyName, utf8Message);

            if (entry.IsSupportStructuredLogging)
            {
                for (var i = 0; i < entry.ParameterCount; i++)
                {
                    var key = entry.GetParameterKey(i);
                    var valueType = entry.GetParameterType(i);
                    if (valueType == typeof(string))
                    {
                        jsonWriter.WriteString(key, entry.GetParameterValue<string>(i));
                    }
                    else if (valueType == typeof(bool))
                    {
                        jsonWriter.WriteBoolean(key, entry.GetParameterValue<bool>(i));
                    }
                    else if (valueType == typeof(bool?))
                    {
                        var nullableValue = entry.GetParameterValue<bool?>(i);
                        if (nullableValue.HasValue)
                        {
                            jsonWriter.WriteBoolean(key, nullableValue.Value);
                        }
                        else
                        {
                            jsonWriter.WriteNull(key);
                        }
                    }
                    else if (valueType == typeof(byte))
                    {
                        jsonWriter.WriteNumber(key, entry.GetParameterValue<byte>(i));
                    }
                    else if (valueType == typeof(byte?))
                    {
                        var nullableValue = entry.GetParameterValue<byte?>(i);
                        if (nullableValue.HasValue)
                        {
                            jsonWriter.WriteNumber(key, nullableValue.Value);
                        }
                        else
                        {
                            jsonWriter.WriteNull(key);
                        }
                    }
                    else if (valueType == typeof(Int16))
                    {
                        jsonWriter.WriteNumber(key, entry.GetParameterValue<Int16>(i));
                    }
                    else if (valueType == typeof(Int16?))
                    {
                        var nullableValue = entry.GetParameterValue<Int16?>(i);
                        if (nullableValue.HasValue)
                        {
                            jsonWriter.WriteNumber(key, nullableValue.Value);
                        }
                        else
                        {
                            jsonWriter.WriteNull(key);
                        }
                    }
                    else if (valueType == typeof(UInt16))
                    {
                        jsonWriter.WriteNumber(key, entry.GetParameterValue<UInt16>(i));
                    }
                    else if (valueType == typeof(UInt16?))
                    {
                        var nullableValue = entry.GetParameterValue<UInt16?>(i);
                        if (nullableValue.HasValue)
                        {
                            jsonWriter.WriteNumber(key, nullableValue.Value);
                        }
                        else
                        {
                            jsonWriter.WriteNull(key);
                        }
                    }
                    else if (valueType == typeof(Int32))
                    {
                        jsonWriter.WriteNumber(key, entry.GetParameterValue<Int32>(i));
                    }
                    else if (valueType == typeof(Int32?))
                    {
                        var nullableValue = entry.GetParameterValue<Int32?>(i);
                        if (nullableValue.HasValue)
                        {
                            jsonWriter.WriteNumber(key, nullableValue.Value);
                        }
                        else
                        {
                            jsonWriter.WriteNull(key);
                        }
                    }
                    else if (valueType == typeof(UInt32))
                    {
                        jsonWriter.WriteNumber(key, entry.GetParameterValue<UInt32>(i));
                    }
                    else if (valueType == typeof(UInt32?))
                    {
                        var nullableValue = entry.GetParameterValue<UInt32?>(i);
                        if (nullableValue.HasValue)
                        {
                            jsonWriter.WriteNumber(key, nullableValue.Value);
                        }
                        else
                        {
                            jsonWriter.WriteNull(key);
                        }
                    }
                    else if (valueType == typeof(Int64))
                    {
                        jsonWriter.WriteNumber(key, entry.GetParameterValue<Int64>(i));
                    }
                    else if (valueType == typeof(Int64?))
                    {
                        var nullableValue = entry.GetParameterValue<Int64?>(i);
                        if (nullableValue.HasValue)
                        {
                            jsonWriter.WriteNumber(key, nullableValue.Value);
                        }
                        else
                        {
                            jsonWriter.WriteNull(key);
                        }
                    }
                    else if (valueType == typeof(UInt64))
                    {
                        jsonWriter.WriteNumber(key, entry.GetParameterValue<UInt64>(i));
                    }
                    else if (valueType == typeof(UInt16?))
                    {
                        var nullableValue = entry.GetParameterValue<UInt16?>(i);
                        if (nullableValue.HasValue)
                        {
                            jsonWriter.WriteNumber(key, nullableValue.Value);
                        }
                        else
                        {
                            jsonWriter.WriteNull(key);
                        }
                    }
                    else if (valueType == typeof(float))
                    {
                        jsonWriter.WriteNumber(key, entry.GetParameterValue<float>(i));
                    }
                    else if (valueType == typeof(float?))
                    {
                        var nullableValue = entry.GetParameterValue<float?>(i);
                        if (nullableValue.HasValue)
                        {
                            jsonWriter.WriteNumber(key, nullableValue.Value);
                        }
                        else
                        {
                            jsonWriter.WriteNull(key);
                        }
                    }
                    else if (valueType == typeof(double))
                    {
                        jsonWriter.WriteNumber(key, entry.GetParameterValue<double>(i));
                    }
                    else if (valueType == typeof(double?))
                    {
                        var nullableValue = entry.GetParameterValue<double?>(i);
                        if (nullableValue.HasValue)
                        {
                            jsonWriter.WriteNumber(key, nullableValue.Value);
                        }
                        else
                        {
                            jsonWriter.WriteNull(key);
                        }
                    }
                    else if (valueType == typeof(decimal))
                    {
                        jsonWriter.WriteNumber(key, entry.GetParameterValue<decimal>(i));
                    }
                    else if (valueType == typeof(decimal?))
                    {
                        var nullableValue = entry.GetParameterValue<decimal?>(i);
                        if (nullableValue.HasValue)
                        {
                            jsonWriter.WriteNumber(key, nullableValue.Value);
                        }
                        else
                        {
                            jsonWriter.WriteNull(key);
                        }
                    }
                    else if (valueType == typeof(DateTime))
                    {
                        var value = entry.GetParameterValue<DateTime>(i);
                        JsonSerializer.Serialize(jsonWriter, value, JsonSerializerOptions);
                    }
                    else if (valueType == typeof(DateTime?))
                    {
                        var value = entry.GetParameterValue<DateTime?>(i);
                        JsonSerializer.Serialize(jsonWriter, value, JsonSerializerOptions);
                    }
                    else if (valueType == typeof(DateTimeOffset))
                    {
                        var value = entry.GetParameterValue<DateTimeOffset>(i);
                        JsonSerializer.Serialize(jsonWriter, value, JsonSerializerOptions);
                    }
                    else if (valueType == typeof(DateTimeOffset?))
                    {
                        var value = entry.GetParameterValue<DateTimeOffset?>(i);
                        JsonSerializer.Serialize(jsonWriter, value, JsonSerializerOptions);
                    }
                    else
                    {
                        var boxedValue = entry.GetParameterValue(i);
                        JsonSerializer.Serialize(jsonWriter, boxedValue, JsonSerializerOptions);
                    }
                }
            }

            // if (entry.ScopeState is { IsEmpty: false } scopeState)
            // {
            //     for (var i = 0; i < scopeState.Properties.Count; i++)
            //     {
            //         var x = scopeState.Properties[i];
            //         // If `BeginScope(format, arg1, arg2)` style is used, the first argument `format` string is passed with this name
            //         if (x.Key == "{OriginalFormat}")
            //             continue;
            //         
            //         jsonWriter.WritePropertyName(x.Key);
            //         if (x.Value is { } value)
            //         {
            //             JsonSerializer.Serialize(jsonWriter, value, JsonSerializerOptions);
            //         }
            //         else
            //         {
            //             jsonWriter.WriteNullValue();
            //         }
            //     }
            // }

            jsonWriter.WriteEndObject();
            jsonWriter.Flush();
        }

        static JsonEncodedText LogLevelToEncodedText(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return Trace;
                case LogLevel.Debug:
                    return Debug;
                case LogLevel.Information:
                    return Information;
                case LogLevel.Warning:
                    return Warning;
                case LogLevel.Error:
                    return Error;
                case LogLevel.Critical:
                    return Critical;
                case LogLevel.None:
                    return None;
                default:
                    return JsonEncodedText.Encode(((int)logLevel).ToString());
            }
        }

        public static void DefaultMetadataFormatter(Utf8JsonWriter jsonWriter, LogInfo info)
        {
            jsonWriter.WriteString(CategoryNameText, info.CategoryName);
            jsonWriter.WriteString(LogLevelText, LogLevelToEncodedText(info.LogLevel));
            jsonWriter.WriteNumber(EventIdText, info.EventId.Id);
            jsonWriter.WriteString(EventIdNameText, info.EventId.Name);
            jsonWriter.WriteString(TimestampText, info.Timestamp);

            // Write Exception
            if (info.Exception is { } ex)
            {
                jsonWriter.WritePropertyName(ExceptionText);
                WriteException(jsonWriter, ex);
            }
        }

        public static void WriteException(Utf8JsonWriter jsonWriter, Exception? ex)
        {
            if (ex == null)
            {
                jsonWriter.WriteNullValue();
            }
            else
            {
                jsonWriter.WriteStartObject();
                {
                    jsonWriter.WriteString(NameText, ex.GetType().FullName);
                    jsonWriter.WriteString(MessageText, ex.Message);
                    jsonWriter.WriteString(StackTraceText, ex.StackTrace);
                    jsonWriter.WritePropertyName(InnerExceptionText);
                    {
                        WriteException(jsonWriter, ex.InnerException);
                    }
                }
                jsonWriter.WriteEndObject();
            }
        }
    }
}
