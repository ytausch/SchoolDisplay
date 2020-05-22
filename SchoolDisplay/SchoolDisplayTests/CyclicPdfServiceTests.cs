using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchoolDisplayTests;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace SchoolDisplay.Tests
{
    [TestClass()]
    public class CyclicPdfServiceTests
    {
        [TestMethod]
        public void SinglePdfTest()
        {
            NPdfTest(1);
        }

        [TestMethod]
        public void TwoPdfTest()
        {
            NPdfTest(2);
        }

        [TestMethod]
        public void ThreePdfTest()
        {
            NPdfTest(3);
        }

        [TestMethod]
        public void FourPdfTest()
        {
            NPdfTest(4);
        }

        [TestMethod]
        public void FivePdfTest()
        {
            NPdfTest(5);
        }

        [TestMethod()]
        public void HundredPdfTest()
        {
            NPdfTest(100);
        }

        [TestMethod]
        public void ThousandPdfTest()
        {
            NPdfTest(973);
        }

        [TestMethod]
        public void AddInBetweenTest()
        {
            var filelist = new List<string> { "a.pdf", "c.pdf" };
            var repo = new FakePdfRepository(filelist);
            var service = new CyclicPdfService(repo);

            var document = (FakePdfDocument)service.GetNextDocument();
            Assert.AreEqual("a.pdf", document.FileName);

            // add in between
            repo.FileList.Add("b.pdf");

            document = (FakePdfDocument)service.GetNextDocument();
            Assert.AreEqual("b.pdf", document.FileName);

            document = (FakePdfDocument)service.GetNextDocument();
            Assert.AreEqual("c.pdf", document.FileName);
        }

        [TestMethod]
        public void RemoveInBetweenTest()
        {
            var repo = new FakePdfRepository(GetDummyFileList(3));
            var service = new CyclicPdfService(repo);

            var document = (FakePdfDocument)service.GetNextDocument();
            Assert.AreEqual(GetDummyFileName(0), document.FileName);

            // remove in between
            repo.FileList.Remove(GetDummyFileName(1));

            document = (FakePdfDocument)service.GetNextDocument();
            Assert.AreEqual(GetDummyFileName(2), document.FileName);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void NoFilesTest()
        {
            var repo = new FakePdfRepository(new List<string>());
            var service = new CyclicPdfService(repo);

            service.GetNextDocument();
        }

        [TestMethod]
        public void CurrentFileChangedTest()
        {
            var repo = new FakePdfRepository(GetDummyFileList(3));
            var service = new CyclicPdfService(repo);

            var eventTriggered = 0;

            service.OnInvalidate += delegate
            {
                eventTriggered++;
            };

            service.GetNextDocument();  // fake pdf #0
            repo.RaiseDataChangedEvent(new ChangedPdfEventArgs(GetDummyFileName(0)));
            repo.RaiseDataChangedEvent(new ChangedPdfEventArgs(GetDummyFileName(0)));   // triggering twice should not change anything

            Assert.AreEqual(0, eventTriggered);     // CyclicPdfService should delay the event by about 2 seconds

            Thread.Sleep(500);
            repo.RaiseDataChangedEvent(new ChangedPdfEventArgs(GetDummyFileName(0)));   // triggering again should not change anything
            Thread.Sleep(2100);

            Assert.AreEqual(1, eventTriggered);      // the event should now be triggered

            repo.RaiseDataChangedEvent(new ChangedPdfEventArgs(GetDummyFileName(0)));
            Thread.Sleep(2100);
            Assert.AreEqual(1, eventTriggered);     // triggering again should not change anything

            var doc = (FakePdfDocument)service.GetNextDocument();               // this should re-enable the event
            Assert.AreEqual(GetDummyFileName(0), doc.FileName);                // because our current file has changed, GetNextDocument() should return it again

            service.GetNextDocument();      // current document: fake PDF #1

            repo.RaiseDataChangedEvent(new ChangedPdfEventArgs(GetDummyFileName(1)));   // change the new current document
            Assert.AreEqual(1, eventTriggered);     // delay is still active...
            Thread.Sleep(2100);
            Assert.AreEqual(2, eventTriggered);     // now it should be triggered twice
        }

        [TestMethod]
        public void OtherFileChangedTest()
        {
            var repo = new FakePdfRepository(GetDummyFileList(3));
            var service = new CyclicPdfService(repo);

            var eventTriggered = false;

            service.OnInvalidate += delegate
            {
                eventTriggered = true;
            };

            service.GetNextDocument();      // dummy file #0
            service.GetNextDocument();      // dummy file #1

            // we now change dummy file #0 and ensure the event is not triggered
            repo.RaiseDataChangedEvent(new ChangedPdfEventArgs(GetDummyFileName(0)));
            Thread.Sleep(2100);
            Assert.IsFalse(eventTriggered);
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