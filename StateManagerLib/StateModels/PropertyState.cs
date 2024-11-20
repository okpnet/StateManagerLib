using LinqExtenssions;
using StateManagerLib.Collections;
using StateManagerLib.Commands;
using StateManagerLib.Extensions;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace StateManagerLib.StateModels
{
    public class PropertyState:IPropertyState,IReferenceState,IStateBase,IDisposable
    {

        protected LinkedList<IExecuteCommand> statefulCommands;

        /// <summary>
        /// オブザーバーの削除コレクション
        /// </summary>
        protected readonly CompositeDisposable disposables = new CompositeDisposable();
        /// <summary>
        /// 
        /// </summary>
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
        /// トップ状態オブジェクト
        /// </summary>
        public IRootState Root => (IRootState)GetRootState(this);
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
        /// <summary>
        /// 変更イベントリスナー登録
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void AddNotifyChangedListener(object value)
        {
            properties.ClaerList();
            if (!this.GetPropertyStates(value).IfDeclare(out var resultList))
            {
                ((List<IPropertyState>)properties).AddRange(resultList);
            }

            disposables.Add(
                    value switch
                    {
                        INotifyCollectionChanged collectionNotify => this.CreateINotifyCollectionChangedSubscriber(value),
                        INotifyPropertyChanged propertyNotify => Properties.CreateINotifyPropertyChangedSubscriber(value),
                        _ => throw new NotImplementedException()
                    }
                );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        public void AddCommandToState(IExecuteCommand command)
        {
            statefulCommands.AddFirst(command);
        }

        public void Stateful(IExecuteCommand? command)
        {
            if (statefulCommands.First is null)
            {
                return;
            }
            while (statefulCommands.First.Value != command)
            {
                statefulCommands.RemoveFirst();
            }
        }
        /// <summary>
        /// トップを取得
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        /// <exception cref="OverflowException"></exception>
        protected IStateBase GetRootState(IStateBase state)
        {
            if(state is IRootState rootState)
            {
                return rootState;
            }
            if (state is IPropertyState propertyState && propertyState.Parent is not null)
            {
                return GetRootState(propertyState.Parent);
            }
            throw new OverflowException("Argment is not IPropertyState or IRootState.");
        }

        public void Dispose()
        {
            disposables.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
