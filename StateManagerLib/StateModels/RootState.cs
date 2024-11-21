using StateManagerLib.Commands;
using StateManagerLib.Extensions;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;

namespace StateManagerLib.StateModels
{
    /// <summary>
    /// トップノード
    /// </summary>
    public class RootState : StateBase,IRootState,IReferenceState, IStateBase,IDisposable
    {
        /// <summary>
        /// オブザーバーの削除コレクション
        /// </summary>
        protected readonly CompositeDisposable disposables = new CompositeDisposable();
        /// <summary>
        /// プロパティの状態オブジェクトリスト
        /// </summary>
        protected IList<IPropertyState> properties = new List<IPropertyState>();
        /// <summary>
        /// コマンドのスタック
        /// </summary>
        public override ICommandStack CommandStack { get; }
        /// <summary>
        /// ルートオブジェクト
        /// </summary>
        public object Value { get; }
        /// <summary>
        /// 親の状態インターフェイス
        /// </summary>
        public IStateBase Parent { get; } = default!;
        /// <summary>
        /// プロパティタイプのプロパティ
        /// </summary>
        public IEnumerable<IPropertyState> Properties => properties;

        public RootState(object value, ICommandStack commandStack)
        {
            CommandStack = commandStack;
            Value = value;
            ((List<IPropertyState>)properties).AddRange(this.GetPropertyStates(value));
            AddEventListener();
        }
        /// <summary>
        /// トップ状態オブジェクトのプロパティチェンジイベントのリスナー登録
        /// </summary>
        protected void AddEventListener()
        {
            if(Value is not INotifyPropertyChanged && Value is not INotifyCollectionChanged)
            {
                return;
            }

            disposables.Add(
                    Value switch
                    {
                        INotifyCollectionChanged collectionNotify => this.CreateINotifyCollectionChangedSubscriber(Value),
                        INotifyPropertyChanged propertyNotify => this.CreateINotifyPropertyChangedSubscriber(Value),
                        _ => throw new NotImplementedException()
                    }
                );
        }

        public void Dispose()
        {
            disposables.Clear();
            foreach(var item in properties)
            {
                ((IDisposable)item).Dispose();
            }
            properties.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
