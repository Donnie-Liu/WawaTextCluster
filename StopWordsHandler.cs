
using System;
using System.Collections;

namespace WawaSoft.Search.Common
{

    /// <summary>
    /// �����Ƴ�ֹͣ��
    /// </summary>
	public class StopWordsHandler
	{		
		public static string[] stopWordsList=new string[] {"��",
            "����","Ҫ","�Լ�","֮","��","��","��","��","��","��","��","Ӧ","��","ĳ","��",
            "��","��","λ","��","һ","��","��","��","��","��","��","��"
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

