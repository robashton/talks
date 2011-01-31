using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Raven.Client.Indexes;
using Demos.Domain;

namespace Demos
{
    [TestFixture]
    public class Map_Reduce : RavenTest
    {
        [Test]
        public void UsingAPreDefinedIndexMeansWeCanDoAggregations()
        {
            #region 5 of one, 3 of another, 2 of another

            using (var session = NewSession())
            {
                for (int x = 0; x < 5; x++)
                {
                    session.Store(new TodoList()
                    {
                        Items = new List<TodoItem>()
                        {
                            new TodoItem(){
                                Category = "CategoryOne"
                            }
                        }
                    });
                }
                for(int x = 0; x < 3 ; x++)
                {
                    session.Store(new TodoList()
                    {
                        Items = new List<TodoItem>()
                        {
                            new TodoItem(){
                                Category = "CategoryTwo"
                            }
                        }
                    });
                }

                for (int x = 0; x < 2; x++)
                {
                    session.Store(new TodoList()
                    {
                        Items = new List<TodoItem>()
                        {
                            new TodoItem(){
                                Category = "CategoryThree"
                            }
                        }
                    });
                }
                session.SaveChanges();
                WaitForIndexing();
            }

            #endregion

            using (var session = NewSession())
            {
                var items = session.Query<CategoryCountItem, TodoList_CountCategories>()
                    .ToArray();
                
                Assert.AreEqual(5,
                    items.Where(x => x.Category == "CategoryOne").First().Count);
                Assert.AreEqual(3,
                    items.Where(x => x.Category == "CategoryTwo").First().Count);
                Assert.AreEqual(2,
                    items.Where(x => x.Category == "CategoryThree").First().Count);

            }
        }

        public class CategoryCountItem
        {
            public string Category { get; set; }
            public int Count { get; set; }
        }

        public class TodoList_CountCategories : AbstractIndexCreationTask<TodoList, CategoryCountItem>
        {
            public TodoList_CountCategories()
            {
                Map = docs => from doc in docs
                              from tag in doc.Items
                              select new
                              {
                                  tag.Category,
                                  Count = 1
                              };
                Reduce = results => from result in results
                                    group result by result.Category into g
                                    select new
                                    {
                                        Category = g.Key,
                                        Count = g.Sum(x=>x.Count)
                                    };
            }
        }
    }
}
