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
        /// ƥ�����ʶ����ģʽ������������ʶ���Ĵ�����ϣ�
        /// ֻ�������������ʵ����,����WordInfo���͵Ķ��Ｏ�ϡ�
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
                    //���������POS_UNK�����������ĵ�δ֪���Ժ�Ӣ�ģ���ֱ�����
                    if((int)word[i].Pos ==0)
                        phrase.Add(word[i]);
                    else
                        i++;

                }
                
                if (i == 0) //�պ�ȡ�����дʣ���ֹ
                    break;
                else if(i==1)//����ֻʣ��һ����
                {
                    phrase.Add(word[0]);
                    break;
                }else if(i==2)//�������ʣ��������
                {
                    break;
                }else if(i==3)//����ȡ��������
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
