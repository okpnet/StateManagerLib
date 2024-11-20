using StateManagerLib.Collections;
using StateManagerLib.Extensions;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Disposables;

namespace StateManagerLib.StateModels
{
    /// <summary>
    /// トップノード
    /// </summary>
    public class RootState : IRootState,IReferenceState, IStateBase,IDisposable
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
        public ICommandStack CommandStack { get; }
        /// <summary>
        /// ルートオブジェクト
        /// </summary>
        public object Value { get; }
        /// <summary>
        /// 状態に対して行った変更
        /// </summary>
        public ICommandCollection Commands { get; }
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
            Value = value;
            CommandStack = commandStack;
            ((List<IPropertyState>)properties).AddRange(this.GetPropertyStates(Value));
            AddEventListener();
        }
        /// <summary>
        /// トップ状態オブジェクトのプロパティチェンジイベントのリスナー登録
        /// </summary>
        protected void AddEventListener()
        {
            if(Value is not INotifyPropertyChanged || Value is not INotifyCollectionChanged)
            {
                return;
            }

            disposables.Add(
                    Value switch
                    {
                        INotifyCollectionChanged collectionNotify => this.CreateINotifyCollectionChangedSubscriber(Value),
                        INotifyPropertyChanged propertyNotify => Properties.CreateINotifyPropertyChangedSubscriber(Value),
                        _ => throw new NotImplementedException()
                    }
                );
        }



        public void Dispose()
        {
            disposables.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
