using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace Tracer.Core.Tests
{
    public class FirstClass
    {
        public FirstClass(Tracer tracer)
        {
            this.tracer = tracer;
        }

        Tracer tracer;

        public void M1()
        {
            tracer.StartTrace();
            Thread.Sleep(15);
            tracer.StopTrace();
        }

        public void M2()
        {
            tracer.StartTrace();
            M3();
            M4();
            Thread.Sleep(30);
            tracer.StopTrace();
        }

        public void M3()
        {
            tracer.StartTrace();
            Thread.Sleep(150);
            tracer.StopTrace();
        }

        public void M4()
        {
            tracer.StartTrace();
            Thread.Sleep(70);
            tracer.StopTrace();
        }
    }

    public class SecondClass
    {
        public SecondClass(Tracer tracer)
        {
            this.tracer = tracer;
        }

        Tracer tracer;

        public void M5()
        {
            tracer.StartTrace();
            Thread firstThread = new Thread(M6);
            firstThread.Start();
            Thread.Sleep(55);
            tracer.StopTrace();
        }

        public void M6()
        {
            tracer.StartTrace();
            Thread.Sleep(15);
            tracer.StopTrace();
        }
    }

    [TestFixture]
    public class Tests
    {
        [Test]
        public void Test1()
        {
            Tracer tracer = new Tracer();
            FirstClass firstClass = new FirstClass(tracer);
            firstClass.M1();
            firstClass.M2();

            var thread = tracer.GetTraceResult().Threads[0];

            Assert.AreEqual(2, thread.Methods.Count);
            Assert.True(
                thread.Methods.FirstOrDefault(name => name.MethodName == "M2") != null);
            Assert.True(thread.TotalTime < 300 &&
                        thread.TotalTime > 200);
            Assert.True(thread.Methods.FirstOrDefault(name => name.MethodName == "M1")
                .ExecutionTime < 60);
        }

        [Test]
        public void Test2()
        {
            Tracer tracer = new Tracer();
            FirstClass firstClass = new FirstClass(tracer);
            firstClass.M1();
            firstClass.M2();

            var thread = tracer.GetTraceResult().Threads[0];

            Assert.AreEqual("FirstClass", thread.Methods.FirstOrDefault(name => name.MethodName == "M1").ClassName
            );
            Assert.True(thread.Methods.FirstOrDefault(name => name.MethodName == "M1")
                .ExecutionTime < 60);
            Assert.True(thread.Methods.FirstOrDefault(name => name.MethodName == "M2")
                .ChildMethods.Count == 2);
            Assert.True(thread.Methods.FirstOrDefault(name => name.MethodName == "M2")
                .ExecutionTime < 300);
        }

        [Test]
        public void Test3()
        {
            Tracer tracer = new Tracer();
            SecondClass secondClass = new SecondClass(tracer);
            secondClass.M5();

            var traceResult = tracer.GetTraceResult();

            Assert.AreEqual(2, tracer.GetTraceResult().Threads.Count);
            Assert.AreEqual("SecondClass",
                traceResult.Threads
                    .FirstOrDefault(x => x.Id == Thread.CurrentThread.ManagedThreadId).Methods
                    .FirstOrDefault(name => name.MethodName == "M5").ClassName
            );
            Assert.True(traceResult.Threads.FirstOrDefault(x => x.Id != Thread.CurrentThread.ManagedThreadId).Methods
                .FirstOrDefault(name => name.MethodName == "M6")
                .ExecutionTime < 35);

            Assert.True(
                traceResult.Threads.FirstOrDefault(x => x.Id == Thread.CurrentThread.ManagedThreadId).TotalTime < 85 &&
                traceResult.Threads.FirstOrDefault(x => x.Id == Thread.CurrentThread.ManagedThreadId).TotalTime > 50);
        }

        [Test]
        public void Test4()
        {
            Tracer tracer = new Tracer();
            SecondClass secondClass = new SecondClass(tracer);
            secondClass.M5();

            var traceResult = tracer.GetTraceResult();

            Assert.AreEqual(0,
                traceResult.Threads
                    .FirstOrDefault(x => x.Id == Thread.CurrentThread.ManagedThreadId).Methods
                    .FirstOrDefault(name => name.MethodName == "M5").ChildMethods.Count);
            Assert.True(
                traceResult.Threads
                    .FirstOrDefault(x => x.Id == Thread.CurrentThread.ManagedThreadId).Methods
                    .FirstOrDefault(name => name.MethodName == "M5").ExecutionTime
                < 80);
            Assert.AreEqual("SecondClass",
                traceResult.Threads
                    .FirstOrDefault(x => x.Id != Thread.CurrentThread.ManagedThreadId).Methods
                    .FirstOrDefault(name => name.MethodName == "M6").ClassName
            );
            Assert.AreEqual(2, tracer.GetTraceResult().Threads.Count);
        }
    }
}