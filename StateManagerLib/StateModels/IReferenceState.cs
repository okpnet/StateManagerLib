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
        /// <summary>
        /// プロパティタイプのプロパティの追加
        /// </summary>
        /// <param name="propertyState"></param>
        void AddDescendantProperty(IPropertyState propertyState);
        /// <summary>
        /// プロパティタイプのプロパティの追加
        /// </summary>
        /// <param name="propertyStates"></param>
        void AddDescendantProperties(IEnumerable<IPropertyState> propertyStates);
    }
}
