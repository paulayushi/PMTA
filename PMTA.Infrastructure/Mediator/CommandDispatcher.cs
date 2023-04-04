using PMTA.Core.Command;

namespace PMTA.Infrastructure.Mediator
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly Dictionary<Type, Func<BaseCommand, Task>> _handlers = new();
        public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
        {
            if (_handlers.ContainsKey(typeof(T)))
            {
                throw new ArgumentOutOfRangeException("You cannot register the same handle twice.");
            }

            _handlers.Add(typeof(T), x => handler((T)x));
        }

        public async Task SendAsync(BaseCommand command)
        {
            if(_handlers.TryGetValue(command.GetType(), out Func<BaseCommand, Task> handler))
            {
                await handler(command);
            }
            else
            {
                throw new ArgumentException(nameof(handler), "No command handler was registered.");
            }
        }        
    }
}
