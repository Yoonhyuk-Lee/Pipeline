using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace yhrhee.Practice.Pipeline.Filter
{
    public interface IFilter
    {
        CancellationToken CancelToken   { get; }
        
        Type SourceType                 { get; }
        object Source                   { get; }
        Type DestinationType            { get; }
        object Destination              { get; }

        void Start();
    }
}
