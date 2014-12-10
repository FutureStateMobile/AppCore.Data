using System;
using FutureState.AppCore.Data.Attributes;

namespace FutureState.AppCore.Data.Tests.Helpers.Models
{
    public class BookModel : ModelBase
    {
        public string Name { get; set; }
        public DateTime PublishDate { get; set; }
        public int BookNumber { get; set; }
        
        [ManyToOne]
        public AuthorModel Author { get; set; }
    }
}