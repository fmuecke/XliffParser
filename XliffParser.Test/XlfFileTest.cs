namespace XliffParser.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class XlfFileTest
    {
        [TestMethod]
        public void XlfSampleHasValidData()
        {
            using (var sample = new ResxWithStaleCorrespondingXlf())
            {
                var doc = new XlfDocument(sample.XlfFileName);
                var file = doc.Files.First();
                Assert.IsTrue(file.TransUnits.Count() > 0);
            }
        }

        [TestMethod]
        public void ExporterGetsTransUnits()
        {
            using (var sample = new ResxWithStaleCorrespondingXlf())
            {
                var doc = new XlfDocument(sample.XlfFileName);
                var file = doc.Files.First();

                var exporter = new TestXlfExporter();
                file.Export(null, exporter, null, null, XlfDialect.Standard);
                Assert.IsTrue(exporter.Units.Count > 0);
                Assert.AreEqual(exporter.Units.Count, file.TransUnits.Count());
            }
        }

        [TestMethod]
        public void ExporterGetsFileName()
        {
            using (var sample = new ResxWithStaleCorrespondingXlf())
            {
                var doc = new XlfDocument(sample.XlfFileName);
                var file = doc.Files.First();

                var exporter = new TestXlfExporter();
                var fileName = Guid.NewGuid().ToString();
                file.Export(fileName, exporter, null, null, XlfDialect.Standard);
                Assert.AreEqual(exporter.File, fileName);
            }
        }

        [TestMethod]
        public void ExporterGetsOnlyFilteredItems()
        {
            using (var sample = new ResxWithStaleCorrespondingXlf())
            {
                var doc = new XlfDocument(sample.XlfFileName);
                var file = doc.Files.First();

                var exporter = new TestXlfExporter();
                file.Export(null, exporter, new List<string>() { "this-state-does-not-exist" }, null, XlfDialect.Standard);
                Assert.AreEqual(exporter.Units.Count, 0);

                file.Export(null, exporter, new List<string>() { "this-state-does-not-exist", "final" }, null, XlfDialect.Standard);
                Assert.IsTrue(exporter.Units.Count > 0);
            }
        }

        [TestMethod]
        public void GetTransUnit_Throws_InvalidOperation_if_id_is_wrong()
        {
            using (var sample = new ResxWithStaleCorrespondingXlf())
            {
                var doc = new XlfDocument(sample.XlfFileName);
                var file = doc.Files.First();
                Assert.ThrowsException<InvalidOperationException>(() => file.GetTransUnit(null, doc.Dialect));
                Assert.ThrowsException<InvalidOperationException>(() => file.GetTransUnit(string.Empty, doc.Dialect));
                Assert.ThrowsException<InvalidOperationException>(() => file.GetTransUnit(Guid.NewGuid().ToString(), doc.Dialect));
            }
        }

        [TestMethod]
        public void TryGetTransUnit_DoesNotThrow_if_id_is_wrong()
        {
            using (var sample = new ResxWithStaleCorrespondingXlf())
            {
                var doc = new XlfDocument(sample.XlfFileName);
                var file = doc.Files.First();
                Assert.IsFalse(file.TryGetTransUnit(null, doc.Dialect, out XlfTransUnit u));
                Assert.IsFalse(file.TryGetTransUnit(string.Empty, doc.Dialect, out XlfTransUnit u2));
                Assert.IsFalse(file.TryGetTransUnit(Guid.NewGuid().ToString(), doc.Dialect, out XlfTransUnit u3));
            }
        }

        [TestMethod]
        public void ExporterGetsOnlySpecifedResTypes()
        {
            throw new NotImplementedException();
        }

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