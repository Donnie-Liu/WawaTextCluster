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
        /// �Կհ��ַ����м򵥷ִʣ������Դ�Сд��
        /// ʵ������п������������ķִ��㷨
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


		public Tokeniser()
		{
		}

    }
}
