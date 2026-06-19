using System;

namespace AdvancedDocumentManager.Models
{
    public class DocumentModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Category { get; set; }

        public DocumentModel()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
        }

        public DocumentModel(string title, string content, string category)
        {
            Id = Guid.NewGuid();
            Title = title;
            Content = content;
            Category = category;
            CreatedDate = DateTime.Now;
        }
    }
}