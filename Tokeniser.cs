/*
Tokenization
Author: Thanh Ngoc Dao - Thanh.dao@gmx.net
Copyright (c) 2005 by Thanh Ngoc Dao.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WawaSoft.Search.Common;
using PanGu;
using Lucene;
using PanGu.HighLight;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Store;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.Search;


namespace WawaSoft.Search.Common
{
    /// <summary>
    /// Summary description for Tokeniser.
    /// Partition string into SUBwords
    /// </summary>
    internal class Tokeniser : ITokeniser
    {

        /// <summary>
        /// 以空白字符进行简单分词，并忽略大小写，
        /// 实际情况中可以用其它中文分词算法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IList<string> Partition(string input)
        {
            PanGu.Segment.Init();
            Segment segment = new Segment();
            ICollection<WordInfo> words1 = segment.DoSegment(input.ToLower());

            ICollection<WordInfo> words = PatternMatch(words1);


            List<string> filter = new List<string>();

            foreach (WordInfo word in words)
            {

                //Int32 x = (Int32)word.Pos | 0x419090f8;
                //if (x == 0x419090f8)
                int i = word.Word.Length;
                if(i>=2)
                    filter.Add(word.Word);
            }

            return filter.ToArray();


            //Regex r = new Regex("([ \\t{}():;. \n])");
            //input = input.ToLower();

            //String[] tokens = r.Split(input);

            //List<string> filter = new List<string>();

            //for (int i = 0; i < tokens.Length; i++)
            //{
            //    MatchCollection mc = r.Matches(tokens[i]);
            //    if (mc.Count <= 0 && tokens[i].Trim().Length > 0
            //        && !StopWordsHandler.IsStopword(tokens[i]))
            //        filter.Add(tokens[i]);
            //}

            //return filter.ToArray();
        }

        /// <summary>
        /// 匹配名词短语的模式，考虑组成名词短语的词性组合，
        /// 只考虑连续三个词的组合,返回WordInfo类型的短语集合。
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public ICollection<WordInfo> PatternMatch(ICollection<WordInfo> words)
        {
            List<WordInfo> phrase = new List<WordInfo>();
            IEnumerator<WordInfo> enums = words.GetEnumerator();
            Stack<WordInfo> s = new Stack<WordInfo>();
            s.Clear();
            while (enums.MoveNext())
            {
                WordInfo temp = enums.Current;
                Int32 x = (Int32)temp.Pos;
                switch (x)
                {
                    case 0:
                    case 8:
                    case 16:
                    case 32:
                    case 64:
                    case 128:
                    case 8388608:
                    case 16777216:  //专有名词，外文词，未知词性直接加入
                        if (s.Count == 1)
                        {
                            phrase.Add(s.Pop());
                        }
                        phrase.Add(temp);
                        s.Clear();
                        break;
                    case 4096:
                    case 1048576:
                    case 1073741824://名词、动词、形容词
                        if (s.Count == 1)
                        {
                            WordInfo t = s.Pop();
                            if (t.Pos.Equals(POS.POS_D_A) && temp.Pos.Equals(POS.POS_D_A))//两个都是形容词
                            {
                                s.Clear();
                                s.Push(temp);
                            }
                            else
                            {   //把两个词连接起来
                                //t.Pos = POS.POS_D_N;
                                t.Word += temp.Word;
                                phrase.Add(t);
                            }
                        }
                        else
                        {
                            s.Push(temp);
                        }
                    break;

                }
            }
            

            return phrase;
             
        }


        public Tokeniser()
        {
        }

    }
}
