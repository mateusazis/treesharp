using System;
using TreeSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TreeSharpTests
{
    [TestClass]
    public class RepeaterTest
    {
        IterComp c5, c10;
        TreeSharp.Action fixedAction;
        Repeater r1, r2, fixedRepeater;

        const int FIXED_REPEATER_COUNT = 4;

        private static RunStatus DoNothing(object ctx) { return RunStatus.Success; }

        [TestInitialize]
        public void Init()
        {
             c5 = new IterComp(5, RunStatus.Success);
             c10 = new IterComp(10, RunStatus.Success);

             r1 = new Repeater(c5);
             r2 = new Repeater(c10);

             fixedAction = new TreeSharp.Action((TreeSharp.ActionDelegate)DoNothing);
             fixedRepeater = new Repeater(fixedAction, FIXED_REPEATER_COUNT);
        }

        [TestMethod]
        public void AlwaysRunningTest()
        {
            r1.Start(null);
            for (int i = 0; i < 6; i++)
                Assert.AreEqual(RunStatus.Running, r1.Tick(null));
        }

        [TestMethod]
        public void RepeaterFunctionallity()
        {
            r2.Start(null);
            int lastI = 10;
            for (int i = 0; i < 30; i++)
            {
                Assert.AreEqual(RunStatus.Running, r2.Tick(null));
                Assert.IsTrue(c10.i == lastI - 1 || ((c10.i == 0 || c10.i == 9) && lastI == 0));
                lastI = c10.i;
            }
        }

        [TestMethod]
        public void SimpleActionTest()
        {
            int x = 0;
            const int count = 23;
            TreeSharp.Action action = new TreeSharp.Action(
                delegate(object context)
            {
                x++;
            });
            Repeater actionRepeater = new Repeater(action);
            actionRepeater.Start(null);
            for (int i = 0; i < count; i++)
                actionRepeater.Tick(null);
            
            Assert.AreEqual<int>(count, x);
        }

        [TestMethod]
        public void RepeaterFixedIterationsTest()
        {
            fixedRepeater.Start(null);
            for (int i = 0; i < FIXED_REPEATER_COUNT; i++)
                fixedRepeater.Tick(null);
            Assert.AreEqual(RunStatus.Success, fixedRepeater.LastStatus);
        }

    }
}
