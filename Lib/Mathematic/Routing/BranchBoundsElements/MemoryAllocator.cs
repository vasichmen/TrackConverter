using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace TrackConverter.Lib.Mathematic.Routing.BranchBoundsElements
{
    internal class MemoryAllocator<T>: IDisposable
    {
        private readonly int Cols;
        private readonly int Rows;
        private ConcurrentQueue<T[,]> stack = new ConcurrentQueue<T[,]>();
        private Task checker;
        private bool stop;


        public MemoryAllocator(int Rows, int Cols)
        {
            this.Cols = Cols;
            this.Rows = Rows;
            checker = new Task(new Action(checkerAction));
            checker.Start();
            stop = false;
            add(100000);
        }

        private void checkerAction()
        {
            while (!stop)
            {
                if (stack.Count < 10000)
                    add(1000);
                Thread.Sleep(10);
            }
        }

        public T[,] Get()
        {
            bool f = stack.TryDequeue(out T[,] res);
            while (!f)
            {
                Thread.Sleep(9);
                f = stack.TryDequeue(out res);
            }
            return res;
        }

        public void Stop()
        {
            stop = true;
            checker.Wait();
        }

        private void add(int count)
        {
            for (int i = 0; i < count; i++)
                stack.Enqueue(new T[Rows, Cols]);
        }

        internal void Dispose()
        {
            // this.stack.Clear();
        }

        void IDisposable.Dispose()
        {
            if (checker != null)
                checker.Dispose();
        }
    }
}
