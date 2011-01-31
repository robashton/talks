using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Demos.Domain;

namespace Demos
{
    [TestFixture]
    public class PersistingData : RavenTest
    {
        [Test]
        public void WatchMeJustSaveThisPocoAndMarvelAtTheSimplicityOfItAll()
        {
            using (var session = NewSession())
            {
                // So we can create a user
                var user = new User()
                {
                     DisplayName = "Rob Ashton",
                     EmailAddress = "robashton@codeofrob.com",
                     Name = "robashton"
                };

                Assert.Null(user.Id);
                session.Store(user);
                Assert.NotNull(user.Id);

                // NOTE: Nothing has been sent to the server yet

                session.SaveChanges();             

                // Okay, now it has.
            }
        }

        [Test]
        public void ThatMeansSerializingEntireDocuments()
        {
            using (var session = NewSession())
            {
                var todoList = new TodoList()
                {
                    Deadline = new DateTime(2010, 11, 20),
                    Name = "Finish this presentation",
                    Items = new List<TodoItem>()
                    {
                        new TodoItem(){
                            Category = "DDD9",
                            IsComplete = false,
                            Title = "PPT",
                            Description = "Create a powerpoint",
                           
                        },                        
                        new TodoItem(){
                            Category = "DDD9",
                            IsComplete = false,
                            Title = "Awkwardly inject humour into slides",
                            Description = "Think of some way to make fun of t'south",
                                
                        },
                        new TodoItem(){
                            Category = "DDD9",
                            IsComplete = false,
                            Title = "Tests",
                            Description = "Create a load of tests for demonstration purposes",
                             
                        }
                    }
                };

                // NOTE: Serializes ENTIRE document into one giant JSON blob!!
                session.Store(todoList);
                session.SaveChanges();
            }
        }

            [Test]
            public void NoteTheCompleteAndUtterLackOfRelations(){
                User user;
                #region Creating and saving the user

                using (var meh = NewSession())
                {
                    user = new User()
                    {
                        DisplayName = "Rob Ashton",
                        EmailAddress = "robashton@codeofrob.com",
                        Name = "robashton"
                    };

                    meh.Store(user);
                    meh.SaveChanges();
                }

                #endregion

                using (var session = NewSession())
                {
                    #region Creating the todo list
                    var todoList = new TodoList()
                    {
                        Deadline = new DateTime(2010, 11, 20),
                        Name = "Finish this presentation",
                        Items = new List<TodoItem>()
                    {
                        new TodoItem(){
                            Category = "DDD9",
                            IsComplete = false,
                            Title = "PPT",
                            Description = "Create a powerpoint",
                           
                        },                        
                        new TodoItem(){
                            Category = "DDD9",
                            IsComplete = false,
                            Title = "Awkwardly inject humour into slides",
                            Description = "Think of some way to make fun of t'south",
                                
                        },
                        new TodoItem(){
                            Category = "DDD9",
                            IsComplete = false,
                            Title = "Tests",
                            Description = "Create a load of tests for demonstration purposes",
                             
                        }
                    }
                    };

                    #endregion

                    // We only store an id to the user
                    // As far as Raven is concerned it is
                    // JUST ANOTHER PROPERTY. Remember that.
                    todoList.UserId = user.Id;

                    // NOTE: Serializes ENTIRE document into one giant JSON blob!!
                    session.Store(todoList);
                    session.SaveChanges();
                }
            }
        
    }
}
