using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Raven.Client.Document;
using Raven.Client.Client;
using Raven.Client;

namespace Demos
{
    [TestFixture]
    public class Connecting
    {
        [Test]
        public void LookAtMeICanConnectToHttpServer()
        {
            // Application start-up
            IDocumentStore store = new DocumentStore()
            {
                Url = "http://10.0.2.2:8080"
            };
            store.Initialize();

            // Per unit of work
            using (var session = store.OpenSession())
            {
                
            }
        }

        [Test]
        public void AndICanAlsoCanRunInProcess()
        {
            // Application start-up
            IDocumentStore store = new EmbeddableDocumentStore()
            {
                Configuration = new Raven.Database.RavenConfiguration()
                {
                    DataDirectory = "Data"
                }
            };
            store.Initialize();

            // Per unit of work
            using (var session = store.OpenSession())
            {

            }
        }

        [Test]
        public void AndEntirelyInMemoryForUberFastTestingHAHAHAHA()
        {
            // Application start-up
            IDocumentStore store = new EmbeddableDocumentStore()
            {
                Configuration = new Raven.Database.RavenConfiguration()
                {
                    RunInMemory = true
                }
            };
            store.Initialize();

            // Per unit of work
            using (var session = store.OpenSession())
            {

            }
        }
    }
}
