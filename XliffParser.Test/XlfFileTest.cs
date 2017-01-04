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
                file.AddOrUpdateTransUnit(unit.Id, unit.Source, "even newer translation", XlfDialect.Standard);
                Assert.AreEqual(initialCount + 1, file.TransUnits.Count(), "after adding the same item again, count should still be (initial+1)");
            }
        }

        [TestMethod]
        public void AddTransUnitOnlyUpdatesExistingTranslationElements()
        {
            using (var sample = new ResxWithNeutralLangXlf())
            {
                // FIXME: use simpler empty document or even file object for this test!
                var doc = new XlfDocument(sample.XlfFileName);
                var file = doc.Files.First();
                var unit1 = file.AddOrUpdateTransUnit(System.Guid.NewGuid().ToString(), "New source text", null, XlfDialect.Standard);
                Assert.IsNull(unit1.Target, "target=null does not create a target element");
                var unit1b = file.AddOrUpdateTransUnit(unit1.Id, unit1.Source, "even newer translation", XlfDialect.Standard);
                Assert.IsNull(unit1b.Target, "update does not create new target elements if the unit already exists");

                var unit2 = file.AddOrUpdateTransUnit(System.Guid.NewGuid().ToString(), "Another source text", "some translation", XlfDialect.Standard);
                Assert.AreEqual(unit2.Target, "some translation", "non empty target strings will create a new target element with the same string");
                var unit2b = file.AddOrUpdateTransUnit(unit2.Id, unit2.Source, "new translation", XlfDialect.Standard);
                Assert.AreEqual(unit2b.Target, "new translation", "existing target elemts should receive new values");
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
