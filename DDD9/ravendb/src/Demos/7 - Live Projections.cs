using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Raven.Client.Indexes;
using Demos.Domain;
using Raven.Client.Linq;

namespace Demos
{
    [TestFixture]
    public class Live_Projections : RavenTest
    {
        #region What I want to achieve here

        // SQL VERSION 
        /* SELECT
         *      tl.UserId,
         *      tl.Name,
         *      (SELECT COUNT(*) FROM TodoListItem ti WHERE ti.TaskId = tl.Id),
         *      u.DisplayName    
         * FROM
         *      TodoList tl
         * LEFT OUTER JOIN
         *      Users u ON tl.UserId = u.UserId
         * 
         * */

        #endregion

        [Test]
        public void AndYouDontSeeMeDenormalisingDataJustForQueryPurposes()
        {
            String id = string.Empty;
            #region Create data
            using (var session = NewSession())
            {
                var user = new User()
                {
                    DisplayName = "Rob Ashton",
                    EmailAddress = "robashton@codeofrob.com",
                    Name = "robashton"
                };
                session.Store(user);
                id = user.Id;

                var todoList = new TodoList(){
                     UserId = user.Id,
                     Items = new List<TodoItem>(){
                         new TodoItem() {},
                         new TodoItem() {},
                         new TodoItem() {}
                     },
                     Name = "Some list"
                };
                session.Store(todoList);
                session.SaveChanges();
                WaitForIndexing();
            }

            #endregion

            #region Query data
            using(var session = NewSession())
            {
                var result = session.Query<TodoList, TodoListViewIndex>()
                    .Where(x=>x.UserId == id)
                    .As<TodoListView>()
                    .First();

                Assert.AreEqual("Rob Ashton", result.UserDisplayName);
                Assert.AreEqual(3, result.TaskCount);
            }
            #endregion
        }

        public class TodoListView
        {
            public string UserId { get; set; }
            public string Name { get; set;}
            public int TaskCount { get; set;}
            public string UserDisplayName { get; set;}
        }

        public class TodoListViewIndex : AbstractIndexCreationTask<TodoList>
        {
            public TodoListViewIndex()
            {
                Map = docs => from doc in docs
                              select new
                              {
                                  UserId = doc.UserId
                              };
                
                TransformResults = (database, results) =>
                    from result in results
                    let user = database.Load<User>(result.UserId)
                    select new
                    {
                        UserId = result.UserId,
                        Name = result.Name,
                        TaskCount = result.Items.Count,
                        UserDisplayName = user.DisplayName
                    };
            }
        }
    }
}
