/* FILE: Misc.cs
 * 
 * This unit contains classes and functions for miscellaneous code that does not belong in
 *  other modules, especially utility functions.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPlatformCloudKit.Common
{
    public static class StringExtensions
    {
        /// <summary>
        /// Extension method that adds a Contains() method to strings that can accept a string comparison style.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="toCheck">The string to search for inside the source string.</param>
        /// <param name="comp">The comparison style.</param>
        /// <returns>TRUE if the string toCheck exists in the source string using the given <br />
        ///  comparison style.</returns>
        /// <remarks>Sourced from this Stack Overflow post that goes into detail on the problems <br />
        ///  case insensitive searches across languages:<br />
        ///      http://stackoverflow.com/questions/444798/case-insensitive-containsstring
        ///  </remarks>
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        /// <summary>
        /// Extension method that adds a a case insensitive culture invariant Contains() method to strings.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="toCheck">The string to search for inside the source string.</param>
        /// <returns>TRUE if the string toCheck exists in the source string given a <br />
        ///  comparison style.</returns>
        public static bool ContainsIgnoreCase(this string source, string toCheck)
        {
            return source.IndexOf(toCheck, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }
    }


    public static class Misc
    {
        /// <summary>
        /// Adds the current date/time in numeric format to a URL to defeat URL request caching, <br />
        ///  ensuring a fresh grab of the url contents.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string CacheBusterUrl(string url)
        {
            string cacheBusterUrlArgument = DateTime.Now.Ticks.ToString();

            if (url.Contains("?"))
                return url + "&uncache=" + cacheBusterUrlArgument;

            return url + "?uncache=" + cacheBusterUrlArgument;
        }
    }
}
