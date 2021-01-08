using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
namespace WolfInv.Com.WQLanguageLib
{
    public class WQLExplainer
    {
    }

    public class SentenceExplainer : iSentenceExplainer
    {
        string wql = null;
        public SentenceExplainer(string s)
        {
            wql = s;
        }
        const string cKeys = "function,var,if,else,begin,end,{,},for,foreach,while,return,continue,break,goto,switch,case";
        public SentenceClass groupSentence(string sql)
        {
            SentenceClass ret = new SentenceClass();
            Regex reg = new Regex(@"");

            return ret;
        }

        public WordInfo[] getKeyWords()
        {
            List<WordInfo> keys = new List<WordInfo>();
            string[] wordsArr = wql.Split(' ');
            List<string> ckeyArr = cKeys.Split(',').ToList();
            int index = 0;
            for(int i=0;i< wordsArr.Length;i++)
            {
                if(ckeyArr.Contains(wordsArr[i].ToLower().Trim()))
                {
                    WordInfo wi = new WordInfo();
                    wi.index = index;
                    wi.isKey = true;
                    wi.Word = wordsArr[i];
                    keys.Add(wi);
                }
                index += wordsArr[i].Length+1;//长度+空格长度1
            }
            return keys.ToArray();
        }
    }

    public class WordInfo
    {
        public string Word;
        public int index;
        public bool isKey;
    }

    public class SentenceClass
    {
        public WordInfo ForeWord;
        public List<WordInfo> ValidWords=new List<WordInfo>();
        public WordInfo Commit;
    }

    public interface iLanguageExplainer
    {
        bool check(string sql);
        
    }

    public interface iSentenceExplainer
    {
        SentenceClass groupSentence(string wql);
    }
}
