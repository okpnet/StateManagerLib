using StateManagerLib.Commands;
using StateManagerLib.Internals;
using StateManagerLib.StateModels;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;

namespace StateManagerLib
{
    public class StateCollecion : IStateCollecion,IDisposable
    {
        int nest = 12;
        readonly IList<IExecuteCommand> commandList = new List<IExecuteCommand>();

        readonly IList<IStateBase> stateList = new List<IStateBase>();
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

        protected void AddState(object value)
        {
            if(value is null)
            {
                return;
            }
            var root=new RootState(value);
            var properties = root.Value.GetType().GetProperties();
            var list = new List<IPropertyState>();
            foreach (var property in properties)
            {
                var addChild = new PropertyState(root, property.Name, [property.Name]);
                if(property.PropertyType.GetInterface(nameof(INotifyPropertyChanged)) is not null)
                {
                    disposables.Add(
                        Observable.FromEventPattern<PropertyChangedEventArgs>(value, nameof(INotifyPropertyChanged.PropertyChanged)).Subscribe(t =>
                        {
                            t.EventArgs.
                        })
                    );
                }
                var descendants = GetState(property, addChild, 1);
                addChild.AddDescendantProperties(descendants);

                list.Add(addChild);
            }
            root.AddDescendantProperties(list);
        }

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
