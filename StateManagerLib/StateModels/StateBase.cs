using StateManagerLib.Commands;

namespace StateManagerLib.StateModels
{
    /// <summary>
    /// 状態抽象化クラス
    /// </summary>
    public abstract class StateBase:IStateBase
    {
        protected bool isUnsoRedo = false;
        /// <summary>
        /// 変更回数
        /// </summary>
        protected int changeCount = 0;
        /// <summary>
        /// 実行したコマンドの順序リスト
        /// </summary>
        protected LinkedList<IExecuteCommand> statefulCommands = [];
        /// <summary>
        /// 変更回数
        /// </summary>
        public int ChangeCount =>changeCount;
        /// <summary>
        /// 状態に対して行った変更
        /// </summary>
        public IEnumerable<IExecuteCommand> Commands=>statefulCommands;
        /// <summary>
        /// コマンドのスタック
        /// </summary>
        public abstract ICommandStack CommandStack { get; }
        /// <summary>
        /// 変更
        /// </summary>
        /// <param name="command"></param>
        public void Change(IExecuteCommand command)
        {
            if(isUnsoRedo)return;
            changeCount++;
            statefulCommands.AddFirst(command);
            CommandStack.PushCommandOnStack(command);
        }

        public void ExecuteTo(IExecuteCommand command)
        {
            if(!ReferenceEquals( command.Owner,this))
            {
                throw new Exception();
            }
            isUnsoRedo=true;
            command.Execute();
            isUnsoRedo = false;
        }
        /// <summary>
        /// 元に戻して編集されたとき
        /// </summary>
        /// <param name="command"></param>
        public void CommandRemoveForAfterUndo(IExecuteCommand? command)
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
    }
}
