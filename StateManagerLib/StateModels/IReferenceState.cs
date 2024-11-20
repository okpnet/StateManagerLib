using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateManagerLib.StateModels
{
    /// <summary>
    /// 
    /// </summary>
    public interface IReferenceState
    {
        /// <summary>
        /// プロパティタイプのプロパティ
        /// </summary>
        IEnumerable<IPropertyState> Properties { get; }
    }
}
