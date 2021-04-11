using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DepartmentTraversal
{
    public class DepartmentArrangementBuilder
    {
        private List<Department> depts;

        public int DeptCount { get; private set; }
        public int StampCount { get; private set; }
        public int StartDept { get; private set; }
        public int FinishDept { get; private set; }
        public IList<Department> Depts => depts.AsReadOnly();

        public DepartmentArrangementBuilder(int deptCount, int stampCount)
        {
            if (deptCount < 2 || stampCount < 2)
            {
                throw new ArgumentException("Both deptCount and stampCount should be greater than 1");
            }

            DeptCount = deptCount;
            StampCount = stampCount;
            depts = new List<Department>();
            for (int i = 0; i < DeptCount; ++i)
            {
                depts.Add(null);
            }
        }

        private bool IsValidDeptId(int deptId)
        {
            return deptId >= 1 && deptId <= DeptCount;
        }

        private bool IsValidStampId(int stampId)
        {
            return stampId >= 1 && stampId <= StampCount;
        }

        private string BitArrayToString(BitArray ba)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < ba.Length; ++i)
            {
                result.Append(ba[i] ? "1" : "0");
            }

            return result.ToString();
        }

        public DepartmentArrangementBuilder SetStartFinish(int startDept, int finishDept)
        {
            if (!IsValidDeptId(startDept) || !IsValidDeptId(finishDept))
            {
                throw new ArgumentException("Both department IDs should be within the [1; deptCount] range");
            }

            this.StartDept = startDept;
            this.FinishDept = finishDept;

            return this;
        }

        public DepartmentArrangementBuilder SetSimpleDepartment(int deptId, DepartmentWorker worker)
        {
            if (!IsValidDeptId(deptId) || worker == null || !worker.IsValid(DeptCount, StampCount))
            {
                throw new ArgumentException("Department|stamp IDs should be between 1 and deptCount|stampCount");
            }

            this.depts[deptId - 1] = new SimpleDepartment(deptId, worker);

            return this;
        }

        public DepartmentArrangementBuilder SetConditionalDepartment(int deptId, int conditionStamp,
            DepartmentWorker workerIfTrue, DepartmentWorker workerIfFalse)
        {
            if (!IsValidDeptId(deptId) || !IsValidStampId(conditionStamp) ||
                workerIfTrue == null || workerIfFalse == null ||
                !workerIfTrue.IsValid(DeptCount, StampCount) || !workerIfFalse.IsValid(DeptCount, StampCount))
            {
                throw new ArgumentException("Department|stamp IDs should be between 1 and deptCount|stampCount");
            }

            this.depts[deptId - 1] = new ConditionalDepartment(deptId, conditionStamp, workerIfTrue, workerIfFalse);

            return this;
        }

        public DepartmentArrangement Build()
        {
            if (!IsValidDeptId(StartDept) || !IsValidDeptId(FinishDept))
            {
                throw new Exception("Start and finish departments should be initialized before building");
            }
            for (int i = 0; i < DeptCount; ++i)
            {
                if (depts[i] == null)
                {
                    throw new Exception("All departments should be initialized before building");
                }
            }

            VisitsDAO dao = new VisitsDAO(DeptCount);

            List<DepartmentVisitStatus> visitStatuses = new List<DepartmentVisitStatus>();
            for (int i = 0; i < DeptCount; ++i)
            {
                visitStatuses.Add(DepartmentVisitStatus.Unvisited);
            }

            // precalc query results

            int nextDept = StartDept;
            List<(int, int)> visitRecord = new List<(int, int)>(); // (department id, unique stamp sheet state id)
            BitArray stamps = new BitArray(StampCount);
            while (nextDept != FinishDept)
            {
                List<string> uniqueVisits = dao.GetDeptRecord(nextDept);
                
                if (uniqueVisits == null)
                {
                    throw new Exception("Filesystem access failed");
                }

                string stampRecord = BitArrayToString(stamps);

                bool infiniteLoop = false;
                for (int i = 0; i < uniqueVisits.Count; ++i)
                {
                    if (stampRecord.Equals(uniqueVisits[i]))
                    {
                        infiniteLoop = true;

                        int loopStart = visitRecord.IndexOf((nextDept, i));
                        for (int j = loopStart; j < visitRecord.Count; ++j)
                        {
                            visitStatuses[visitRecord[j].Item1 - 1] = DepartmentVisitStatus.InfiniteLoop;
                        }

                        break;
                    }
                }
                if (infiniteLoop)
                {
                    break;
                }

                visitStatuses[nextDept - 1] = DepartmentVisitStatus.Visited;
                visitRecord.Add((nextDept, uniqueVisits.Count));
                dao.AddDeptVisit(nextDept, stampRecord);

                nextDept = depts[nextDept - 1].SetStampsAndGetNext(stamps);
            }

            if (nextDept == FinishDept)
            {
                visitStatuses[nextDept - 1] = DepartmentVisitStatus.Visited;
                dao.AddDeptVisit(nextDept, BitArrayToString(stamps));
            }
            
            // prepare files for queries

            for (int i = 1; i <= DeptCount; ++i)
            {
                List<string> uniqueVisits = dao.GetDeptRecord(i);

                if (uniqueVisits == null)
                {
                    throw new Exception("Filesystem access failed");
                }

                List<string> uniqueProcessedVisits = new List<string>();

                foreach (string stampRecord in uniqueVisits)
                {
                    BitArray recordedStamps = new BitArray(stampRecord.Length);

                    for (int j = 0; j < recordedStamps.Length; ++j)
                    {
                        recordedStamps[j] = (stampRecord[j] == '1');
                    }

                    depts[i - 1].SetStampsAndGetNext(recordedStamps);

                    uniqueProcessedVisits.Add(BitArrayToString(recordedStamps));
                }

                uniqueProcessedVisits = uniqueProcessedVisits.Distinct().ToList();

                dao.FinalizeDeptRecord(i, visitStatuses[i - 1], uniqueProcessedVisits);
            }

            return new DepartmentArrangement(DeptCount, dao);
        }

    }
}
