using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Raven.Client.Client;
using NUnit.Framework;
using System.Threading;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace Demos
{
    public class RavenTest
    {
        EmbeddableDocumentStore store;

        [SetUp]
        public void Setup()
        {
            store = new EmbeddableDocumentStore()
            {
             // Url = "http://10.0.2.2:8080"
               RunInMemory = true
            };
            store.Initialize();
            IndexCreation.CreateIndexes(this.GetType().Assembly, store);
        }

        [TearDown]
        public void Teardown()
        {
            store.Dispose();
        }

        public void WaitForIndexing()
        {
            while (store.DocumentDatabase.Statistics.StaleIndexes.Length > 0)
            {
                Thread.Sleep(200);
            }
        }

        public IDocumentSession NewSession()
        {
            return store.OpenSession();
        }
    }
}
