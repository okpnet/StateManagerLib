using LinqExtenssions;
using StateManagerLib.Commands;
using StateManagerLib.Extensions;
using StateManagerLib.StateModels;
using System.Collections;
using System.Reactive.Linq;

namespace StateManagerLib
{
    /// <summary>
    /// 状態オブジェクトコレクション
    /// </summary>
    public class StateCollecion : StateCollectionBase,IStateCollecion, ICommandStack,IDisposable
    {
        /// <summary>
        /// 戻した実行実績
        /// </summary>
        readonly LinkedList<IExecuteCommand> discardedCommands = new ();  
        /// <summary>
        /// Stateの一覧
        /// </summary>
        readonly IList<IRootState> stateList = [];
        
        /// <summary>
        /// 状態管理の登録をしたオブジェクト
        /// </summary>
        public IEnumerable<object> StateValues => stateList.Select(t => t.Value);

        public int Count => stateList.Count;

        public bool IsReadOnly => false;

        public object this[int index] 
        {
            get=> stateList[index].Value;
            set 
            {
                var val = stateList[index].Value;
                if (Remove(val))
                {
                    Insert(index, new RootState(value,this));
                }
            }
        }

        public override void Dispose()
        {
            discardedCommands.Clear();
            foreach(var state in stateList)
            {
                ((IDisposable)state).Dispose();
            }
            stateList.Clear();
            base.Dispose();
        }
        /// <summary>
        /// 元に戻す
        /// </summary>
        public void Undo()
        {
            if(current is not null)
            {
                discardedCommands.AddFirst(current);
                current = null;
            }
            if(executeCommands.FirstOrDefault().IfDeclare(out var cmd))
            {
                current = cmd!;
                current.ToExecute();
                executeCommands.RemoveFirst();
            }
        }
        /// <summary>
        /// 引数のオブジェクトだけ戻す
        /// 追加されていないときは何もしない
        /// </summary>
        /// <param name="value"></param>
            
        public void Undo(object value)
        {
            if (!stateList.FirstOrDefault(t => ReferenceEquals(t.Value, value) | Equals(t.Value, value)).IfDeclare(out var root))
            {
                return;
            }
            if (!executeCommands.FirstOrDefault(t => root!.GetAllDescendants().Contains(t.Owner)).IfDeclare(out var cmd))
            {
                return;
            }
            if (current is not null)
            {
                discardedCommands.AddFirst(current);
                current = null;
            }
            current = cmd!;
            current.ToExecute();
            executeCommands.Remove(cmd!);
        }
        /// <summary>
        /// 元に戻す
        /// </summary>
        public void Redo()
        {
            if(current is not null)
            {
                executeCommands.AddFirst(current);
                current = null;
            }
            if (discardedCommands.FirstOrDefault().IfDeclare(out var cmd))
            {
                current= cmd!;
                current.ToExecute();
                discardedCommands.RemoveFirst();
            }
        }
        /// <summary>
        /// 引数のオブジェクトだけ元に戻す
        /// リストに無いときは何もしない
        /// </summary>
        /// <param name="value"></param>
        public void Redo(object value)
        {
            if(!stateList.FirstOrDefault(t=>ReferenceEquals(t.Value,value) | Equals(t.Value,value)).IfDeclare(out var root))
            {
                return;
            }
            if (!discardedCommands.FirstOrDefault(t => root!.GetAllDescendants().Contains(t.Owner)).IfDeclare(out var cmd))
            {
                return;
            }
            if (current is not null)
            {
                executeCommands.AddFirst(current);
                current = null;
            }
            current = cmd!;
            current.ToExecute();
            discardedCommands.Remove(cmd!);
        }


        public int IndexOf(object item)
        {
            var stateItem = stateList.FirstOrDefault(t => ReferenceEquals(t.Value, item) | Equals(t.Value, item));
            if(stateItem is null)
            {
                return -1;
            }
            return stateList.IndexOf(stateItem);
        }

        public void Insert(int index, object item)
        {
            if (item is null || stateList.Any(t => ReferenceEquals(t.Value, item)))
            {
                return;
            }
            stateList.Insert(index,new RootState(item, this));
        }

        public void RemoveAt(int index)
        {
            stateList.RemoveAt(index);
        }

        public void Add(object item)
        {
            if (item is null || stateList.Any(t=>ReferenceEquals(t.Value,item) | Equals(t.Value,item)))
            {
                return;
            }
            stateList.Add(new RootState(item, this));
        }

        public void Clear()
        {
            stateList.Clear();
            discardedCommands.Clear();
            executeCommands.Clear();
        }

        public bool Contains(object item)
        {
            return stateList.Any(t=>t.Value == item);
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            Array.Copy(stateList.Select(t => t.Value).ToArray(), array, Count);
        }

        public bool Remove(object item)
        {
            var removeItem=stateList.FirstOrDefault(t=>t.Value == item);
            if(removeItem is not null)
            {
                ((IDisposable)removeItem).Dispose();
                return stateList.Remove(removeItem);
            }
            return false;
        }

        public IEnumerator<object> GetEnumerator()
        {
            return stateList.Select(t => t.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
