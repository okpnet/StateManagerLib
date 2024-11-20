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
        public static IDisposable CreateINotifyPropertyChangedSubscriber(this IEnumerable<IPropertyState> propertyStates, object value)
        {
            return 
                Observable.FromEventPattern<PropertyChangedEventArgs>(value, nameof(INotifyPropertyChanged.PropertyChanged)).Subscribe(t =>
                {
                    var propName = t.EventArgs.PropertyName;
                    
                    if (propName is (null or "") || !propertyStates.FirstOrDefault(t=>t.Name==propName).IfDeclare(out var propertyState))
                    {
                        return;
                    }

                    if (value.GetValueFromPropertyPath(propName).IfDeclare(out var result))
                    {
                        ((IDisposable)propertyState!).Dispose();
                        //コマンド追加
                        var cmd = new PropertyChangeCommand(propertyState, propName, result);
                        propertyState!.Root.CommandStack.PushCommandOnStack(cmd);
                        propertyState.AddCommandToState(cmd);
                        propertyState!.AddNotifyChangedListener(result!);
                        propertyState.GetPropertyStates(result!);
                    }
                    else
                    {
                        ((IDisposable)propertyState!).Dispose();
                    }
                });
        }
        /// <summary>
        /// INotifyCollectionChangedサブスクライバ
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IDisposable CreateINotifyCollectionChangedSubscriber(this IStateBase property, object value)
        {
            return
                Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(value, nameof(INotifyCollectionChanged.CollectionChanged)).Subscribe(t =>
                {
                    if(value is not null)
                    {
                        //コマンド追加
                        var cmd = t.EventArgs.Action switch
                        {
                            NotifyCollectionChangedAction.Add => new CollectionChangeCommand(property, t.EventArgs.NewItems!),
                            NotifyCollectionChangedAction.Remove => new CollectionChangeCommand(property, t.EventArgs.OldItems!),
                            NotifyCollectionChangedAction.Reset=> new CollectionChangeCommand(property, t.EventArgs.OldItems!),
                            _=>throw new NotImplementedException()
                        };

                        var root = property switch
                        {
                            IPropertyState propertyState => propertyState.Root,
                            IRootState rootState => rootState,
                            _ => throw new NotImplementedException()
                        };
                        root.CommandStack.PushCommandOnStack(cmd);
                    }
                    else
                    {
                        if(property is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    }
                });
        }
    }
}
