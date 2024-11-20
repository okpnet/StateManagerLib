using StateManagerLib.Commands;

namespace StateManagerLib
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICommandStack
    {
        /// <summary>
        /// コマンドの登録
        /// </summary>
        /// <param name="command"></param>
        void PushCommandOnStack(IExecuteCommand command);
    }
}
