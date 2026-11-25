using CQRS.Core.Commands;

namespace CQRS.Core.Infrastracture
{
    public class CommandDispatcher:ICommandDispatcher
    {
        private readonly Dictionary<Type, Func<BaseCommand, Task>> _handlers = new();
        
        public CommandDispatcher()
        {
            _handlers = new Dictionary<Type, Func<BaseCommand, Task>>();
        }
        
        public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
        {
            if (_handlers.ContainsKey(typeof(T)))
            {
                throw new InvalidOperationException($"Handler for command {typeof(T).Name} already registered.");
            }
            _handlers[typeof(T)] = async cmd => await handler((T)cmd);
        }

        public async Task SendAsync(BaseCommand command)
        {
            if (!_handlers.TryGetValue(command.GetType(), out Func<BaseCommand, Task>? handler))
            {
                throw new InvalidOperationException($"No command handler registered for {command.GetType().Name}.");
            }

            await handler!(command);
        }
    }
}