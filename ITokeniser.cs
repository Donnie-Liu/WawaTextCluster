using System.Collections.Generic;

namespace WawaSoft.Search.Common
{
    /// <summary>
    /// �ִ����ӿ�
    /// </summary>
    public interface ITokeniser
    {
        IList<string> Partition(string input);
    }
}