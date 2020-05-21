using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchoolDisplay;
using SchoolDisplayTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolDisplay.Tests
{
    [TestClass()]
    public class CyclicPdfServiceTests
    {
        [TestMethod()]
        public void SinglePdfTest()
        {
            NPdfTest(1);
        }

        [TestMethod()]
        public void TwoPdfTest()
        {
            NPdfTest(2);
        }

        [TestMethod()]
        public void ThreePdfTest()
        {
            NPdfTest(3);
        }

        [TestMethod()]
        public void FourPdfTest()
        {
            NPdfTest(4);
        }

        [TestMethod()]
        public void FivePdfTest()
        {
            NPdfTest(5);
        }

        [TestMethod()]
        public void HundredPdfTest()
        {
            NPdfTest(100);
        }

        [TestMethod()]
        public void ThousandPdfTest()
        {
            NPdfTest(973);
        }

        private void NPdfTest(int n)
        {
            var repo = new FakePdfRepository(GetDummyFileList(n));
            var service = new CyclicPdfService(repo);

            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    var document = (FakePdfDocument)service.GetNextDocument();
                    Assert.AreEqual(GetDummyFileName(j), document.FileName);
                }
            }
        }


        /* helper methods */
        private List<string> GetDummyFileList(int fileCount)
        {
            List<string> fileList = new List<string>(fileCount);

            for (int i = 0; i < fileCount; i++)
            {
                fileList.Add(GetDummyFileName(i));
            }

            return fileList;
        }

        private string GetDummyFileName(int id)
        {
            // the file names have to alphabetically ordered
            return string.Format("{0}.pdf", id.ToString("D8"));
        }
    }
}