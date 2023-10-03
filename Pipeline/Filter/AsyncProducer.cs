using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace yhrhee.Practice.Pipeline.Filter
{
    public class AsyncProducer<DstType> : IFilter
    {
        protected readonly BlockingCollection<DstType>  _dstQueue;
        protected readonly Func<Task<DstType>>          _body;
        protected readonly CancellationToken            _cancelToken;

        public CancellationToken CancelToken => _cancelToken;
        public object Source => null;
        public object Destination => _dstQueue;
        public Type SourceType => null;
        public Type DestinationType => typeof(DstType);

        public AsyncProducer(Func<Task<DstType>> body, CancellationToken cancelToken)
        {
            _dstQueue           = new BlockingCollection<DstType>();
            _cancelToken        = cancelToken;
            _body               = body;
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                try
                {
                    while (!_cancelToken.IsCancellationRequested)
                    {
                        var dst = await _body();
                        _dstQueue.Add(dst);
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
