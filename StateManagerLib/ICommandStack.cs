using StateManagerLib.Commands;
using StateManagerLib.StateModels;

namespace StateManagerLib
{
    /// <summary>
    /// スタックを登録するインターフェイス
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
