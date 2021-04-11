using System;
using System.Collections.Generic;

namespace DepartmentTraversal
{
    public class DepartmentArrangement
    {
        private int deptCount;
        private VisitsDAO dao;

        internal DepartmentArrangement(int deptCount, VisitsDAO dao)
        {
            this.deptCount = deptCount;
            this.dao = dao;
        }

        public (DepartmentVisitStatus, List<List<int>>) Query(int deptId)
        {
            if (deptId < 1 || deptId > deptCount)
            {
                throw new ArgumentException("Department ID should be within the [1; deptCount] range");
            }
            
            DepartmentVisitStatus visitStatus = DepartmentVisitStatus.Unvisited;
            List<List<int>> uniqueVisits = null;

            List<string> record = dao.GetDeptRecord(deptId);

            visitStatus = (DepartmentVisitStatus)Enum.Parse(typeof(DepartmentVisitStatus), record[0]);

            uniqueVisits = new List<List<int>>();
            for (int i = 1; i < record.Count; ++i)
            {
                List<int> uniqueVisit = new List<int>();

                string line = record[i];
                for (int j = 0; j < line.Length; ++j)
                {
                    if (line[j] == '1')
                    {
                        uniqueVisit.Add(j + 1);
                    }
                }

                uniqueVisits.Add(uniqueVisit);
            }

            return (visitStatus, uniqueVisits);
        }
    }
}
