using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tracer.Core
{
    [DataContract]
    public class ThreadInfo
    {
        [DataMember] public long Id { get; set; }
        [DataMember] public long TotalTime { get; set; }
        [DataMember] public List<MethodInfo> Methods { get; set; }

        public ThreadInfo()
        {
            Methods = new List<MethodInfo>();
        }
    }
}