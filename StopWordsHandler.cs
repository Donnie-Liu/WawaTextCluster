
using System;
using System.Collections;

namespace WawaSoft.Search.Common
{

    /// <summary>
    /// 用于移除停止词
    /// </summary>
	public class StopWordsHandler
	{		
		public static string[] stopWordsList=new string[] {"的",
            "我们","要","自己","之","将","“","”","，","（","）","后","应","到","某","后",
            "个","是","位","新","一","两","在","中","或","有","更","好"
		} ;

		private static readonly Hashtable _stopwords=null;

		public static object AddElement(IDictionary collection,Object key, object newValue)
		{
			object element = collection[key];
			collection[key] = newValue;
			return element;
		}

		public static bool IsStopword(string str)
		{
			
			//int index=Array.BinarySearch(stopWordsList, str)
			return _stopwords.ContainsKey(str.ToLower());
		}
	

		static StopWordsHandler()
		{
			if (_stopwords == null)
			{
				_stopwords = new Hashtable();
				double dummy = 0;
				foreach (string word in stopWordsList)
				{
					AddElement(_stopwords, word, dummy);
				}
			}
		}
	}
}

