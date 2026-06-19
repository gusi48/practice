using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdvancedDocumentManager.Models;

namespace Tests
{
    [TestClass]
    public class DocumentModelTests
    {
        [TestMethod]
        public void Constructor_ShouldInitializeFieldsCorrectly()
        {
            string expectedTitle = "Тестовый документ";
            string expectedContent = "Текст тестового документа";
            string expectedCategory = "Учеба";

            var doc = new DocumentModel(expectedTitle, expectedContent, expectedCategory);

            Assert.AreEqual(expectedTitle, doc.Title);
            Assert.AreEqual(expectedContent, doc.Content);
            Assert.AreEqual(expectedCategory, doc.Category);
            Assert.AreNotEqual(Guid.Empty, doc.Id);
        }

        [TestMethod]
        public void DocumentId_ShouldBeUniqueForEveryInstance()
        {
            var doc1 = new DocumentModel("Док 1", "Контент 1", "Категория 1");
            var doc2 = new DocumentModel("Док 2", "Контент 2", "Категория 2");

            Assert.AreNotEqual(doc1.Id, doc2.Id);
        }
    }
}