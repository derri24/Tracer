namespace Tracer.Core
{
    public class AllMethodInfo
    {
        public AllMethodInfo(long threadId, string methodName, string parentMethodName, string className,
            long executionTime)
        {
            ThreadId = threadId;
            MethodName = methodName;
            ParentMethodName = parentMethodName;
            ClassName = className;
            ExecutionTime = executionTime;
        }

        public long ThreadId { get; set; }
        public string MethodName { get; set; }
        public string ParentMethodName { get; set; }
        public string ClassName { get; set; }
        public long ExecutionTime { get; set; }
    }
}