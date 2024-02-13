using System.Globalization;

namespace DataBus.Application.Exceptions;

public class PropertyNullExeption : Exception
{
    public PropertyNullExeption() : base() { }

    public PropertyNullExeption(string message) : base(message) { }

    public PropertyNullExeption(string message, params object[] args)
        : base(String.Format(CultureInfo.CurrentCulture, message, args))
    {
    }
}
