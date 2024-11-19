using StateManagerLib.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateManagerLib.StateModels
{
    public class RootState : IRootState,IReferenceState, IStateBase
    {
        protected IList<IPropertyState> properties = new List<IPropertyState>();
        /// <summary>
        /// ルートオブジェクト
        /// </summary>
        public object Value { get; }
        /// <summary>
        /// 状態に対して行った変更
        /// </summary>
        public ICommandCollection Commands { get; }
        /// <summary>
        /// 親の状態インターフェイス
        /// </summary>
        public IStateBase Parent { get; } = default!;
        /// <summary>
        /// プロパティタイプのプロパティ
        /// </summary>
        public IEnumerable<IPropertyState> Properties => properties;
        /// <summary>
        /// プロパティタイプのプロパティの追加
        /// </summary>
        /// <param name="propertyState"></param>
        public void AddDescendantProperty(IPropertyState propertyState) => properties.Add(propertyState);
        /// <summary>
        /// プロパティタイプのプロパティの追加
        /// </summary>
        /// <param name="propertyStates"></param>
        public void AddDescendantProperties(IEnumerable<IPropertyState> propertyStates) => ((List<IPropertyState>)properties).AddRange(propertyStates);

        public RootState(object value)
        {
            Value = value;
        }

        public RootState(object value, IEnumerable<IPropertyState> propertyStates) : this(value)
        {
            AddDescendantProperties(propertyStates);
        }
    }
}
