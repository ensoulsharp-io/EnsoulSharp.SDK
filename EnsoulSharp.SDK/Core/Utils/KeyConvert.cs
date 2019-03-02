namespace EnsoulSharp.SDK.Core.Utils
{
    public class KeyConvert
    {
        /// <summary>
        ///     Transforms the virtual key to text.
        /// </summary>
        /// <param name="vKey">
        ///     The virtual key.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string KeyToText(uint vKey)
        {
            /* A-Z */
            if (vKey >= 65 && vKey <= 90)
            {
                return ((char)vKey).ToString();
            }

            /* F1-F12 */
            if (vKey >= 112 && vKey <= 123)
            {
                return ("F" + (vKey - 111));
            }

            switch (vKey)
            {
                case 9:
                    return "Tab";
                case 16:
                    return "Shift";
                case 17:
                    return "Ctrl";
                case 20:
                    return "CAPS";
                case 27:
                    return "ESC";
                case 32:
                    return "Space";
                case 45:
                    return "Insert";
                default:
                    return vKey.ToString();
            }
        }
    }
}