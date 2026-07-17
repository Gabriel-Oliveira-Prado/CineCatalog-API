using System;

namespace CineCatalog_API.Application.Exceptions
{
    public class TmdbUnavailableException : Exception
    {
        public TmdbUnavailableException(string message) : base(message)
        {
        }

        public TmdbUnavailableException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
