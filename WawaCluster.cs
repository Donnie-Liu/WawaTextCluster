using System.Collections.Generic;

namespace WawaSoft.Search.Common
{
    internal class WawaCluster
    {
        public WawaCluster(int dataindex,double[] data)
        {
            CurrentMembership.Add(dataindex);
            Mean =(double[]) data.Clone();
        }

        /// <summary>
        /// 该聚类的数据成员索引
        /// </summary>
        internal List<int> CurrentMembership = new List<int>();
        /// <summary>
        /// 该聚类的中心
        /// </summary>
        internal double[] Mean;
        /// <summary>
        /// 该方法计算聚类对象的均值 
        /// </summary>
        /// <param name="coordinates"></param>
        public void UpdateMean(double[][] coordinates)
        {
            // 根据 mCurrentMembership 取得原始资料点对象 coord ，该对象是 coordinates 的一个子集；
            //然后取出该子集的均值；取均值的算法很简单，可以把 coordinates 想象成一个 m*n 的距阵 ,
            //每个均值就是每个纵向列的取和平均值 , //该值保存在 mCenter 中

            for (int i = 0; i < CurrentMembership.Count; i++)
            {
                double[] coord = coordinates[CurrentMembership[i]];
                for (int j = 0; j < coord.Length; j++)
                {
                    Mean[j] += coord[j]; // 得到每个纵向列的和；
                }
                for (int k = 0; k < Mean.Length; k++)
                {
                    Mean[k] /= CurrentMembership.Count; // 对每个纵向列取平均值
                }
            }
        }
    }
}
