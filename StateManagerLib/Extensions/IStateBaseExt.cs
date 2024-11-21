using LinqExtenssions;
using StateManagerLib.Commands;
using StateManagerLib.StateModels;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reflection;

namespace StateManagerLib.Extensions
{
    public static class IStateBaseExt
    {
        /// <summary>
        /// オブジェクトのプロパティからI
        /// </summary>
        /// <param name="stateBase"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<IPropertyState> GetPropertyStates(this IStateBase stateBase, object value)
        {
            var properties = value.GetType().
                GetProperties(BindingFlags.Public | BindingFlags.Instance).
                Where(t => typeof(INotifyPropertyChanged).IsAssignableFrom(t.PropertyType) ||
                typeof(INotifyCollectionChanged).IsAssignableFrom(t.PropertyType));
            foreach (var property in properties)
            {
                var path = stateBase switch
                {
                    IPropertyState state => state.Paths.Append(property.Name).ToArray(),
                    _ => []
                };
                yield return new PropertyState(stateBase, property.Name, path);
            }
        }
        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="list"></param>
        public static void ClaerList(this IList<IPropertyState> list)
        {
            foreach (var property in list)
            {
                if(property is PropertyState state)
                {
                    state.Dispose();
                }
                list.Clear();
            }
        }
        /// <summary>
        /// INotifyPropertyChangedサブスクライバ
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IDisposable CreateINotifyPropertyChangedSubscriber(this IStateBase stateful, object value)
        {
            return 
                Observable.FromEventPattern<PropertyChangedEventArgs>(value, nameof(INotifyPropertyChanged.PropertyChanged)).Subscribe(t =>
                {

                    var propName = t.EventArgs.PropertyName;
                    if(propName is (null or "") || stateful is not StateBase stateBase)
                    {
                        return;
                    }
                    var changeValue = value.GetValueFromPropertyPath(propName);
                    stateBase.Change(new PropertyChangeCommand(stateBase, propName, changeValue));

                    if (stateful is not IReferenceState referenceState  || !referenceState.Properties.FirstOrDefault(t=>t.Name==propName).IfDeclare(out var propertyState))
                    {
                        return;
                    }
                    ((IDisposable)propertyState!).Dispose();
                    if (changeValue is null)
                    {
                        return;
                    }
                    propertyState!.AddNotifyChangedListener(changeValue);
                    propertyState.GetPropertyStates(changeValue);
                });
        }
        /// <summary>
        /// INotifyCollectionChangedサブスクライバ
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IDisposable CreateINotifyCollectionChangedSubscriber(this IStateBase stateful, object value)
        {
            return
                Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(value, nameof(INotifyCollectionChanged.CollectionChanged)).Subscribe(t =>
                {
                    if(value is not null)
                    {
                        //コマンド追加
                        var cmd = t.EventArgs.Action switch
                        {
                            NotifyCollectionChangedAction.Add => new CollectionChangeCommand(stateful, t.EventArgs.NewItems!),
                            NotifyCollectionChangedAction.Remove => new CollectionChangeCommand(stateful, t.EventArgs.OldItems!),
                            NotifyCollectionChangedAction.Reset=> new CollectionChangeCommand(stateful, t.EventArgs.OldItems!),
                            _=>throw new NotImplementedException()
                        };

                        var root = stateful switch
                        {
                            IPropertyState propertyState => propertyState.Root,
                            IRootState rootState => rootState,
                            _ => throw new NotImplementedException()
                        };
                        
                        var stateCollection = root.CommandStack as IStateCollecion;
                        if(stateCollection is not null)
                        {
                            if (t.EventArgs.NewItems is not null)
                            {
                                foreach (var newItem in t.EventArgs.NewItems)
                                {
                                    if (newItem is null)
                                    {
                                        continue;
                                    }
                                    stateCollection.Add(newItem);
                                }
                            }

                            if(t.EventArgs.OldItems is not null && root.CommandStack is IStateCollecion collection)
                            {
                                foreach (var oldItem in t.EventArgs.OldItems)
                                {
                                    if (oldItem is null)
                                    {
                                        continue;
                                    }
                                    stateCollection.Remove(oldItem);
                                }
                            }
                        }
                        root.CommandStack.PushCommandOnStack(cmd);
                    }
                    else
                    {
                        if(stateful is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    }
                });
        }
        /// <summary>
        /// ルートがもつ全てのIStateインターフェイス
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static IEnumerable<IStateBase> GetAllDescendants(this IRootState root) => Recursive(root);
        /// <summary>
        /// 子の再帰取得
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static IEnumerable<IStateBase> Recursive(IStateBase state)
        {
            yield return state;
            if(state is not IReferenceState refState)
            {
                yield break;
            }
            foreach(var property in refState.Properties)
            {
                yield return property;
                var reults = Recursive(property);
                foreach (var reult in reults)
                {
                    yield return reult;
                }
            }
        }
    }
}
