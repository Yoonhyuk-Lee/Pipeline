using System;
using System.Threading;
using System.Threading.Tasks;
using yhrhee.Practice.Test;
using yhrhee.Practice.Pipeline;

namespace yhrhee.Practice
{
    class Program
    {
        static void Main(string[] args)
        {
            const int TEST_SIZE = 10;           

            Task.Run(() => 
            {
                var cancelTokenSource = new CancellationTokenSource();
                try
                {
                    int srcCount = TEST_SIZE;
                    int dstCount = 0;

                    var pipeline = MyPipeline.CreatePipeline(PracticeModule.DelayExecute<int>(() => srcCount--, TimeSpan.FromMilliseconds(100)), cancelTokenSource.Token)
                            .AddFilter(PracticeModule.DelayExecute<int, string>(i => i.ToString("D4"), TimeSpan.FromMilliseconds(500)))
                            .AddFilter(PracticeModule.DelayExecute<string, string>(str => string.Format("[Header]::{0}", str), TimeSpan.FromMilliseconds(200)))
                            .AddFilter(PracticeModule.DelayExecute<string, string>(str => string.Format("{0}::[Footer]", str), TimeSpan.FromMilliseconds(300)))
                            .SetConsumer<string>(str => { Console.WriteLine(string.Format("[{0}]::[{1}]", DateTime.Now.ToString("HH:mm:ss.fff"), str)); ++dstCount; });

                    Console.WriteLine("Start pipeline...");
                    pipeline.Start();

                    while (dstCount < TEST_SIZE)
                        Thread.Sleep(1);
                }
                finally
                {
                    Console.WriteLine("Stop pipeline...");
                    cancelTokenSource.Cancel();
                    cancelTokenSource.Dispose();
                    cancelTokenSource = null;
                }                
            });

            var key = Console.ReadKey();
        }
    }
}
