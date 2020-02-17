using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Primes_Parallel
{
    public class HugeArray<T> : IEnumerable<T>
    where T : struct
    {
        public static int arysize = (Int32.MaxValue >> 4) / Marshal.SizeOf<T>();

        public readonly ulong Capacity;
        private readonly T[][] content;

        public T this[ulong index]
        {
            get
            {
                if (index < 0 || index >= Capacity)
                    throw new IndexOutOfRangeException();
                int chunk = (int)(index / (ulong)arysize);
                int offset = (int)(index % (ulong)arysize);
                return content[chunk][offset];
            }
            set
            {
                if (index < 0 || index >= Capacity)
                    throw new IndexOutOfRangeException();
                int chunk = (int)(index / (ulong)arysize);
                int offset = (int)(index % (ulong)arysize);
                content[chunk][offset] = value;
            }
        }

        public HugeArray(ulong capacity)
        {
            Capacity = capacity;
            int nChunks = (int)(capacity / (ulong)arysize);
            int nRemainder = (int)(capacity % (ulong)arysize);

            if (nRemainder == 0)
                content = new T[nChunks][];
            else
                content = new T[nChunks + 1][];

            for (int i = 0; i < nChunks; i++)
                content[i] = new T[arysize];
            if (nRemainder > 0)
                content[content.Length - 1] = new T[nRemainder];
        }

        public IEnumerator<T> GetEnumerator()
        {
            return content.SelectMany(c => c).GetEnumerator();
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}
