using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tracer.Core
{
    [DataContract]
    public class TraceResult
    {
        public IReadOnlyList<ThreadInfo> Threads
        {
            get => _threads;
        }

        [DataMember(Name = "Threads")] private List<ThreadInfo> _threads;

        public TraceResult()
        {
            _threads = new List<ThreadInfo>();
        }

        public void AddThreadToList(ThreadInfo threadInfo)
        {
            _threads.Add(threadInfo);
        }
    }
}