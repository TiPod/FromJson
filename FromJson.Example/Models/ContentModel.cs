using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FromJson.Example.Models
{
    public class ContentModel
    {
        public string Text { get; set; }
        public string Author { get; set; }
        public DateTime? CreateAt { get; set; }
    }

    public class FullModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ContentModel Content { get; set; }
    }
}
