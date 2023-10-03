using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace yhrhee.Practice.Pipeline.Filter
{
    public class AsyncArrayProducer<DstType> : IFilter
    {
        protected readonly BlockingCollection<DstType>  _dstQueue;
        protected readonly Func<Task<IEnumerable<DstType>>>        _body;
        protected readonly CancellationToken            _cancelToken;
        protected readonly TimeSpan                     _delay;

        public CancellationToken CancelToken => _cancelToken;
        public object Source => null;
        public object Destination => _dstQueue;
        public Type SourceType => null;
        public Type DestinationType => typeof(DstType);

        public AsyncArrayProducer(Func<Task<IEnumerable<DstType>>> body, TimeSpan delay, CancellationToken cancelToken)
        {
            _dstQueue           = new BlockingCollection<DstType>();
            _cancelToken        = cancelToken;
            _body               = body;
            _delay              = delay;
        }

        public void Start()
        {
            Task.Run(async () =>
            {                
                try
                {
                    while (!_cancelToken.IsCancellationRequested)
                    {
                        foreach(var dst in await _body())
                            _dstQueue.Add(dst);

                        await Task.Delay(_delay, _cancelToken);
                    }
                }
                finally
                {
                    _dstQueue.CompleteAdding();
                }
            }, _cancelToken);
        }
    }
}
