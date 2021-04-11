using System.Collections;

namespace DepartmentTraversal
{
    // this is a bit questionable when it comes to extensibility
    // (e.g. if we wanted to have depts that add two stamps and don't remove any),
    // but it prevents some pretty bad spaghetti code in this task, so yeah :D
    public class DepartmentWorker
    {
        public int AddedStamp { get; private set; }
        public int RemovedStamp { get; private set; }
        public int NextDept { get; private set; }

        public DepartmentWorker(int addedStamp, int removedStamp, int nextDept)
        {
            AddedStamp = addedStamp; 
            RemovedStamp = removedStamp; 
            NextDept = nextDept; 
        }

        internal int SetStampsAndGetNext(BitArray stamps)
        {
            stamps.Set(AddedStamp - 1, true);
            stamps.Set(RemovedStamp - 1, false);

            return NextDept;
        }

        internal bool IsValid(int deptCount, int stampCount)
        {
            return AddedStamp >= 1 && AddedStamp <= stampCount &&
                RemovedStamp >= 1 && RemovedStamp <= stampCount &&
                NextDept >= 1 && NextDept <= deptCount;
        }
    }
}
