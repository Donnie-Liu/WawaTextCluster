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
        /// �þ�������ݳ�Ա����
        /// </summary>
        internal List<int> CurrentMembership = new List<int>();
        /// <summary>
        /// �þ��������
        /// </summary>
        internal double[] Mean;
        /// <summary>
        /// �÷�������������ľ�ֵ 
        /// </summary>
        /// <param name="coordinates"></param>
        public void UpdateMean(double[][] coordinates)
        {
            // ���� mCurrentMembership ȡ��ԭʼ���ϵ���� coord ���ö����� coordinates ��һ���Ӽ���
            //Ȼ��ȡ�����Ӽ��ľ�ֵ��ȡ��ֵ���㷨�ܼ򵥣����԰� coordinates �����һ�� m*n �ľ��� ,
            //ÿ����ֵ����ÿ�������е�ȡ��ƽ��ֵ , //��ֵ������ mCenter ��

            for (int i = 0; i < CurrentMembership.Count; i++)
            {
                double[] coord = coordinates[CurrentMembership[i]];
                for (int j = 0; j < coord.Length; j++)
                {
                    Mean[j] += coord[j]; // �õ�ÿ�������еĺͣ�
                }
                for (int k = 0; k < Mean.Length; k++)
                {
                    Mean[k] /= CurrentMembership.Count; // ��ÿ��������ȡƽ��ֵ
                }
            }
        }
    }
}
