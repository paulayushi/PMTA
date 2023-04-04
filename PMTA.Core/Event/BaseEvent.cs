namespace PMTA.Core.Event
{
    public class BaseEvent : Message.Message
    {
        protected BaseEvent(string type)
        {
            Type = type;
        }

        public int Version { get; set; }
        public string Type { get; set; }
    }
}
