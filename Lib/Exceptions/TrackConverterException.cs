using System;

namespace TrackConverter.Lib.Exceptions
{
    /// <summary>
    /// исключение, создаваемое TrackConverter
    /// </summary>
    [Serializable]
    public class TrackConverterException: ApplicationException
    {
        public TrackConverterException() { }
        public TrackConverterException(string message) : base(message) { }
        public TrackConverterException(string message, Exception inner) : base(message, inner) { }
        protected TrackConverterException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
