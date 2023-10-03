using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace yhrhee.Practice.Test
{
    public static class PracticeModule
    {
        public static Func<TInput, TOutput> DelayExecute<TInput, TOutput>(Func<TInput, TOutput> body, TimeSpan delay)
        {
            return src => 
            { 
                Thread.Sleep(delay); 
                return body(src); 
            };
        }

        public static Func<TOutput> DelayExecute<TOutput>(Func<TOutput> body, TimeSpan delay)
        {
            return () =>
            {
                Thread.Sleep(delay);
                return body();
            };
        }
    }
}
