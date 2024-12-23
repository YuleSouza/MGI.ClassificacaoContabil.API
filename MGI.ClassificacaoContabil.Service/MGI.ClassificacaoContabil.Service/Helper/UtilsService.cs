using System.Text.RegularExpressions;

namespace Service.Helper
{
    public static class UtilsService
    {
        public static bool DateHasValue(DateTime? date)
        {
            return date != null && date.HasValue && date.Value != DateTime.MinValue;
        }
        public static bool DateHasValue(DateTime date)
        {
            return date != DateTime.MinValue;
        }
        public static string GetFormatedDate(DateTime? date, string format)
        {
            return DateHasValue(date) ? date.Value.ToString(format) : DateTime.MinValue.ToString(format);
        }

        public static bool EmailValido(string email) 
        {
            var pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+\.br$";
            var regex = new Regex(pattern); 
            return regex.IsMatch(email);
        }
    }
}
