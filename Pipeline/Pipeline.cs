using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using yhrhee.Practice.Pipeline.Filter;

namespace yhrhee.Practice.Pipeline
{
    public class MyPipeline
    {
        protected readonly List<IFilter>    _filters;

        protected MyPipeline()
        {
            _filters = new List<IFilter>();
        }

        public static MyPipeline CreatePipeline<TOutput>(Func<Task<TOutput>> source, CancellationToken cancelToken)
        {
            var pipeline = new MyPipeline();
            AsyncProducer<TOutput> producer = new AsyncProducer<TOutput>(source, cancelToken);
            pipeline._filters.Add(producer);

            return pipeline;
        }

        public static MyPipeline CreatePipeline<TOutput>(Func<TOutput> source, CancellationToken cancelToken)
        {
            var pipeline = new MyPipeline();
            Producer<TOutput> producer = new Producer<TOutput>(source, cancelToken);
            pipeline._filters.Add(producer);

            return pipeline;
        }

        public MyPipeline AddFilter<TInput, TOutput>(Func<TInput, TOutput> func)
        {
            var lastfilter = _filters.Last();

            var dstType = lastfilter.DestinationType;
            if(!dstType.Equals(typeof(TInput)))
                throw new InvalidCastException("invalid filter");

            Filter<TInput, TOutput> filterToAdd = new Filter<TInput, TOutput>(lastfilter.Destination, func, lastfilter.CancelToken);
            _filters.Add(filterToAdd);

            return this;
        }

        public MyPipeline SetConsumer<TInput>(Action<TInput> func)
        {
            var lastfilter = _filters.Last();

            var dstType = lastfilter.DestinationType;
            if (!typeof(TInput).Equals(dstType))
                throw new InvalidCastException("invalid filter");

            Consumer<TInput> consumer = new Consumer<TInput>(lastfilter.Destination, func, lastfilter.CancelToken);
            _filters.Add(consumer);

            return this;
        }

        public void Start()
        {
            foreach (var filter in _filters)
                filter.Start();
        }
    }
}
