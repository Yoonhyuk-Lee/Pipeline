using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace yhrhee.Practice.Pipeline.Filter
{
    public class Consumer<SrcType> : IFilter
    {
        protected readonly BlockingCollection<SrcType>  _srcQueue;
        protected readonly Action<SrcType>              _body;
        protected readonly CancellationToken            _cancelToken;

        public CancellationToken CancelToken => _cancelToken;
        public object Source => _srcQueue;
        public object Destination => null;
        public Type SourceType => typeof(SrcType);
        public Type DestinationType => null;

        public Consumer(object srcQ, Action<SrcType> body, CancellationToken cancelToken)
        {
            _srcQueue       = srcQ as BlockingCollection<SrcType>;
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

                }
            }, _cancelToken);
        }

        protected SrcType Take()
        {
            return _srcQueue.Take(_cancelToken);
        }

        protected void Execute(SrcType src)
        {
            _body(src);
        }
    }
}
