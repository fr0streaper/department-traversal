using System.Collections;

namespace DepartmentTraversal
{
    public class SimpleDepartment : Department
    {
        public DepartmentWorker Worker { get; private set; }

        internal SimpleDepartment(int id, DepartmentWorker worker)
        {
            Id = id;
            Worker = worker;
        }

        internal override int SetStampsAndGetNext(BitArray stamps)
        {
            return Worker.SetStampsAndGetNext(stamps);
        }
    }
}
