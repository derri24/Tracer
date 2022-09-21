using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Tracer.Example
{
    public class FirstClass
    {
        public FirstClass(Core.Tracer tracer)
        {
            this.tracer = tracer;
        }

        Core.Tracer tracer;

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
        public SecondClass(Core.Tracer tracer)
        {
            this.tracer = tracer;
        }

        Core.Tracer tracer;

        public void M5()
        {
            tracer.StartTrace();
            Thread thread = new Thread(M6);
            thread.Start();
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

    internal class Program
    {
        public static void Main(string[] args)
        {
            Core.Tracer tracer = new Core.Tracer();
            FirstClass firstClass = new FirstClass(tracer);
            SecondClass secondClass = new SecondClass(tracer);

            firstClass.M1();
            firstClass.M2();

            secondClass.M5();

            tracer.SaveInformation(tracer.GetTraceResult(), ".\\resultsSerialization\\file");
        }
    }
}