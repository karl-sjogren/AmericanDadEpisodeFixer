using System;

namespace AmericanDadEpisodeFixer {
    public static class StringExtensions {
        public static Int32 ToInt32(this string source) {
            Int32 tmp;
            if (Int32.TryParse(source, out tmp))
                return tmp;

            return 0;
        }
    }
}