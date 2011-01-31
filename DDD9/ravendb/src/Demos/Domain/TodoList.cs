using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demos.Domain
{
    public class TodoList
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public DateTime Deadline { get; set; }
        public List<TodoItem> Items { get; set; }
    }
}
