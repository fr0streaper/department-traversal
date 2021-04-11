using System.Collections;

namespace DepartmentTraversal
{
    public enum DepartmentVisitStatus
    {
        Unvisited = 0,
        Visited = 1,
        InfiniteLoop = 2
    }

    public abstract class Department
    {
        public int Id { get; protected set; }

        internal abstract int SetStampsAndGetNext(BitArray stamps);

    }
}
