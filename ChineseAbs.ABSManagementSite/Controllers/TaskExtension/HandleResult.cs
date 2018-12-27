namespace ChineseAbs.ABSManagementSite.Controllers.TaskExtension
{
    public enum EventResult
    {
        Continue,
        Cancel
    }

    public class HandleResult
    {
        public HandleResult()
        {
            EventResult = TaskExtension.EventResult.Continue;
            Message = string.Empty;
        }

        public HandleResult(EventResult eventResult, string message)
        {
            EventResult = eventResult;
            Message = message;
        }

        public string Message { get; set; }

        public EventResult EventResult { get; set; }
    }
}