namespace Crypt
{
    public static class Crypter
    {
        public static string Encrypt(string message, int shift)
        {
            while (shift > 26)
            {
                shift-=26;
            }
            char[] charArray = message.ToCharArray();

            for (int i = 0; i < charArray.Length; i++)
            {
                if (char.IsLetter(charArray[i]))
                {
                   
                    char offset = char.IsUpper(charArray[i]) ? 'A' : 'a';
                    charArray[i] = (char)(((charArray[i] + shift - offset) % 26) + offset);
                }
            }

            return new string(charArray);
        }

        public static string Decrypt(string message, int shift)
        {
            while (shift > 26)
            {
                shift -= 26;
            }
            return Encrypt(message, 26 - shift);
        }
    }
}
