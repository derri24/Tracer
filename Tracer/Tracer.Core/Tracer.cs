using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Tracer.Serialization.Abstractions;

namespace Tracer.Core
{
    public class Tracer : ITracer
    {
        private List<AllMethodInfo> listOfAllMethodInfo;
        private List<Stopwatch> stopWatches;
        private Stopwatch stopWatch;

        public Tracer()
        {
            listOfAllMethodInfo = new List<AllMethodInfo>();
            stopWatches = new List<Stopwatch>();
        }

        public void StartTrace()
        {
            stopWatch = new Stopwatch();
            stopWatches.Add(stopWatch);
            stopWatches[stopWatches.Count - 1].Start();
        }

        public void StopTrace()
        {
            stopWatches[stopWatches.Count - 1].Stop();
            var stacktrace = new StackTrace();
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var methodName = stacktrace.GetFrame(1).GetMethod().Name;
            var parentName = stacktrace.GetFrame(2).GetMethod().Name;

            var className = stacktrace.GetFrame(1).GetMethod().ReflectedType.Name;

            var executionTime = stopWatches[stopWatches.Count - 1].ElapsedMilliseconds;
            AllMethodInfo allMethodInfo = new AllMethodInfo(threadId, methodName, parentName, className, executionTime);
            listOfAllMethodInfo.Add(allMethodInfo);
            stopWatches.RemoveAt(stopWatches.Count - 1);
        }

        private MethodInfo GetParent(List<MethodInfo> methods, AllMethodInfo allMethodInfo)
        {
            var tempParent =
                methods.FirstOrDefault(childMethod => childMethod.MethodName == allMethodInfo.ParentMethodName);
            if (tempParent != null)
                return tempParent;
            for (int i = 0; i < methods.Count; i++)
            {
                var parent = GetParent(methods[i].ChildMethods, allMethodInfo);
                if (parent != null)
                    return parent;
            }

            return null;
        }

        private long GetTotalTime(List<MethodInfo> methods)
        {
            long totalTime = 0;
            for (int i = 0; i < methods.Count; i++)
                totalTime += methods[i].ExecutionTime;
            return totalTime;
        }

        private TraceResult CreateTree()
        {
            TraceResult traceResult = new TraceResult();
            for (int i = listOfAllMethodInfo.Count - 1; i >= 0; i--)
            {
                var resultTread =
                    traceResult.Threads.FirstOrDefault(tread => tread.Id == listOfAllMethodInfo[i].ThreadId);
                MethodInfo methodInfo = new MethodInfo(listOfAllMethodInfo[i].MethodName,
                    listOfAllMethodInfo[i].ClassName, listOfAllMethodInfo[i].ExecutionTime);
                if (resultTread == null)
                {
                    ThreadInfo threadInfo = new ThreadInfo();
                    threadInfo.Id = listOfAllMethodInfo[i].ThreadId;
                    threadInfo.Methods.Add(methodInfo);
                    threadInfo.TotalTime = GetTotalTime(threadInfo.Methods);
                    traceResult.AddThreadToList(threadInfo);
                }
                else
                {
                    resultTread.TotalTime = GetTotalTime(resultTread.Methods);
                    var parent = GetParent(resultTread.Methods, listOfAllMethodInfo[i]);
                    if (parent == null)
                        resultTread.Methods.Add(methodInfo);
                    else
                        parent.ChildMethods.Add(methodInfo);
                }
            }

            return traceResult;
        }

        private FileStream CreateStream(string fileName, string format)
        {
            return new FileStream($".\\{fileName}.{format}", FileMode.Create);
        }

        public void SaveInformation(TraceResult traceResult, string fileName)
        {
            string pluginPath = ".\\plugins";
            var pluginFiles = Directory.GetFiles(pluginPath, "*.dll");
            foreach (var plugin in pluginFiles)
            {
                Assembly myDll = Assembly.LoadFrom(plugin);
                var types = myDll.GetTypes().Where(t =>
                    t.GetInterfaces().Where(i => i.FullName == typeof(ITraceResultSerializer).FullName).Any());
                foreach (var type in types)
                {
                    var obj = (ITraceResultSerializer) Activator.CreateInstance(type);
                    var stream = CreateStream(fileName, obj.Format);
                    obj.Serialize(traceResult, stream);
                    stream.Close();
                }
            }
        }

        public TraceResult GetTraceResult()
        {
            return CreateTree();
        }
    }
}