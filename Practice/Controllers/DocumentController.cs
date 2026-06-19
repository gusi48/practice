using System;
using System.Collections.Generic;
using AdvancedDocumentManager.Models;

namespace Practice.Controllers
{
    public class DocumentController
    {
        private readonly List<DocumentModel> _documents;

        public DocumentController()
        {
            _documents = new List<DocumentModel>();
        }

        public List<DocumentModel> GetAllDocuments()
        {
            return _documents;
        }

        public bool AddDocument(string title, string content, string category)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return false; 
            }

            var newDoc = new DocumentModel(title, content, category);
            _documents.Add(newDoc);
            return true;
        }

        public bool DeleteDocument(Guid id)
        {
            foreach (var doc in _documents)
            {
                if (doc.Id == id)
                {
                    _documents.Remove(doc);
                    return true;
                }
            }
            return false;
        }
    }
}