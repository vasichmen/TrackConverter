using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TrackConverter.Lib.Mathematic.Routing.BranchBoundsElements
{
    class MemoryAllocator<T>:IDisposable
    {
        int Cols;
        int Rows;

        ConcurrentQueue<T[,]> stack= new ConcurrentQueue<T[,]>();
        Task checker;
        bool stop;


        public MemoryAllocator(int Rows, int Cols)
        {
            this.Cols = Cols;
            this.Rows = Rows;
            checker = new Task(new Action(checkerAction));
            checker.Start();
            stop = false;
            Add(100000);
        }

        private  void checkerAction()
        {
            while (!stop)
            {
                if (stack.Count < 10000)
                    Add(1000);
                Thread.Sleep(10);  
            }
        }

        public T[,] Get()
        {
            T[,] res = null;
            bool f = stack.TryDequeue(out res);
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

        private  void Add(int count)
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
