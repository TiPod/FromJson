using System;

namespace FromJson.Tests.Models
{
    public class TestModel
    {
        public string Text { get; set; }
        public DateTime? CreateAt { get; set; }
        public int Status { get; set; }
        public SecondModel Detail { get; set; }
    }

    public class SecondModel
    {
        public string Name { get; set; }
    }
}