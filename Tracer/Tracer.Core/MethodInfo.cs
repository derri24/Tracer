using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tracer.Core
{
    [DataContract]
    public class MethodInfo
    {
        [DataMember] public string MethodName { get; set; }
        [DataMember] public string ClassName { get; set; }
        [DataMember] public long ExecutionTime { get; set; }
        [DataMember] public List<MethodInfo> ChildMethods;

        public MethodInfo()
        {
        }

        public MethodInfo(string methodName, string className, long executionTime)
        {
            ChildMethods = new List<MethodInfo>();
            MethodName = methodName;
            ClassName = className;
            ExecutionTime = executionTime;
        }
    }
}