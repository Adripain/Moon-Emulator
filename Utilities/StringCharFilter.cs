using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Moon.Utilities
{
    static class StringCharFilter
    {
        /// 

        /// Escapes the characters used for injecting special chars from a user input.
        /// 

        /// The string/text to escape.
        /// Allow line breaks to be used (\r\n).
        /// 
        public static string Escape(string str, bool allowBreaks = false)
        {
            str = str.Trim();
            str = str.Replace(Convert.ToChar(1), ' ');
            str = str.Replace(Convert.ToChar(2), ' ');
            str = str.Replace(Convert.ToChar(3), ' ');
            str = str.Replace(Convert.ToChar(9), ' ');

            if (!allowBreaks)
            {
                str = str.Replace(Convert.ToChar(10), ' ');
                str = str.Replace(Convert.ToChar(13), ' ');
            }

            str = Regex.Replace(str, "<(.|\\n)*?>", string.Empty);

            return str;
        }
                public static string EscapeJSONString(string str)
        {
            return str.Replace("\"", "\\\"");
        }
}
}