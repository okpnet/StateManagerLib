using StateManagerLib.StateModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateManagerLib.Commands
{
    /// <summary>
    /// プロパティ変更コマンド
    /// </summary>
    public class PropertyChangeCommand : Command, IPropertyChangeCommand, IExecuteCommand
    {
        /// <summary>
        /// プロパティ名
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// オーナー
        /// </summary>
        public new IPropertyState Owner
        {
            get
            {
                if(base.Owner is not PropertyState property)
                {
                    throw new InvalidOperationException();
                }
                return property;
            }
        }
        /// <summary>
        /// セットされたインスタンス
        /// </summary>
        public object? Value { get; set; }

        public PropertyChangeCommand(IPropertyState owner,string name,object? value) : base(owner)
        {
            Value = value;
            Name = name;
        }
        /// <summary>
        /// 戻す
        /// </summary>
        /// <returns></returns>
        public override bool Execute(object refValue)
        {
            if(refValue is null)
            {
                return false;
            } 

            var propinfo = refValue.GetType().GetProperty(Owner.Name);
            if (propinfo is null)
            {
                throw new ArgumentNullException($"`refValue` has not {Owner.Name} properties.");
            }
            propinfo.SetValue(refValue, Value);
            return true;
        }
    }
}
