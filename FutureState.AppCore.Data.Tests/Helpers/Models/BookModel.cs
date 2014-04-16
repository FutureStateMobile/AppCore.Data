using System;

namespace FutureState.AppCore.Data.Tests.Helpers.Models
{
    public class BookModel : ModelBase
    {
        public Guid StudentId { get; set; }
        public string Name { get; set; }
        public DateTime PublishDate { get; set; }
    }
}