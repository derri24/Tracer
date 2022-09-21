using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Tracer.Core;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization.Xml
{
    public class Xml:ITraceResultSerializer
    {
        public string Format { get; } = "xml";
        
        public void Serialize(TraceResult traceResult, Stream to)
        {
            var serializer = new DataContractSerializer(typeof(TraceResult));
            using (XmlWriter xw = XmlWriter.Create(to,new XmlWriterSettings(){Indent = true}))
            {
                serializer.WriteObject(xw, traceResult);
            }
        }
    }
    
}