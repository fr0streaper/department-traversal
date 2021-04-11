using System.Collections;

namespace DepartmentTraversal
{
    public class ConditionalDepartment : Department
    {
        public int ConditionStamp { get; private set; }
        public DepartmentWorker WorkerIfTrue { get; private set; }
        public DepartmentWorker WorkerIfFalse { get; private set; }

        internal ConditionalDepartment(int id, int conditionStamp,
            DepartmentWorker workerIfTrue, DepartmentWorker workerIfFalse)
        {
            Id = id;
            ConditionStamp = conditionStamp;
            WorkerIfTrue = workerIfTrue;
            WorkerIfFalse = workerIfFalse;
        }

        internal override int SetStampsAndGetNext(BitArray stamps)
        {
            if (stamps[ConditionStamp - 1])
            {
                return WorkerIfTrue.SetStampsAndGetNext(stamps);
            }
            else
            {
                return WorkerIfFalse.SetStampsAndGetNext(stamps);
            }
        }
    }
}
