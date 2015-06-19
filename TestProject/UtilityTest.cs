using DNACircSynchronizer.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestProject
{
    
    
    /// <summary>
    ///This is a test class for UtilityTest and is intended
    ///to contain all UtilityTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UtilityTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for FormatTimetoCJTimeString
        ///</summary>
        [TestMethod()]
        public void FormatTimetoCJTimeStringTest()
        {
            DateTime inDateTime = new DateTime(2009, 1, 5, 8, 10, 0); 
            string expected = "0810"; // TODO: Initialize to an appropriate value
            string actual;
            actual = Utility.FormatTimetoCJTimeString(inDateTime);
            Assert.IsTrue(expected == actual, string.Format("Time should have been {0} but instead was {1}", expected, actual));


            inDateTime = new DateTime(2009, 1, 5, 14, 01, 0); 
            expected = "1401"; // TODO: Initialize to an appropriate value
            actual = Utility.FormatTimetoCJTimeString(inDateTime);
            Assert.IsTrue(expected == actual, string.Format("Time should have been {0} but instead was {1}", expected, actual));

        }
    }
}
