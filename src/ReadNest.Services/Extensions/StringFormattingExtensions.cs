namespace ReadNest.Services.Extensions
{
    public static class StringFormattingExtensions
    {
        public static string FormatISBN(this string isbn)
        {
            if (!string.IsNullOrWhiteSpace(isbn) && isbn.All(char.IsDigit))
            {
                return $"{isbn.Substring(0, 3)}-{isbn.Substring(3)}";
            }
            return isbn;
        }
    }
}
