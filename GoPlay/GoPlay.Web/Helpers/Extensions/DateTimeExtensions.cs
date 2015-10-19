using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoPlay.Web.Helpers.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts the value of the current DateTime object to the date and time specified by an offset value.
        /// </summary>
        /// <param name="dt">DateTime value.</param>
        /// <param name="offset">The offset to convert the DateTime value to.</param>
        /// <returns>DateTime value that is local to an offset.</returns>
        public static DateTime ToOffset (this DateTime dt, TimeSpan offset)
        {
            return dt.ToUniversalTime().Add(offset);
        }
    }
}