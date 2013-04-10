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
            //1、获取文档输入
            string[] docs = getInputDocs("test.txt");
            if (docs.Length < 1)
            {
                Console.WriteLine("没有文档输入");
                Console.Read();
                return;
            }

            //2、初始化TFIDF测量器，用来生产每个文档的TFIDF权重
            TFIDFMeasure tf = new TFIDFMeasure(docs, new Tokeniser());

            int K = 5; //聚成3个聚类

            //3、生成k-means的输入数据，是一个联合数组，第一维表示文档个数，
            //第二维表示所有文档分出来的所有词
            double[][] data = new double[docs.Length][];
            int docCount = docs.Length; //文档个数
            int dimension = tf.NumTerms;//所有词的数目
            for (int i = 0; i < docCount; i++)
            {
                //for (int j = 0; j < dimension; j++)
                {
                    data[i] = tf.GetTermVector2(i); //获取第i个文档的TFIDF权重向量
                }
            }

            //4、初始化k-means算法，第一个参数表示输入数据，第二个参数表示要聚成几个类
            WawaKMeans kmeans = new WawaKMeans(data, K);
            //5、开始迭代
            kmeans.Start();

            //6、获取聚类结果并输出
            WawaCluster[] clusters = kmeans.Clusters;
            StringBuilder sb = new StringBuilder();
            foreach (WawaCluster cluster in clusters)
            {
                List<int> members = cluster.CurrentMembership;

                //获取该聚类的关键词并打印
                IEnumerable<string> keywords = tf.GetKeyword(cluster.CurrentMembership, 1);
                StringBuilder sbTemp = new StringBuilder();
                sbTemp.Append("---------");
                foreach (string s in keywords)
                {
                    sbTemp.AppendFormat("{0},", s);
                }
                sbTemp.Append("-------\r\n");
                Console.WriteLine(sbTemp);

                //打印该聚类的成员
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
        /// 获取文档输入
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
