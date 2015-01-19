using System;
using System.Linq;
using NUnit.Framework;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence.Repositories;
using Umbraco.Core.Persistence.UnitOfWork;
using Umbraco.Tests.TestHelpers;

namespace Umbraco.Tests.Persistence.Repositories
{
    [DatabaseTestBehavior(DatabaseBehavior.NewDbFileAndSchemaPerTest)]
    [TestFixture]
    public class TaskRepositoryTest : BaseDatabaseFactoryTest
    {
        [Test]
        public void Can_Add()
        {
            var provider = new PetaPocoUnitOfWorkProvider(Logger);
            var unitOfWork = provider.GetUnitOfWork();
            using (var repo = new TaskRepository(unitOfWork, CacheHelper, Logger, SqlSyntaxProvider))
            {
                var created = DateTime.Now;
                repo.AddOrUpdate(new Task(new TaskType("asdfasdf"))
                {
                    AssigneeUserId = 0,
                    Closed = false,
                    Comment = "hello world",
                    EntityId = -1,
                    OwnerUserId = 0
                });
                unitOfWork.Commit();

                var found = repo.GetAll().ToArray();

                Assert.AreEqual(1, found.Count());
                Assert.AreEqual(0, found.First().AssigneeUserId);
                Assert.AreEqual(false, found.First().Closed);
                Assert.AreEqual("hello world", found.First().Comment);
                Assert.GreaterOrEqual(found.First().CreateDate, created);
                Assert.AreEqual(-1, found.First().EntityId);
                Assert.AreEqual(0, found.First().OwnerUserId);
                Assert.AreEqual(true, found.First().HasIdentity);
                Assert.AreEqual(true, found.First().TaskType.HasIdentity);
            }            
        }

        [Test]
        public void Get_All()
        {
            CreateTestData(false, 20);

            var provider = new PetaPocoUnitOfWorkProvider(Logger);
            var unitOfWork = provider.GetUnitOfWork();
            using (var repo = new TaskRepository(unitOfWork, CacheHelper, Logger, SqlSyntaxProvider))
            {
                var found = repo.GetAll().ToArray();
                Assert.AreEqual(20, found.Count());
            }
        }

        [Test]
        public void Get_All_With_Closed()
        {
            CreateTestData(false, 10);
            CreateTestData(true, 5);

            var provider = new PetaPocoUnitOfWorkProvider(Logger);
            var unitOfWork = provider.GetUnitOfWork();
            using (var repo = new TaskRepository(unitOfWork, CacheHelper, Logger, SqlSyntaxProvider))
            {
                var found = repo.GetTasks(includeClosed: true).ToArray();
                Assert.AreEqual(15, found.Count());
            }
        }

        [Test]
        public void Get_All_Without_Closed()
        {
            CreateTestData(false, 10);
            CreateTestData(true, 5);

            var provider = new PetaPocoUnitOfWorkProvider(Logger);
            var unitOfWork = provider.GetUnitOfWork();
            using (var repo = new TaskRepository(unitOfWork, CacheHelper, Logger, SqlSyntaxProvider))
            {
                var found = repo.GetTasks(includeClosed: false);
                Assert.AreEqual(10, found.Count());
            }
        }

        private void CreateTestData(bool closed, int count)
        {
            var provider = new PetaPocoUnitOfWorkProvider(Logger);
            var unitOfWork = provider.GetUnitOfWork();
            using (var repo = new TaskRepository(unitOfWork, CacheHelper, Logger, SqlSyntaxProvider))
            {
                for (int i = 0; i < count; i++)
                {
                    repo.AddOrUpdate(new Task(new TaskType("asdfasdf"))
                    {
                        AssigneeUserId = 0,
                        Closed = closed,
                        Comment = "hello world " + i,
                        EntityId = -1,
                        OwnerUserId = 0
                    });
                    unitOfWork.Commit();
                }
                
            }
        }
    }
}