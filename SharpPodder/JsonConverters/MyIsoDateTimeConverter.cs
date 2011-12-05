using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Newtonsoft.Json.Utilities;
using Newtonsoft.Json;

namespace SharpPodder
{
  /// <summary>
  /// Converts a <see cref="DateTime"/> to and from the ISO 8601 date format (e.g. 2008-04-12T12:53Z).
  /// </summary>
  public class MyIsoDateTimeConverter : JsonConverter
  {
    private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK";

    private DateTimeStyles _dateTimeStyles = DateTimeStyles.RoundtripKind;
    private string _dateTimeFormat;
    private CultureInfo _culture;

    /// <summary>
    /// Gets or sets the date time styles used when converting a date to and from JSON.
    /// </summary>
    /// <value>The date time styles used when converting a date to and from JSON.</value>
    public DateTimeStyles DateTimeStyles
    {
      get { return _dateTimeStyles; }
      set { _dateTimeStyles = value; }
    }

    /// <summary>
    /// Gets or sets the date time format used when converting a date to and from JSON.
    /// </summary>
    /// <value>The date time format used when converting a date to and from JSON.</value>
    public string DateTimeFormat
    {
      get { return _dateTimeFormat ?? string.Empty; }
      set { _dateTimeFormat = string.IsNullOrWhiteSpace(value) ? null : value; }
    }

    /// <summary>
    /// Gets or sets the culture used when converting a date to and from JSON.
    /// </summary>
    /// <value>The culture used when converting a date to and from JSON.</value>
    public CultureInfo Culture
    {
      get { return _culture ?? CultureInfo.CurrentCulture; }
      set { _culture = value; }
    }

    /// <summary>
    /// Writes the JSON representation of the object.
    /// </summary>
    /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
    /// <param name="value">The value.</param>
    //public abstract void WriteJson (JsonWriter writer, object value, JsonSerializer serializer);
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      string text;

      if (value is DateTime)
      {
        DateTime dateTime = (DateTime)value;

        if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
          || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
          dateTime = dateTime.ToUniversalTime();

        text = dateTime.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);
      }
      else if (value is DateTimeOffset)
      {
        DateTimeOffset dateTimeOffset = (DateTimeOffset)value;
        if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
          || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
          dateTimeOffset = dateTimeOffset.ToUniversalTime();

        text = dateTimeOffset.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);
      }
      else
      {
        throw new Exception(string.Format("Unexpected value when converting date. Expected DateTime or DateTimeOffset, got {0}.", value));
      }

      writer.WriteValue(text);
    }

    /// <summary>
    /// Reads the JSON representation of the object.
    /// </summary>
    /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <returns>The object value.</returns>
    public override object ReadJson(JsonReader reader, Type objectType, object o, JsonSerializer s)
    {
     Type t =
	    objectType == typeof(DateTime?) ? typeof(DateTime)
		: objectType == typeof(DateTimeOffset?) ? typeof(DateTimeOffset)
		: objectType;
      if (reader.TokenType == JsonToken.Null)
      {
        if (objectType != typeof(DateTime?) && objectType != typeof(DateTimeOffset?))
          throw new Exception(string.Format("Cannot convert null value to {0}.", objectType));
 
        return null;
      }

      if (reader.TokenType != JsonToken.String)
        throw new Exception(string.Format("Unexpected token parsing date. Expected String, got {0}.", reader.TokenType));

      string dateText = reader.Value.ToString();

      if (t == typeof(DateTimeOffset))
      {
        if (!string.IsNullOrEmpty(_dateTimeFormat))
          return DateTimeOffset.ParseExact(dateText, _dateTimeFormat, Culture, _dateTimeStyles);
        else
          return DateTimeOffset.Parse(dateText, Culture, _dateTimeStyles);
      }

      if (!string.IsNullOrEmpty(_dateTimeFormat))
        return DateTime.ParseExact(dateText, _dateTimeFormat, Culture, _dateTimeStyles);
      else
        return DateTime.Parse(dateText, Culture, _dateTimeStyles);
    }

    /// <summary>
    /// Determines whether this instance can convert the specified object type.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <returns>
    ///   <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
    /// </returns>
    public override bool CanConvert(Type objectType)
    {
        Type t =
	    objectType == typeof(DateTime?) ? typeof(DateTime)
		: objectType == typeof(DateTimeOffset?) ? typeof(DateTimeOffset)
		: objectType;

      if (typeof(DateTime).IsAssignableFrom(t))
        return true;
      if (typeof(DateTimeOffset).IsAssignableFrom(t))
        return true;

      return false;
    }
  }
}