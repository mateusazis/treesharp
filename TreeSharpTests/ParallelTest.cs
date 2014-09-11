using System;
using TreeSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TreeSharpTests
{
    [TestClass]
    public class ParallelTest
    {
        IterComp c1, c2, c1Failure, c2Failure, cCont1, cCont2;
        Parallel p1, empty, single, pSuccessOne, pFailureOne, pFailureAll, pContinuous, pContinousStop;

        [TestInitialize]
        public void Init()
        {
             c1 = new IterComp(0, RunStatus.Success);
             c2 = new IterComp(1, RunStatus.Success);
             c1Failure = new IterComp(0, RunStatus.Failure);
             c2Failure = new IterComp(1, RunStatus.Failure);
             cCont1 = new IterComp(5, RunStatus.Success);
             cCont2 = new IterComp(10, RunStatus.Success);

             p1 = new Parallel(Policy.ALL_MET, Policy.ALL_MET, c1, c2);
             empty = new Parallel(Policy.ALL_MET, Policy.ALL_MET);
             single = new Parallel(Policy.ALL_MET, Policy.ALL_MET, c2);

             pSuccessOne = new Parallel(Policy.ALL_MET, Policy.ONE_MET, c1, c2);
             pFailureOne = new Parallel(Policy.ONE_MET, Policy.ALL_MET, c1Failure, c2Failure);
             pFailureAll = new Parallel(Policy.ALL_MET, Policy.ALL_MET, c1Failure, c2Failure);
             pContinuous = new Parallel(Policy.ALL_MET, Policy.ALL_MET, cCont1, cCont2);
             pContinousStop = new Parallel(Policy.ALL_MET, Policy.ONE_MET, cCont1);
        }

        [TestMethod]
        public void EmptyParallelTest()
        {
            empty.Start(null);
            Assert.AreEqual(RunStatus.Success, empty.Tick(null));
        }

        [TestMethod]
        public void ParallelSingleActionTest()
        {
            single.Start(null);
            Assert.AreEqual(RunStatus.Running, single.Tick(null));
            Assert.AreEqual(RunStatus.Success, single.Tick(null));
        }

        [TestMethod]
        public void SuccessAll()
        {
            p1.Start(null);
            Assert.AreEqual(RunStatus.Running, p1.Tick(null));
            Assert.AreEqual(RunStatus.Success, p1.Tick(null));
        }

        [TestMethod]
        public void SuccessOne()
        {
            pSuccessOne.Start(null);
            Assert.AreEqual(RunStatus.Success, pSuccessOne.Tick(null));
        }

        [TestMethod]
        public void FailOne()
        {
            pFailureOne.Start(null);
            Assert.AreEqual(RunStatus.Failure, pFailureOne.Tick(null));
        }

        [TestMethod]
        public void FailAll()
        {
            pFailureAll.Start(null);
            Assert.AreEqual(RunStatus.Running, pFailureAll.Tick(null));
            Assert.AreEqual(RunStatus.Failure, pFailureAll.Tick(null));
        }

        [TestMethod]
        public void ParallelFunctionallity()
        {
            pContinuous.Start(null);
            for (int i = 0; i < 3; i++)
                pContinuous.Tick(null);
            Assert.AreEqual<int>(2, cCont1.i);
            Assert.AreEqual<int>(7, cCont2.i);
            for (int i = 0; i < 6; i++)
                pContinuous.Tick(null);
            Assert.AreEqual<int>(0, cCont1.i);//c1 should not run after a certain time
            Assert.AreEqual<int>(1, cCont2.i);
        }

        //[TestMethod]
        ////[ExpectedException(typeof(System.ApplicationException))]
        //public void StopOnTheRightMoment()
        //{
        //    pContinousStop.Start(null);
        //    for (int i = 0; i < 6; i++)
        //        pContinousStop.Tick(null);
        //    Assert.AreEqual(RunStatus.Success, pContinousStop.LastStatus);

        //    //should throw exception here
        //    pContinousStop.Tick(null);
            
        //}
    }

    public class IterComp : Composite
    {
        private int n;
        public int i;
        RunStatus result;
        public IterComp(int n, RunStatus result)
        {
            this.n = n;
            this.result = result;
        }

        public override void Start(object context)
        {
            base.Start(context);
            i = n;
        }

        public override System.Collections.Generic.IEnumerable<RunStatus> Execute(object context)
        {
            while (i > 0)
            {
                i--;
                yield return RunStatus.Running;
            }
            yield return result;
            yield break;
        }
    }
}
