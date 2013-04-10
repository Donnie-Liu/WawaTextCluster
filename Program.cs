using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WawaSoft.Search.Common;

namespace WawaSoft.Search.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //1����ȡ�ĵ�����
            string[] docs = getInputDocs("test.txt");
            if (docs.Length < 1)
            {
                Console.WriteLine("û���ĵ�����");
                Console.Read();
                return;
            }

            //2����ʼ��TFIDF����������������ÿ���ĵ���TFIDFȨ��
            TFIDFMeasure tf = new TFIDFMeasure(docs, new Tokeniser());

            int K = 5; //�۳�3������

            //3������k-means���������ݣ���һ���������飬��һά��ʾ�ĵ�������
            //�ڶ�ά��ʾ�����ĵ��ֳ��������д�
            double[][] data = new double[docs.Length][];
            int docCount = docs.Length; //�ĵ�����
            int dimension = tf.NumTerms;//���дʵ���Ŀ
            for (int i = 0; i < docCount; i++)
            {
                //for (int j = 0; j < dimension; j++)
                {
                    data[i] = tf.GetTermVector2(i); //��ȡ��i���ĵ���TFIDFȨ������
                }
            }

            //4����ʼ��k-means�㷨����һ��������ʾ�������ݣ��ڶ���������ʾҪ�۳ɼ�����
            WawaKMeans kmeans = new WawaKMeans(data, K);
            //5����ʼ����
            kmeans.Start();

            //6����ȡ�����������
            WawaCluster[] clusters = kmeans.Clusters;
            StringBuilder sb = new StringBuilder();
            foreach (WawaCluster cluster in clusters)
            {
                List<int> members = cluster.CurrentMembership;

                //��ȡ�þ���Ĺؼ��ʲ���ӡ
                IEnumerable<string> keywords = tf.GetKeyword(cluster.CurrentMembership, 1);
                StringBuilder sbTemp = new StringBuilder();
                sbTemp.Append("---------");
                foreach (string s in keywords)
                {
                    sbTemp.AppendFormat("{0},", s);
                }
                sbTemp.Append("-------\r\n");
                Console.WriteLine(sbTemp);

                //��ӡ�þ���ĳ�Ա
                sb.Append(sbTemp.ToString());
                foreach (int i in members)
                {
                    Console.WriteLine(docs[i]);
                    sb.AppendFormat("{0}\r\n", docs[i]);
                }
            }
            Console.Read();
        }

        /// <summary>
        /// ��ȡ�ĵ�����
        /// </summary>
        /// <returns></returns>
        private static string[] getInputDocs(string file)
        {
            List<string> ret = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader(file, Encoding.Default))
                {
                    string temp;
                    while ((temp = sr.ReadLine()) != null)
                    {
                        ret.Add(temp);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return ret.ToArray();
        }

  
    }
}
