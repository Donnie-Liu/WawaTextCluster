using System.Collections.Generic;

namespace WawaSoft.Search.Common
{
    /// <summary>
    /// ·Ö´ÊÆ÷½Ó¿Ú
    /// </summary>
    public interface ITokeniser
    {
        IList<string> Partition(string input);
    }
}