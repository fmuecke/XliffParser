namespace XliffParser.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class XlfFileTest
    {
        [TestMethod]
        public void AddTransUnitTest()
        {
            using (var sample = new ResxWithStaleCorrespondingXlf())
            {
                var doc = new XlfDocument(sample.XlfFileName);
                var file = doc.Files.First();
                var initialCount = file.TransUnits.Count();
                var unit = file.AddOrUpdateTransUnit(System.Guid.NewGuid().ToString(), "New source text", "new translation", XlfDialect.Standard);

                Assert.AreEqual(initialCount + 1, file.TransUnits.Count(), "after adding one item, count should be (initial+1)");

                try
                {
                    file.AddTransUnit(unit.Id, string.Empty, string.Empty, XlfFile.AddMode.FailIfExists, XlfDialect.Standard);
                    Assert.Fail("Adding the same unit must result in exception");
                }
                catch (System.InvalidOperationException)
                {
                }

                var sameUnit = file.AddTransUnit(unit.Id, string.Empty, string.Empty, XlfFile.AddMode.SkipExisting, XlfDialect.Standard);
                Assert.AreEqual(unit.ToString(), sameUnit.ToString(), "unit must not be modified but returned");
            }
        }

        [TestMethod]
        public void UpdateTransUnitTest()
        {
            using (var sample = new ResxWithStaleCorrespondingXlf())
            {
                var doc = new XlfDocument(sample.XlfFileName);
                var file = doc.Files.First();
                var initialCount = file.TransUnits.Count();
                var unit = file.AddOrUpdateTransUnit(System.Guid.NewGuid().ToString(), "New source text", "new translation", XlfDialect.Standard);
                file.AddOrUpdateTransUnit(unit.Id, unit.Source, "evene newer translation", XlfDialect.Standard);
                Assert.AreEqual(initialCount + 1, file.TransUnits.Count(), "after adding the same item again, count should still be (initial+1)");
            }
        }

        [TestMethod]
        public void RemoveTransUnitTest()
        {
            using (var sample = new ResxWithStaleCorrespondingXlf())
            {
                var doc = new XlfDocument(sample.XlfFileName);
                var file = doc.Files.First();
                var initialCount = file.TransUnits.Count();

                // first be sure unit is there
                file.GetTransUnit("c", XlfDialect.Standard);

                file.RemoveTransUnit("c", XlfDialect.Standard);
                Assert.AreEqual(initialCount - 1, file.TransUnits.Count(), "after removing an item the count should one less than before");
            }
        }
    }
}
