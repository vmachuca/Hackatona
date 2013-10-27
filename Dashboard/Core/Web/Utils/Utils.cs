#region "Usings"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#endregion

#region "Implementation"

namespace Core.Web.Utils
{
    #region "Public Class"

    public class Utils
    {
        #region "Public Methods"

        public static bool VerifyEmpty(object obj)
        {
            return obj.ToString().Trim().Equals(string.Empty) ? true : false;
        }

        public static bool isNumber(string input)
        {
            return Regex.IsMatch(input, @"\d");
        }

        #endregion
    }

    #endregion
}

#endregion