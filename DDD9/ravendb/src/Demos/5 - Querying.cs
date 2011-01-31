using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Demos.Domain;
using Raven.Client.Indexes;

namespace Demos
{
    [TestFixture]
    public class Querying : RavenTest
    {
        [Test]
        public void LookAtMeICanQueryDocumentsWithoutAnIndex()
        {
            #region Saving 5 lists for one user, and 3 lists for another

            using (var session = NewSession())
            {
                for (int x = 0; x < 5; x++)
                {
                    session.Store(new TodoList()
                    {
                         UserId = "users/1"
                    });
                }
                for (int x = 0; x < 3; x++)
                {
                    session.Store(new TodoList()
                    {
                        UserId = "users/2"
                    });
                }
                session.SaveChanges();


            }
            #endregion

            using (var session = NewSession())
            {
                var listForUser1 = (from list in session.Query<TodoList>()
                                    where list.UserId == "users/1"
                                    select list)
                                    .ToArray();

                Assert.AreEqual(5, listForUser1.Length);                
            }
        }

        [Test]
        public void YesThatIncludesCollectionsToo()
        {
            #region Saving 5 lists, 3 with DDD9 items, 2 without

            using (var session = NewSession())
            {
                for (int x = 0; x < 3; x++)
                {
                    session.Store(new TodoList()
                    {
                        UserId = "users/1",
                        Items = new List<TodoItem>()
                        {
                            new TodoItem(){
                                 Category = "Raven"
                            }
                        }

                    });
                }
                for (int x = 0; x < 2; x++)
                {
                    session.Store(new TodoList()
                    {
                        UserId = "users/2",
                        Items = new List<TodoItem>()
                        {
                            new TodoItem(){
                                 Category = "DDD9"
                            }
                        }
                    });
                }
                session.SaveChanges();


            }
            #endregion

            using (var session = NewSession())
            {
                var listsContainingRavenItems = (from list in session.Query<TodoList>()
                                    where list.Items.Any(x=>x.Category == "Raven")
                                    select list)
                                    .ToArray();

                Assert.AreEqual(3, listsContainingRavenItems.Length);
            }
        }

        [Test]
        public void ButWeCanUseHomeMadeIndexesIfWeWant()
        {
            #region Saving 5 lists for one user, and 3 lists for another

            using (var session = NewSession())
            {
                for (int x = 0; x < 5; x++)
                {
                    session.Store(new TodoList()
                    {
                        UserId = "users/1"
                    });
                }
                for (int x = 0; x < 3; x++)
                {
                    session.Store(new TodoList()
                    {
                        UserId = "users/2"
                    });
                }
                session.SaveChanges();
                WaitForIndexing();

            }
            #endregion

            using (var session = NewSession())
            {
                var listForUser1 = (from list in session.Query<TodoList, TodoList_ByUser>()
                                    where list.UserId == "users/1"
                                    select list)
                                    .ToArray();

                Assert.AreEqual(5, listForUser1.Length);
            }
        }

        public class TodoList_ByUser : AbstractIndexCreationTask<TodoList>
        {
            public TodoList_ByUser()
            {
                Map = docs => from doc in docs
                              select new
                              {
                                  doc.UserId
                              };

            }
        }
    }
}
