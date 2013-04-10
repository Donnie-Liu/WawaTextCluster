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
        /// ��ȡĳ���ĵ��Ĺؼ���
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<string> GetKeyword(List<int> arr, int count)
        {
            //1����ÿ���ĵ��ִʲ�������һ���б���
            List<string> allWords = new List<string>();
            foreach (int i in arr)
            {
                //�����һ���ĵ�����ֵĶ���ʽ�������
                allWords.AddRange(GetDistinctWords(_tokenizer.Partition(_docs[i])));
            }

            //2����һ���ֵ䱣��ʵĴ�Ƶ,key�Ǵʣ�value���ظ�����
            Dictionary<string, int> tfDict = SortByDuplicateCount(allWords);

            //3������������Ĵ�Ƶ�ֵ䣬����ȡÿ���ʵ�IDFֵ�����Ѹ��º�Ľ������һ��tfidfDict�ʵ�
            //�ôʵ��key�Ǵʣ�value��tfidfֵ
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

            //4����tfidf�ֵ䰳Ȩ������
            tfidfDict = GetSortByValueDict(tfidfDict);

            //5������Ҫ��ȡ�Ĺؼ�������
            int keywordCount = count;
            if (keywordCount > tfidfDict.Count)
                keywordCount = tfidfDict.Count;

            //6����һ�����鱣��tfidf�ֵ��keys����Щkey������
            string[] keywordArr = new string[tfidfDict.Count];
            tfidfDict.Keys.CopyTo(keywordArr, 0);

            //7���ڹؼ���������ȡ��ǰ�����ؼ��ʷ��ظ�������
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
        /// ��һ�����ϰ��ظ���������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputList"></param>
        /// <returns></returns>
        public static Dictionary<T, int> SortByDuplicateCount<T>(IList<T> inputList)
        {
            //���ڼ���ÿ��Ԫ�س��ֵĴ�����key��Ԫ�أ�value�ǳ��ִ���
            Dictionary<T, int> distinctDict = new Dictionary<T, int>();
            for (int i = 0; i < inputList.Count; i++)
            {
                //����û��trygetvalue�����������hash
                if (distinctDict.ContainsKey(inputList[i]))
                    distinctDict[inputList[i]]++;
                else
                    distinctDict.Add(inputList[i], 1);
            }

            Dictionary<T, int> sortByValueDict = GetSortByValueDict(distinctDict);
            return sortByValueDict;
        }


        /// <summary>
        /// ��һ���ֵ䰳value��˳������
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="distinctDict"></param>
        /// <returns></returns>
        public static Dictionary<K, V> GetSortByValueDict<K, V>(IDictionary<K, V> distinctDict)
        {
            //���ڸ�tempDict.Values�������ʱ����
            V[] tempSortList = new V[distinctDict.Count];
            distinctDict.Values.CopyTo(tempSortList, 0);
            Array.Sort(tempSortList); //����������
            Array.Reverse(tempSortList);//��ת

            //���ڱ��水value������ֵ�
            Dictionary<K, V> sortByValueDict =
                new Dictionary<K, V>(distinctDict.Count);
            for (int i = 0; i < tempSortList.Length; i++)
            {
                foreach (KeyValuePair<K, V> pair in distinctDict)
                {
                    //�Ƚ����������Ƿ��൱Ҫ��Equals��������==������
                    if (pair.Value.Equals(tempSortList[i]) && !sortByValueDict.ContainsKey(pair.Key))
                        sortByValueDict.Add(pair.Key, pair.Value);
                }
            }
            return sortByValueDict;
        }

        /// <summary>
        /// ��һ�������������
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
