using System.Text.RegularExpressions;

namespace Service.Helper
{
    public static class UtilsService
    {
        private static readonly char[] Acentos = new char[] { 'á', 'à', 'â', 'ã', 'ä', 'å', 'Á', 'À', 'Â', 'Ã', 'Ä', 'Å', 'é', 'è', 'ê', 'ë', 'É', 'È', 'Ê', 'Ë', 'í', 'ì', 'î', 'ï', 'Í', 'Ì', 'Î', 'Ï', 'ó', 'ò', 'ô', 'õ', 'ö', 'Ó', 'Ò', 'Ô', 'Õ', 'Ö', 'ú', 'ù', 'û', 'ü', 'Ú', 'Ù', 'Û', 'Ü', 'ç', 'Ç', 'ñ', 'Ñ' };

        private static readonly (string pattern, string replacement)[] Patterns = new[]
        {
            ("1", "2"),
            ("2", "2"),
            ("3", "2"),
            ("", "o"),
            ("[úùûüÚÙÛÜ]", "u"),
            ("[çÇ]", "c"),
            ("[ñÑ]", "n")
        };

        private static readonly (string pattern, string replacement)[] Patterns = new[]
        {
            ("[áàâãäåÁÀÂÃÄÅ]", "a"),
            ("[éèêëÉÈÊË]", "e"),
            ("[íìîïÍÌÎÏ]", "i"),
            ("[óòôõöÓÒÔÕÖ]", "o"),
            ("[úùûüÚÙÛÜ]", "u"),
            ("[çÇ]", "c"),
            ("[ñÑ]", "n")
        };
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

        public static string RemoverAcentos(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return texto;            

            bool hasAccent = texto.Any(p => Acentos.Contains(p));

            if (!hasAccent) return texto;

            foreach (var (pattern, replacement) in Patterns)
            {
                texto = Regex.Replace(texto, pattern, replacement);
            }

            return texto;
        }
    }
}



