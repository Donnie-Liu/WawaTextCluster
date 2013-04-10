/*
 * tf/idf implementation 
 * Author: Thanh Dao, thanh.dao@gmx.net
 */
using System;
using System.Collections;
using System.Collections.Generic;
using WawaSoft.Search.Common;


namespace WawaSoft.Search.Common
{
	/// <summary>
	/// Summary description for TF_IDFLib.
	/// </summary>
	public class TFIDFMeasure
	{
		private string[] _docs;
		private string[][] _ngramDoc;
		private int _numDocs=0;
		private int _numTerms=0;
		private ArrayList _terms;
		private int[][] _termFreq;
		private float[][] _termWeight;
		private int[] _maxTermFreq;
		private int[] _docFreq;

        ITokeniser _tokenizer = null;





	    private IDictionary _wordsIndex=new Hashtable() ;

		public TFIDFMeasure(string[] documents,ITokeniser tokeniser)
		{
			_docs=documents;
			_numDocs=documents.Length ;
		    _tokenizer = tokeniser;
			MyInit();
		}

	    public int NumTerms
	    {
	        get { return _numTerms; }
	        set { _numTerms = value; }
	    }

	    private void GeneratNgramText()
		{
			
		}

		private ArrayList GenerateTerms(string[] docs)
		{
			ArrayList uniques=new ArrayList() ;
			_ngramDoc=new string[_numDocs][] ;
			for (int i=0; i < docs.Length ; i++)
			{
				IList<string> words=_tokenizer.Partition(docs[i]);		

				for (int j=0; j < words.Count; j++)
					if (!uniques.Contains(words[j]) )				
						uniques.Add(words[j]) ;
								
			}
			return uniques;
		}
		


		private static object AddElement(IDictionary collection, object key, object newValue)
		{
			object element=collection[key];
			collection[key]=newValue;
			return element;
		}

		private int GetTermIndex(string term)
		{
			object index=_wordsIndex[term];
			if (index == null) return -1;
			return (int) index;
		}

		private void MyInit()
		{
			_terms=GenerateTerms (_docs );
			NumTerms=_terms.Count ;

			_maxTermFreq=new int[_numDocs] ;
			_docFreq=new int[NumTerms] ;
			_termFreq =new int[NumTerms][] ;
			_termWeight=new float[NumTerms][] ;

			for(int i=0; i < _terms.Count ; i++)			
			{
				_termWeight[i]=new float[_numDocs] ;
				_termFreq[i]=new int[_numDocs] ;

				AddElement(_wordsIndex, _terms[i], i);			
			}
			
			GenerateTermFrequency ();
			GenerateTermWeight();			
				
		}
		
		private float Log(float num)
		{
			return (float) Math.Log(num) ;//log2
		}

		private void GenerateTermFrequency()
		{
			for(int i=0; i < _numDocs  ; i++)
			{								
				string curDoc=_docs[i];
				IDictionary freq=GetWordFrequency(curDoc);
				IDictionaryEnumerator enums=freq.GetEnumerator() ;
				_maxTermFreq[i]=int.MinValue ;
				while (enums.MoveNext())
				{
					string word=(string)enums.Key;
					int wordFreq=(int)enums.Value ;
					int termIndex=GetTermIndex(word);
                    if(termIndex == -1)
                        continue;
					_termFreq [termIndex][i]=wordFreq;
					_docFreq[termIndex] ++;

					if (wordFreq > _maxTermFreq[i]) _maxTermFreq[i]=wordFreq;					
				}
			}
		}
		

		private void GenerateTermWeight()
		{			
			for(int i=0; i < NumTerms   ; i++)
			{
				for(int j=0; j < _numDocs ; j++)				
					_termWeight[i][j]=ComputeTermWeight (i, j);				
			}
		}

		private float GetTermFrequency(int term, int doc)
		{			
			int freq=_termFreq [term][doc];
			int maxfreq=_maxTermFreq[doc];			
			
			return ( (float) freq/(float)maxfreq );
		}

		private float GetInverseDocumentFrequency(int term)
		{
			int df=_docFreq[term];
			return Log((float) (_numDocs) / (float) df );
		}

		private float ComputeTermWeight(int term, int doc)
		{
			float tf=GetTermFrequency (term, doc);
			float idf=GetInverseDocumentFrequency(term);
			return tf * idf;
		}
		
		private  float[] GetTermVector(int doc)
		{
			float[] w=new float[NumTerms] ; 
			for (int i=0; i < NumTerms; i++)											
				w[i]=_termWeight[i][doc];
			
				
			return w;
		}
        public double [] GetTermVector2(int doc)
        {
            double [] ret = new double[NumTerms];
            float[] w = GetTermVector(doc);
            for (int i = 0; i < ret.Length; i++ )
            {
                ret[i] = w[i];
            }
            return ret;
        }

		public double GetSimilarity(int doc_i, int doc_j)
		{
			double [] vector1=GetTermVector2 (doc_i);
			double [] vector2=GetTermVector2 (doc_j);

			return TermVector.ComputeCosineSimilarity(vector1, vector2) ;

		}
		
		private IDictionary GetWordFrequency(string input)
		{
			string convertedInput=input.ToLower() ;
					
            List<string> temp = new List<string>(_tokenizer.Partition(convertedInput));
			string[] words= temp.ToArray();		
	        
			Array.Sort(words);
			
			String[] distinctWords=GetDistinctWords(words);
						
			IDictionary result=new Hashtable();
			for (int i=0; i < distinctWords.Length; i++)
			{
				object tmp;
				tmp=CountWords(distinctWords[i], words);
				result[distinctWords[i]]=tmp;
				
			}
			
			return result;
		}				
				
		private static string[] GetDistinctWords(String[] input)
		{				
			if (input == null)			
				return new string[0];			
			else
			{
                List<string> list = new List<string>();
				
				for (int i=0; i < input.Length; i++)
					if (!list.Contains(input[i])) // N-GRAM SIMILARITY?				
						list.Add(input[i]);
				
				return list.ToArray();
			}
		}
		

		
		private int CountWords(string word, string[] words)
		{
			int itemIdx=Array.BinarySearch(words, word);
			
			if (itemIdx > 0)			
				while (itemIdx > 0 && words[itemIdx].Equals(word))				
					itemIdx--;				
						
			int count=0;
			while (itemIdx < words.Length && itemIdx >= 0)
			{
				if (words[itemIdx].Equals(word)) count++;				
				
				itemIdx++;
				if (itemIdx < words.Length)				
					if (!words[itemIdx].Equals(word)) break;					
				
			}
			
			return count;
		}

        /// <summary>
        /// 获取某组文档的关键词
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<string> GetKeyword(List<int> arr, int count)
        {
            //1、给每个文档分词并保存在一个列表里
            List<string> allWords = new List<string>();
            foreach (int i in arr)
            {
                //这里把一个文档里出现的多个词进行消重
                allWords.AddRange(GetDistinctWords(_tokenizer.Partition(_docs[i])));
            }

            //2、用一个字典保存词的词频,key是词，value是重复次数
            Dictionary<string, int> tfDict = SortByDuplicateCount(allWords);

            //3、遍历已排序的词频字典，并获取每个词的IDF值，并把更新后的结果放入一个tfidfDict词典
            //该词典的key是词，value是tfidf值
            Dictionary<string, float> tfidfDict = new Dictionary<string, float>(tfDict.Count);
            foreach (KeyValuePair<string, int> pair in tfDict)
            {
                int tremIndex;
                if (tfDict.TryGetValue(pair.Key, out tremIndex))
                {
                    float idf = GetInverseDocumentFrequency(GetTermIndex(pair.Key));
                    tfidfDict.Add(pair.Key, pair.Value * idf);
                }
            }

            //4、给tfidf字典俺权重排序
            tfidfDict = GetSortByValueDict(tfidfDict);

            //5、更新要提取的关键词数量
            int keywordCount = count;
            if (keywordCount > tfidfDict.Count)
                keywordCount = tfidfDict.Count;

            //6、用一个数组保存tfidf字典的keys，这些key已排序
            string[] keywordArr = new string[tfidfDict.Count];
            tfidfDict.Keys.CopyTo(keywordArr, 0);

            //7、在关键词数组里取出前几个关键词返回给调用者
            List<string> result = new List<string>(keywordCount);
            int tempCount = 0;
            foreach (string str in keywordArr)
            {
                tempCount++;
                result.Add(str);
                if (tempCount >= keywordCount) break;
            }
            return result;
        }

        /// <summary>
        /// 把一个集合按重复次数排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputList"></param>
        /// <returns></returns>
        public static Dictionary<T, int> SortByDuplicateCount<T>(IList<T> inputList)
        {
            //用于计算每个元素出现的次数，key是元素，value是出现次数
            Dictionary<T, int> distinctDict = new Dictionary<T, int>();
            for (int i = 0; i < inputList.Count; i++)
            {
                //这里没用trygetvalue，会计算两次hash
                if (distinctDict.ContainsKey(inputList[i]))
                    distinctDict[inputList[i]]++;
                else
                    distinctDict.Add(inputList[i], 1);
            }

            Dictionary<T, int> sortByValueDict = GetSortByValueDict(distinctDict);
            return sortByValueDict;
        }


        /// <summary>
        /// 把一个字典俺value的顺序排序
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="distinctDict"></param>
        /// <returns></returns>
        public static Dictionary<K, V> GetSortByValueDict<K, V>(IDictionary<K, V> distinctDict)
        {
            //用于给tempDict.Values排序的临时数组
            V[] tempSortList = new V[distinctDict.Count];
            distinctDict.Values.CopyTo(tempSortList, 0);
            Array.Sort(tempSortList); //给数据排序
            Array.Reverse(tempSortList);//反转

            //用于保存按value排序的字典
            Dictionary<K, V> sortByValueDict =
                new Dictionary<K, V>(distinctDict.Count);
            for (int i = 0; i < tempSortList.Length; i++)
            {
                foreach (KeyValuePair<K, V> pair in distinctDict)
                {
                    //比较两个泛型是否相当要用Equals，不能用==操作符
                    if (pair.Value.Equals(tempSortList[i]) && !sortByValueDict.ContainsKey(pair.Key))
                        sortByValueDict.Add(pair.Key, pair.Value);
                }
            }
            return sortByValueDict;
        }

        /// <summary>
        /// 对一个数组进行排重
        /// </summary>
        /// <param name="scanKeys"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetDistinctWords<T>(IEnumerable<T> scanKeys)
        {
            T temp = default(T);
            if (scanKeys.Equals(temp))
                return new T[0];
            else
            {
                Dictionary<T, T> fixKeys = new Dictionary<T, T>();
                foreach (T key in scanKeys)
                {
                    fixKeys[key] = key;
                }
                T[] result = new T[fixKeys.Count];
                fixKeys.Values.CopyTo(result, 0);
                return result;
            }
        }

	}
}
