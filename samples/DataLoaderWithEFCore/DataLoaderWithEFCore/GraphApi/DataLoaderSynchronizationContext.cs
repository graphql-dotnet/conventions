//using System.Threading;
//using GraphQL.DataLoader;

//namespace DataLoaderWithEFCore.GraphApi
//{
//    /// <summary>
//    /// A custom synchronization context to work with the async/await infrastructure.
//    /// </summary>
//    internal class DataLoaderSynchronizationContext : SynchronizationContext
//    {
//        /// <summary>
//        /// Creates a new <see cref="DataLoaderSynchronizationContext"/>.
//        /// </summary>
//        internal DataLoaderSynchronizationContext(DataLoaderContext context)
//        {
//            UnderlyingContext = context;
//        }

//        /// <summary>
//        /// Gets the underlying <see cref="DataLoaderContext"/>.
//        /// </summary>
//        public DataLoaderContext UnderlyingContext { get; }

//        /// <summary>
//        /// Synchronously invokes the given callback before preparing to fire any new loaders.
//        /// </summary>
//        /// <remarks>
//        /// <para>This method is called by the async/await infrastructure.</para>
//        /// <para>Usually this method would run asynchronously, however we call it synchronously instead
//        /// because we want to let all the continuation code finish before firing additional loaders.</para>
//        /// <para>The callback will execute with this as the current synchronization context.</para>
//        /// </remarks>
//        public override void Post(SendOrPostCallback d, object state)
//        {
//            var prevCtx = SynchronizationContext.Current;
//            var wasCurrentContext = prevCtx == this;
//            if (!wasCurrentContext) SynchronizationContext.SetSynchronizationContext(this);
//            try
//            {
//                d(state);
//                UnderlyingContext.CommitLoadersOnThread();
//            }
//            finally
//            {
//                if (!wasCurrentContext) SynchronizationContext.SetSynchronizationContext(prevCtx);
//            }
//        }

//        /// <see cref="Post"/>
//        public override void Send(SendOrPostCallback d, object state) => Post(d, state);
//    }
//}
