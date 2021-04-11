using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DepartmentTraversal
{
    internal class VisitsDAO
    {
        private const int ID_LENGTH = 8;

        private string daoId;

        internal VisitsDAO(int deptCount)
        {
            daoId = GenerateDAOId();
            Directory.CreateDirectory(GetDaoDir());
        }

        ~VisitsDAO()
        {
            DirectoryInfo DaoDirectory = new DirectoryInfo(GetDaoDir());

            DaoDirectory.Delete(true);
        }

        private string GenerateDAOId()
        {
            // a more reasonble implementation would probaby involve
            // a hash of time or something, possible TODO

            StringBuilder idBuilder = new StringBuilder();
            string charset = "qwertyuiopasdfghjklzxcvbnm";
            Random random = new Random();

            for (int i = 0; i < ID_LENGTH; ++i)
            {
                idBuilder.Append(charset[random.Next(charset.Length)]);
            }

            return idBuilder.ToString();
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

        private string GetDaoDir()
        {
            return Directory.GetCurrentDirectory() + "\\tmp\\" + daoId;
        }

        private string GetDeptFilename(int deptId)
        {
            return string.Format(GetDaoDir() + "\\{0}.dat", deptId);
        }

        internal List<string> GetDeptRecord(int deptId)
        {
            List<string> deptRecord = new List<string>();

            try
            {
                using (FileStream fs = File.Open(GetDeptFilename(deptId), FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader sr = new StreamReader(GetDeptFilename(deptId)))
                    {
                        string line;

                        while ((line = sr.ReadLine()) != null)
                        {
                            deptRecord.Add(line);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return null;
            }

            return deptRecord;
        }

        internal void AddDeptVisit(int deptId, string stampRecord)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(GetDeptFilename(deptId), true))
                {
                    sw.WriteLine(stampRecord);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be written:");
                Console.WriteLine(e.Message);
            }
        }

        internal void FinalizeDeptRecord(int deptId, DepartmentVisitStatus visitStatus, List<string> visitRecords)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(GetDeptFilename(deptId), false))
                {
                    sw.WriteLine(visitStatus.ToString());
                    foreach (string line in visitRecords)
                    {
                        sw.WriteLine(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be written:");
                Console.WriteLine(e.Message);
            }
        }

    }
}
