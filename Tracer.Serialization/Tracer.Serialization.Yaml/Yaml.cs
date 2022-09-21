using System.IO;
using Tracer.Core;
using Tracer.Serialization.Abstractions;
using SharpYaml.Serialization;

namespace Tracer.Serialization.Yaml
{
    public class Yaml:ITraceResultSerializer
    {
        public string Format { get; } = "yaml";
        public void Serialize(TraceResult traceResult,Stream to)
        {
            var yamlSerializer = new Serializer();
            yamlSerializer.Serialize(to, traceResult);
        }
    }
}