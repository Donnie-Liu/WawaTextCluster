using System;

namespace WawaSoft.Search.Common
{

    public class WawaKMeans
    {
        /// <summary>
        /// ���ݵ�����
        /// </summary>
        readonly int _coordCount;
        /// <summary>
        /// ԭʼ����
        /// </summary>
        readonly double[][] _coordinates;
        /// <summary>
        /// ���������
        /// </summary>
        readonly int _k;
        /// <summary>
        /// ����
        /// </summary>
        private readonly WawaCluster[] _clusters;

        internal WawaCluster[] Clusters
        {
            get { return _clusters; }
        } 

        /// <summary>
        /// ����һ���������ڼ�¼�͸���ÿ�����ϵ������ĸ�Ⱥ����
        /// _clusterAssignments[j]=i;// ��ʾ�� j �����ϵ�������ڵ� i ��Ⱥ����
        /// </summary>
        readonly int[] _clusterAssignments;
        /// <summary>
        /// ����һ���������ڼ�¼�͸���ÿ�����ϵ���������
        /// </summary>
        private readonly int[] _nearestCluster;
        /// <summary>
        /// ����һ������������ʾ���ϵ㵽���ĵ�ľ���,
        /// ���С�_distanceCache[i][j]��ʾ��i�����ϵ㵽��j��Ⱥ�۶������ĵ�ľ��룻
        /// </summary>
        private readonly double[,] _distanceCache;
        /// <summary>
        /// ������ʼ�����������
        /// </summary>
        private static readonly Random _rnd = new Random(1);

        public WawaKMeans(double[][] data, int K)
        {
            _coordinates = (double[][])data.Clone();
            _coordCount = data.Length;
            _k = K;
            _clusters = new WawaCluster[K];
            _clusterAssignments = new int[_coordCount];
            _nearestCluster = new int[_coordCount];
            _distanceCache = new double[_coordCount,data.Length];
            InitRandom();
        }

        public void Start()
        {
            int iter = 0;
            while (true)
            {
                Console.WriteLine("Iteration " + (iter++) + "...");
                //1�����¼���ÿ������ľ�ֵ
                for (int i = 0; i < _k; i++)
                {
                    _clusters[i].UpdateMean(_coordinates);
                }

                //2������ÿ�����ݺ�ÿ���������ĵľ���
                for (int i = 0; i < _coordCount; i++)
                {
                    for (int j = 0; j < _k; j++)
                    {
                        double dist = getDistance(_coordinates[i], _clusters[j].Mean);
                        _distanceCache[i,j] = dist;
                    }
                }

                //3������ÿ���������ĸ��������
                for (int i = 0; i < _coordCount; i++)
                {
                    _nearestCluster[i] = nearestCluster(i);
                }

                //4���Ƚ�ÿ����������ľ����Ƿ�����������ľ���
                //���ȫ��ȱ�ʾ���еĵ��Ѿ�����Ѿ����ˣ�ֱ�ӷ��أ�
                int k = 0;
                for (int i = 0; i < _coordCount; i++)
                {
                    if (_nearestCluster[i] == _clusterAssignments[i])
                        k++;

                }
                if (k+3 >= _coordCount)
                    break;

                //5��������Ҫ���µ������ϵ��Ⱥ����Ĺ�ϵ��������Ϻ������¿�ʼѭ����
                //��Ҫ�޸�ÿ������ĳ�Ա�ͱ�ʾĳ�����������ĸ�����ı���
                for (int j = 0; j < _k; j++)
                {
                    _clusters[j].CurrentMembership.Clear();
                }
                for (int i = 0; i < _coordCount; i++)
                {
                    _clusters[_nearestCluster[i]].CurrentMembership.Add(i);
                    _clusterAssignments[i] = _nearestCluster[i];
                }
                
            }

        }

        /// <summary>
        /// ����ĳ���������ĸ��������
        /// </summary>
        /// <param name="ndx"></param>
        /// <returns></returns>
        int nearestCluster(int ndx)
        {
            int nearest = -1;
            double min = Double.MaxValue;
            for (int c = 0; c < _k; c++)
            {
                double d = _distanceCache[ndx,c];
                if (d < min)
                {
                    min = d;
                    nearest = c;
                }
          
            }
            if(nearest==-1)
            {
                ;
            }
            return nearest;
        }
        /// <summary>
        /// ����ĳ������ĳ�������ĵľ���
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        static double getDistance(double[] coord, double[] center)
        {
            //int len = coord.Length;
            //double sumSquared = 0.0;
            //for (int i = 0; i < len; i++)
            //{
            //    double v = coord[i] - center[i];
            //    sumSquared += v * v; //ƽ����
            //}
            //return Math.Sqrt(sumSquared);

            //Ҳ���������Ҽн�������ĳ������ĳ�������ĵľ���
            return 1- TermVector.ComputeCosineSimilarity(coord, center);

        } 
        /// <summary>
        /// �����ʼ��k������
        /// </summary>
        private void InitRandom()
        {
            for (int i = 0; i < _k; i++)
            {
                int temp = _rnd.Next(_coordCount);
                _clusterAssignments[temp] = i; //��¼��temp���������ڵ�i������
                _clusters[i] = new WawaCluster(temp,_coordinates[temp]);
            }
        }
    }
}
