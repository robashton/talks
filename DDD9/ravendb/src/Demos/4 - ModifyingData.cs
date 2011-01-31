using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Demos.Domain;

namespace Demos
{
    [TestFixture]
    public class ModifyingData : RavenTest
    {
        [Test]
        public void IfILoadAnItemAndModifyItChangesArePersisted()
        {
            string id = string.Empty;
            #region Saving the user and keeping the id around
            using (var meh = NewSession())
            {
                var user = new User()
                {
                    DisplayName = "Rob Ashton",
                    EmailAddress = "robashton@codeofrob.com",
                    Name = "robashton"
                };

                meh.Store(user);
                meh.SaveChanges();
                id = user.Id;
            }


            #endregion

            using (var session = NewSession())
            {
                User user = session.Load<User>(id);
                user.DisplayName = "Doris";
                session.SaveChanges();
            }

            using (var session = NewSession())
            {
                User user = session.Load<User>(id);
                Assert.AreEqual("Doris", user.DisplayName);
            }
        }



        [Test]
        public void IfIModifySeveralItemsTheWholeLotGetPersistedAtomically()
        {
            string userId = string.Empty;
            String listId = string.Empty;
            #region Saving the items and keeping the ids around
            using (var meh = NewSession())
            {
                var user = new User()
                {
                    DisplayName = "Rob Ashton",
                    EmailAddress = "robashton@codeofrob.com",
                    Name = "robashton"
                };
                var list = new TodoList()
                {
                    Name = "DDD9 todo list",
                    Items = new List<TodoItem>()
                    {
                        new TodoItem(){
                             Title = "Item #1"
                        }
                    }
                };

                meh.Store(user);
                meh.Store(list);
                meh.SaveChanges();

                userId = user.Id;
                listId = list.Id;
            }


            #endregion

            using (var session = NewSession())
            {
                User user = session.Load<User>(userId);
                TodoList list = session.Load<TodoList>(listId);

                user.DisplayName = "Doris";
                list.Items.Add(new TodoItem()
                {
                    Title = "Item #2"
                });   

                // This saves ALL changes atomically 
                // (YES! A unit of work with a single request)
                session.SaveChanges();
            }

            using (var session = NewSession())
            {
                User user = session.Load<User>(userId);
                TodoList list = session.Load<TodoList>(listId);
                Assert.AreEqual("Doris", user.DisplayName);
                Assert.AreEqual(2, list.Items.Count);
            }
        }
    }
}
