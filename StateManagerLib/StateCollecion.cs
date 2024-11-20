using LinqExtenssions;
using StateManagerLib.Commands;
using StateManagerLib.Internals;
using StateManagerLib.StateModels;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;

namespace StateManagerLib
{
    public class StateCollecion : IStateCollecion,IDisposable
    {
        /// <summary>
        /// 再帰取得するネスト深さ
        /// </summary>
        int nest = 12;
        /// <summary>
        /// コマンドの一覧
        /// </summary>
        readonly IList<IExecuteCommand> commandList = new List<IExecuteCommand>();
        /// <summary>
        /// Stateの一覧
        /// </summary>
        readonly IList<IStateBase> stateList = new List<IStateBase>();
        /// <summary>
        /// オブザーバーの破棄コレクション
        /// </summary>
        readonly CompositeDisposable disposables = new CompositeDisposable();
        /// <summary>
        /// プロパティを検索する深さ
        /// </summary>
        public int FindPropertyNest 
        {
            get => nest;
            set
            {
                nest = 0 >= value ? 1 : value;
                System.Diagnostics.Debug.Assert(100 > nest, "The nested level set for property search exceeds 100.");
            }
        }
        /// <summary>
        /// 状態の追加
        /// </summary>
        /// <param name="value"></param>
        protected void AddState(object value)
        {
            if(value is null)
            {
                return;
            }
            var root=new RootState(value);
            var properties = root.Value.GetType().GetProperties();
            var list = new List<IPropertyState>();

            if(value is INotifyPropertyChanged)
            {
                disposables.Add(
                    Observable.FromEventPattern<PropertyChangedEventArgs>(value,
                    )
            }

            foreach (var property in properties)
            {
                var addChild = new PropertyState(root, property.Name, [property.Name]);
                if(property is INotifyPropertyChanged)
                {
                    disposables.Add(
                        Observable.FromEventPattern<PropertyChangedEventArgs>(value, nameof(INotifyPropertyChanged.PropertyChanged)).Subscribe(t =>
                        {
                            var propName=t.EventArgs.PropertyName;
                            if(propName is (null or "") ||
                                root.Properties.FirstOrDefault(x => x.Name == propName) is not PropertyState propertyState)
                            {
                                return;
                            }
                            if (value.GetValueFromPropertyPath(propName).IfDeclare(out var result))
                            {
                                propertyState!.AddPropertyChangeObserver(result!);
                            }
                            else
                            {
                                propertyState.Dispose();
                            }
                        })
                    );
                }
                var descendants = GetState(property, addChild, 1);
                addChild.AddDescendantProperties(descendants);

                list.Add(addChild);
            }
            root.AddDescendantProperties(list);
        }
        /// <summary>
        /// 再帰的に取得したプロパティをIPropertyStateに変換する
        /// </summary>
        /// <param name="property"></param>
        /// <param name="stateBase"></param>
        /// <param name="nestedLevel"></param>
        /// <returns></returns>
        protected IEnumerable<IPropertyState> GetState(PropertyInfo property, IPropertyState stateBase, int nestedLevel)
        {
            if (nestedLevel > FindPropertyNest)
            {
                yield break;
            }
            var list = new List<IPropertyState>();
            var properties = property.PropertyType.GetProperties();
            foreach (var prop in properties) 
            {
                var addChild= new PropertyState(stateBase,prop.Name, stateBase.Paths.Append(prop.Name).ToArray());
                var descendants = GetState(prop, addChild, nestedLevel++);
                addChild.AddDescendantProperties(descendants);
                yield return addChild;
            }
        }
        /// <summary>
        /// プロパティの展開
        /// </summary>
        /// <param name="findPropertyResult"></param>
        /// <returns></returns>
        internal IEnumerable<FindPropertyResult> GetProperties(FindPropertyResult findPropertyResult)
        {
            if(findPropertyResult.Property.PropertyType.GetInterface(nameof(INotifyPropertyChanged)) is not null ||
                findPropertyResult.Property.PropertyType.GetInterface(nameof(INotifyCollectionChanged)) is not null
                )
            {
                yield return findPropertyResult;
            }

            if(findPropertyResult.NestLevel > FindPropertyNest)
            {
                yield break;
            }

            var props = findPropertyResult.Property.PropertyType.GetProperties();
            foreach (var prop in props)
            {
                var results = GetProperties(new FindPropertyResult(prop, findPropertyResult.Path.Append(prop.Name).ToArray(), findPropertyResult.NestLevel + 1));
                foreach(var result in results)
                {
                    yield return result;
                }
            }
        }

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(IStateBase item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(IStateBase item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IStateBase[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IStateBase> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(IStateBase item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            disposables.Clear();
        }
    }
}
