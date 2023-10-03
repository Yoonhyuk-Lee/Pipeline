using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace yhrhee.Practice.Pipeline.Filter
{
    public class Filter<SrcType, DstType> : IFilter
    {
        protected readonly BlockingCollection<SrcType> _srcQueue;
        protected readonly BlockingCollection<DstType> _dstQueue;

        protected readonly Func<SrcType, DstType> _body;

        protected readonly CancellationToken _cancelToken;

        public CancellationToken CancelToken => _cancelToken;
        public object Source => _srcQueue;
        public object Destination => _dstQueue;
        public Type SourceType => typeof(SrcType);
        public Type DestinationType => typeof(DstType);

        public Filter(object srcQ, Func<SrcType, DstType> body, CancellationToken cancelToken)
        {
            _srcQueue       = srcQ as BlockingCollection<SrcType>;
            _dstQueue       = new BlockingCollection<DstType>();
            _body           = body;
            _cancelToken    = cancelToken;
        }

        public void Start()
        {
            Task.Run(() =>
            {
                try
                {
                    while (!_srcQueue.IsAddingCompleted)
                    {
                        Execute(Take());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    _dstQueue.CompleteAdding();
                }
            }, _cancelToken);
        }

        protected SrcType Take()
        {
            return _srcQueue.Take(_cancelToken);
        }

        protected void Execute(SrcType src)
        {
            var dst = _body(src);
            Console.WriteLine("[thread {0}]::[Execute]", Thread.CurrentThread.ManagedThreadId);
            _dstQueue.Add(dst);
        }
    }
}
