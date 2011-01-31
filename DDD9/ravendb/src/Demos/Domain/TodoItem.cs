using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demos.Domain
{
    public class TodoItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public bool IsComplete { get; set; }
    }
}
