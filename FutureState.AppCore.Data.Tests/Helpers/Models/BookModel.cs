using System;
using System.Collections.Generic;
using System.Linq;
using FutureState.AppCore.Data.Attributes;

namespace FutureState.AppCore.Data.Tests.Helpers.Models
{
    public class BookModel : ModelBase
    {
        private List<AuthorModel> _authors;

        public BookModel()
        {
            _authors = new List<AuthorModel>();
        }

        public string Name { get; set; }
        public int ISBN { get; set; }
        public DateTime PublishDate { get; set; }

        [ManyToOne]
        public PublisherModel Publisher { get; set; }

        [ManyToMany]
        public IEnumerable<AuthorModel> Authors
        {
            get { return _authors; }
            private set { _authors = (List<AuthorModel>)value; }
        }

        public void AddAuthors(params AuthorModel[] books)
        {
            _authors.AddRange(books);
        }

        public void RemoveAuthors(params AuthorModel[] authors)
        {
            _authors.RemoveAll(authors.Contains);
        }

    }
}