using System.Text.RegularExpressions;

namespace Service.Delegates
{
    public delegate bool ValidationRule<T>(T value);

    public static class NameValidator
    {
        public static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Only letters (including Azerbaijani letters: ə, ı, ö, ü, ş, ç, ğ, Ə, I, İ, Ö, Ü, Ş, Ç, Ğ)
            // No numbers, spaces, or special characters.
            Regex regex = new Regex(@"^[a-zA-ZəıöüşçğƏIİÖÜŞÇĞ]+$");
            return regex.IsMatch(name);
        }

        /* 
        // Usage example:
        // ValidationRule<string> nameValidationRule = NameValidator.IsValidName;
        // bool isValid = nameValidationRule("Əhməd");
        */
    }
}
