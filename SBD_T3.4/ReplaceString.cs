using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBD_T3._4
{
    public class ReplaceString
    {
        /// <summary>
        /// Remove os itens de strList da string strIn
        /// </summary>
        /// <param name="strIn"></param>
        /// <param name="strList"></param>
        /// <returns></returns>
        public static string Remove(string strIn, params string [] strList)
        {
            return strList.Aggregate(strIn, (current, item) => current.Replace(item, string.Empty));
        }
    }
}
