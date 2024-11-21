using StateManagerLib.StateModels;

namespace StateManagerLib.Commands
{
    public abstract class Command:IExecuteCommand
    {
        /// <summary>
        /// オーナー
        /// </summary>
        public IStateBase Owner { get; } = default!;
        /// <summary>
        /// セットされたインスタンス
        /// </summary>
        public object? Value { get; }
        /// <summary>
        /// 戻す
        /// </summary>
        /// <returns></returns>
        public abstract bool Execute();

        protected Command(IStateBase owner,object? value)
        {
            Owner = owner;
            Value = value;
        }
    }
}
