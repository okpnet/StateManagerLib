using LinqExtenssions;
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

        public PropertyChangeCommand(IStateBase owner,string name,object? value) : base(owner,value)
        {
            Name = name;
        }
        /// <summary>
        /// 戻す
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            var refValue=Owner switch
            {
                IRootState root => root.Value,
                IPropertyState property => property.Root.Value.GetValueFromPropertyPath(string.Join('.', property.Paths)),
                _ => null
            };
            if(refValue is null)
            {
                return false;
            } 

            var propinfo = refValue.GetType().GetProperty(Name);
            if (propinfo is null)
            {
                throw new ArgumentNullException($"`refValue` has not {Name} properties.");
            }
            propinfo.SetValue(refValue, Value);
            return true;
        }
    }
}
