using System.IO;
using Newtonsoft.Json;
using Tracer.Core;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization.Json
{
    public class Json : ITraceResultSerializer
    {
        public string Format { get; } = "json";
        public void Serialize(TraceResult traceResult, Stream to)
        {
            StreamWriter sw = new StreamWriter(to);
            sw.Write(JsonConvert.SerializeObject(traceResult, Formatting.Indented));
            sw.Close();
        }
    }
}