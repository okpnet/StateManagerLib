using StateManagerLib.Collections;
using StateManagerLib.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateManagerLib.StateModels
{
    public class PropertyState:IPropertyState,IReferenceState,IStateBase,IDisposable
    {
        IDisposable? disposable;
        protected IList<IPropertyState> properties=new List<IPropertyState>();
        /// <summary>
        /// 状態に対して行った変更
        /// </summary>
        public ICommandCollection Commands { get; }
        /// <summary>
        /// 親の状態インターフェイス
        /// </summary>
        public IStateBase Parent { get; } = default!;
        /// <summary>
        /// プロパティの名前
        /// </summary>
        public string Name { get; }=string.Empty;
        /// <summary>
        /// ルートからのこのプロパティまでのパス
        /// </summary>
        public string[] Paths { get; set; } = [];
        /// <summary>
        /// プロパティタイプのプロパティ
        /// </summary>
        public IEnumerable<IPropertyState> Properties => properties;
        /// <summary>
        /// プロパティタイプのプロパティの追加
        /// </summary>
        /// <param name="propertyState"></param>
        public void AddDescendantProperty(IPropertyState propertyState)=>properties.Add(propertyState);
        /// <summary>
        /// プロパティタイプのプロパティの追加
        /// </summary>
        /// <param name="propertyStates"></param>
        public void AddDescendantProperties(IEnumerable<IPropertyState> propertyStates)=>((List<IPropertyState>)properties).AddRange(propertyStates);

        public PropertyState(IStateBase parent, string name, string[] paths)
        {
            Parent = parent;
            Name = name;
            Paths = paths;
        }

        public PropertyState(IStateBase parent, string name, string[] paths,IEnumerable<IPropertyState> propertyStates):this(parent, name, paths)
        {
            properties=propertyStates.ToList();
        }

        public void AddPropertyChangeObserver(object value)
        {
            if(value is INotifyPropertyChanged)
            {
                disposable = Observable.FromEventPattern<PropertyChangedEventArgs>(value, nameof(INotifyPropertyChanged.PropertyChanged)).Subscribe(t =>
                {
                    if (t.EventArgs.PropertyName != Name)
                    {
                        return;
                    }
                    var path = string.Join('.', Paths);
                    var val = value.GetValueFromPropertyPath(Name);
                    //コマンド追加

                    if (val is INotifyPropertyChanged notify || val is INotifyCollectionChanged)
                    {//イベント追加
                        foreach (var prop in properties)
                        {
                            prop.AddPropertyChangeObserver(val);
                        }
                    }
                });
            }
            else if(value is INotifyCollectionChanged)
            {
                disposable = Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(value, nameof(INotifyPropertyChanged.PropertyChanged)).Subscribe(t =>
                {
                    if (t.EventArgs. != Name)
                    {
                        return;
                    }
                    var path = string.Join('.', Paths);
                    var val = value.GetValueFromPropertyPath(Name);
                    //コマンド追加

                    if (val is INotifyPropertyChanged notify || val is INotifyCollectionChanged)
                    {//イベント追加
                        foreach (var prop in properties)
                        {
                            prop.AddPropertyChangeObserver(val);
                        }
                    }
                });
            }
        }



        public void Dispose()
        {
            disposable?.Dispose();
        }
    }
}
