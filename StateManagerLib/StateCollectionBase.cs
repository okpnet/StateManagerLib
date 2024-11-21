using StateManagerLib.Commands;

namespace StateManagerLib
{
    /// <summary>
    /// スタックコマンド
    /// </summary>
    public abstract class StateCollectionBase : ICommandStack ,IDisposable
    {
        protected LinkedList<IExecuteCommand> executeCommands = new();
        /// <summary>
        /// 
        /// </summary>

        protected IExecuteCommand? current;

        public virtual void Dispose()
        {
            executeCommands.Clear();
            GC.SuppressFinalize(this);
        }

        public void PushCommandOnStack(IExecuteCommand command)
        {
            if(current is not null)
            {
                executeCommands.AddFirst(current);
            }
            current = command;
        }
    }
}
