using CQRS.Core.Messages;

namespace Post.Cmd.Api.Events
{

    public abstract class BaseEvent: Message
    {
        protected BaseEvent(string type)
        {
            this.Type = type;
        }
        public string? Type { get; set; }
        public int Version { get; set; }
    }
}