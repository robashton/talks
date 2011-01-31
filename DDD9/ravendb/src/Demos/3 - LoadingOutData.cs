using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Demos.Domain;

namespace Demos
{
    [TestFixture]
    public class LoadingOutData : RavenTest
    {
        [Test]
        public void LookHowEasyItIsToLoadItemsOutById()
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
                Assert.NotNull(user);
            }
        }
    }
}
