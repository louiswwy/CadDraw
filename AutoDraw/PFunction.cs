using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoDraw
{
    class PFunction
    {
        /// <summary>
        /// 正则表达式
        /// </summary>
        /// <param name="text"></param>
        /// <param name="patten"></param>
        /// <returns></returns>
        public bool isExMatch(string text, string patten)
        {
            bool _isMatch = false;
            Regex Patten = new Regex(patten);
            List<string> _match = new List<string>();

            //if (Regex.IsMatch(text, patten))
            if (Patten.Match(text).Success)
            {
                _isMatch = true;
                for (int num = 1; num < Patten.Match(text).Groups.Count; num++)
                {
                    _match.Add(Patten.Match(text).Groups[num].Value);

                }

            }
            else
                _isMatch = false;
            return _isMatch;
        }


        /// <summary>
        /// 正则表达式
        /// </summary>
        /// <param name="text"></param>
        /// <param name="patten"></param>
        /// <returns></returns>
        public bool isExMatch(string text, string patten,out List<string> ListOut)
        {
            bool _isMatch = false;
            Regex Patten = new Regex(patten);
            List<string> _match = new List<string>();
            ListOut = new List<string>();
            //if (Regex.IsMatch(text, patten))
            if (Patten.Match(text).Success)
            {
                _isMatch = true;
                for (int num = 1; num < Patten.Match(text).Groups.Count; num++)
                {
                    _match.Add(Patten.Match(text).Groups[num].Value);
                    ListOut.Add(Patten.Match(text).Groups[num].Value);
                }

            }
            else
                _isMatch = false;
            return _isMatch;
        }
    }
}
