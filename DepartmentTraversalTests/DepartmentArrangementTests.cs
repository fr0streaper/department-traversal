using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DepartmentTraversal.Tests
{
    [TestClass()]
    public class DepartmentArrangementTests
    {
        private string ConvertRecords(int stampCount, List<List<int>> records)
        {
            StringBuilder result = new StringBuilder();

            foreach (List<int> record in records) {
                int recordIndex = 0;

                for (int i = 0; i < stampCount; ++i)
                {
                    if (recordIndex < record.Count && record[recordIndex] == i + 1)
                    {
                        result.Append("1");
                        ++recordIndex;
                    }
                    else
                    {
                        result.Append("0");
                    }
                }

                result.Append(",");
            }
            if (result.Length > 0)
            {
                result.Remove(result.Length - 1, 1);
            }

            return result.ToString();
        }

        // 1 ---[ +1 -2 ]---> 2
        // 2 ---[ +2 -1 ]---> 1
        [TestMethod()]
        public void BasicArrangementTest()
        {
            DepartmentArrangementBuilder builder = new DepartmentArrangementBuilder(2, 2);

            builder.SetStartFinish(1, 2);

            builder.SetSimpleDepartment(1, new DepartmentWorker(1, 2, 2));
            builder.SetSimpleDepartment(2, new DepartmentWorker(2, 1, 1));

            DepartmentArrangement arrangement = builder.Build();

            var (status, records) = arrangement.Query(1);

            Assert.AreEqual(DepartmentVisitStatus.Visited, status);
            Assert.IsTrue(ConvertRecords(2, records).Equals("10"));
        }

        // 1 ---[ +1 -2 ]---> 2
        // 2 ---[ ?1 (+2 -1) | (+1 -2)]---> 3 | 4
        // 3 ---[ +1 -2 ]---> 1
        // 4 ---[ +2 -1 ]---> 1
        [TestMethod()]
        public void ConditionalArrangementTest()
        {
            DepartmentArrangementBuilder builder = new DepartmentArrangementBuilder(4, 2);

            builder.SetStartFinish(1, 3);

            builder.SetSimpleDepartment(1, new DepartmentWorker(1, 2, 2));
            builder.SetConditionalDepartment(2, 1, new DepartmentWorker(2, 1, 3), new DepartmentWorker(1, 2, 4));
            builder.SetSimpleDepartment(3, new DepartmentWorker(1, 2, 1));
            builder.SetSimpleDepartment(4, new DepartmentWorker(2, 1, 1));

            DepartmentArrangement arrangement = builder.Build();

            var (status, records) = arrangement.Query(4);

            Assert.AreEqual(DepartmentVisitStatus.Unvisited, status);
            Assert.IsTrue(ConvertRecords(2, records).Equals(""));

            (status, records) = arrangement.Query(3);

            Assert.AreEqual(DepartmentVisitStatus.Visited, status);

            (status, records) = arrangement.Query(2);

            Assert.IsTrue(ConvertRecords(2, records).Equals("01"));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidBuilderConstructorArgumentsTest()
        {
            DepartmentArrangementBuilder builder = new DepartmentArrangementBuilder(1, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidBuilderUsageStartFinishTest()
        {
            DepartmentArrangementBuilder builder = new DepartmentArrangementBuilder(2, 2);

            builder.SetStartFinish(-1, 1000);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidBuilderUsageSimpleDeptTest()
        {
            DepartmentArrangementBuilder builder = new DepartmentArrangementBuilder(2, 2);

            builder.SetSimpleDepartment(4, null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidBuilderUsageConditionalDeptTest()
        {
            DepartmentArrangementBuilder builder = new DepartmentArrangementBuilder(2, 2);

            builder.SetConditionalDepartment(1, 2, new DepartmentWorker(10, 10, 10), new DepartmentWorker(10, 10, 10));
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void IncompleteBuildAttemptTest()
        {
            DepartmentArrangementBuilder builder = new DepartmentArrangementBuilder(2, 2);

            builder.Build();
        }

        // 1 ---[ +1 -2 ]---> 2
        // 2 ---[ +2 -1 ]---> 1
        // 3 ---[ +1 -2 ]---> 4
        // 4 ---[ +2 -1 ]---> 3
        [TestMethod()]
        public void InfiniteLoopTest()
        {
            DepartmentArrangementBuilder builder = new DepartmentArrangementBuilder(4, 2);

            builder.SetStartFinish(1, 3);

            builder.SetSimpleDepartment(1, new DepartmentWorker(1, 2, 2));
            builder.SetSimpleDepartment(2, new DepartmentWorker(2, 1, 1));
            builder.SetSimpleDepartment(3, new DepartmentWorker(1, 2, 4));
            builder.SetSimpleDepartment(4, new DepartmentWorker(2, 1, 3));

            DepartmentArrangement arrangement = builder.Build();

            var (status, records) = arrangement.Query(1);

            Assert.AreEqual(DepartmentVisitStatus.InfiniteLoop, status);
            Assert.IsTrue(ConvertRecords(2, records).Equals("10"));

            (status, records) = arrangement.Query(2);

            Assert.AreEqual(DepartmentVisitStatus.InfiniteLoop, status);
            Assert.IsTrue(ConvertRecords(2, records).Equals("01"));

            (status, records) = arrangement.Query(3);

            Assert.AreEqual(DepartmentVisitStatus.Unvisited, status);
            Assert.IsTrue(ConvertRecords(2, records).Equals(""));
        }

        // 1 ---[ +1 -2 ]---> 2
        // 2 ---[ +3 -2 ]---> 3
        // 3 ---[ +2 -1 ]---> 1
        // 4 ---[ +1 -2 ]---> 5
        // 5 ---[ +1 -2 ]---> 4
        [TestMethod()]
        public void MultipleRecordsTest()
        {
            DepartmentArrangementBuilder builder = new DepartmentArrangementBuilder(5, 3);

            builder.SetStartFinish(1, 4);

            builder.SetSimpleDepartment(1, new DepartmentWorker(1, 2, 2));
            builder.SetSimpleDepartment(2, new DepartmentWorker(3, 2, 3));
            builder.SetSimpleDepartment(3, new DepartmentWorker(2, 1, 1));
            builder.SetSimpleDepartment(4, new DepartmentWorker(1, 2, 5));
            builder.SetSimpleDepartment(5, new DepartmentWorker(1, 2, 4));

            DepartmentArrangement arrangement = builder.Build();

            var (status, records) = arrangement.Query(1);

            Assert.AreEqual(DepartmentVisitStatus.InfiniteLoop, status);
            Assert.IsTrue(ConvertRecords(3, records).Equals("100,101"));
        }
    }
}