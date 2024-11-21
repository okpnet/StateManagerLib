using LinqExtenssions;
using StateManagerLib.StateModels;
using System.Collections;

namespace StateManagerLib.Commands
{
    public class CollectionChangeCommand:Command,ICollectionExecuteCommand,IExecuteCommand
    {
        /// <summary>
        /// どんな操作がされたか
        /// </summary>
        public CollectionOperationType Operation { get; }

        public CollectionChangeCommand(IStateBase owner,object? value) : base(owner,value)
        {
        }

        public override bool Execute()
        {
            var refValue = Owner switch
            {
                IRootState root => root.Value,
                IPropertyState property => property.Root.Value.GetValueFromPropertyPath(string.Join('.', property.Paths)),
                _ => null
            };

            if (refValue is not ICollection collection)
            {
                return false;
            }

            return Operation switch
            {
                CollectionOperationType.CollectionAdd => AddPrev(collection),
                CollectionOperationType.CollectionRemove => RemovePrev(collection),
                _ => false
            };
        }

        /// <summary>
        /// 追加を戻す=>削除
        /// </summary>
        /// <param name="collection"></param>
        protected bool AddPrev(ICollection collection)
        {
            var list = Value as IList;
            if (list is null || list.Count==0)
            {
                return false;
            }
            var conatins = list.GetType().GetMethod(nameof(IList<object>.Contains));
            var remove = collection.GetType().GetMethod(nameof(ICollection<object>.Remove));
            if (conatins is null || remove is null)
            {
                return false;
            }
            var enumrator = collection.GetEnumerator();
            var removeList = new List<object>();
            while (enumrator.MoveNext())
            {
                var val = enumrator.Current;
                if (conatins.Invoke(list, [val]) is bool result && result)
                {
                    removeList.Add(val);
                }
            }

            for (var i = 0; removeList.Count > i; i++)
            {
                remove.Invoke(collection, [removeList[i]]);
            }
            return true;
        }
        /// <summary>
        /// 削除を戻す=>追加
        /// </summary>
        /// <param name="collection"></param>
        protected bool RemovePrev(ICollection collection)
        {

            var add = collection.GetType().GetMethod(nameof(ICollection<object>.Add));
            var list=Value as IList;
            if (add is null || list is null)
            {
                return false;
            }
            var enumrator = list.GetEnumerator();
            while (enumrator.MoveNext())
            {
                var val = enumrator.Current;
                add.Invoke(list,[val]);
            }
            return true;
        }
    }
}
