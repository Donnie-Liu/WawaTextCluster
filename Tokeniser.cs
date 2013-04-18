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
            ICollection<WordInfo> words = segment.DoSegment(input.ToLower());

            

            List<string> filter = new List<string>();

            foreach (WordInfo word in words)
            {
                
                Int32 x=(Int32)word.Pos | 0x419090f8;
                if(x == 0x419090f8)
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
            int i = 0;
            WordInfo[] word = new WordInfo[3];
            while (true)
            {
                
                while (enums.MoveNext() && i < 3)
                {
                    word[i] = enums.Current;
                    //如果词性是POS_UNK，包含了中文的未知词性和英文，则直接输出
                    if((int)word[i].Pos ==0)
                        phrase.Add(word[i]);
                    else
                        i++;

                }
                
                if (i == 0) //刚好取完所有词，终止
                    break;
                else if(i==1)//正好只剩下一个词
                {
                    phrase.Add(word[0]);
                    break;
                }else if(i==2)//最后正好剩下两个词
                {
                    break;
                }else if(i==3)//正好取得三个词
                {
                    
                }
  
            }

            return phrase;
             
        }


		public Tokeniser()
		{
		}

    }
}
