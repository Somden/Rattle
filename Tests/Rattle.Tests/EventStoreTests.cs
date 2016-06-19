using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using NUnit.Framework;
using Rattle.Domain.UserAccounts;
using Rattle.Infrastructure;
using Rattle.Infrastructure.Repositories;

namespace Rattle.Tests
{
    [TestFixture]
    public class EventStoreTests
    {
        private AmazonDynamoDBClient _amazonDynamoDbClient;

        [SetUp]
        public void SetUp()
        {
            try
            {
                AmazonDynamoDBConfig ddbConfig = new AmazonDynamoDBConfig();
                ddbConfig.ServiceURL = "http://localhost:8000";
                _amazonDynamoDbClient = new AmazonDynamoDBClient("AKIAJY7EDMWJPJZLVK4A", "D4IfaYo8efzejbwgc+Js5+QqB+VdHthbGg6sfgB2", ddbConfig);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            CreateTable();
        }

        [TearDown]
        public void TearDown()
        {
            DeleteTable();
            _amazonDynamoDbClient.Dispose();
        }
        
        [Test]
        public void Should_Create_User_Accout()
        {
            var eventStore = new EventStore(_amazonDynamoDbClient);
            var userAccountRepository = new UserAccountRepository(eventStore);
            var id = Guid.NewGuid();
            var userAccount = new UserAccount(id);
            var userName = "ololo";
            userAccount.ChangeUserName(userName);
            userAccountRepository.Create(userAccount);

            UserAccount getUac = userAccountRepository.FindBy(id);

            Assert.That(getUac, Is.Not.Null);
            Assert.That(getUac.Id, Is.EqualTo(id));
            Assert.That(getUac.UserName, Is.EqualTo(userName));
            Assert.That(getUac.Version, Is.EqualTo(2));
        }

        [Test]
        public void Should_Update_User_Accout()
        {
            var eventStore = new EventStore(_amazonDynamoDbClient);
            var userAccountRepository = new UserAccountRepository(eventStore);
            var id = Guid.NewGuid();
            userAccountRepository.Create(new UserAccount(id));

            UserAccount userAccount = userAccountRepository.FindBy(id);
            var userName = "ololo";
            userAccount.ChangeUserName(userName);
            userAccountRepository.Save(userAccount);

            Assert.That(userAccount, Is.Not.Null);
            Assert.That(userAccount.Id, Is.EqualTo(id));
            Assert.That(userAccount.UserName, Is.EqualTo(userName));
            Assert.That(userAccount.Version, Is.EqualTo(2));
        }

        private void CreateTable()
        {
            CreateTableRequest createEventStreamTableRequest = new CreateTableRequest
            {
                TableName = "EventStream",
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Id",
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>()
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Id",
                        KeyType = "HASH"
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput(1, 1),
            };

            CreateTableRequest createEventWrapperTableRequest = new CreateTableRequest
            {
                TableName = "EventWrapper",
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Id",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "EventNumber",
                        AttributeType = "N"
                    }
                },
                KeySchema = new List<KeySchemaElement>()
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Id",
                        KeyType = "HASH"
                    },
                    new KeySchemaElement
                    {
                        AttributeName = "EventNumber",
                        KeyType = "RANGE"
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput(1, 1),
            };


            var createResponse1 = _amazonDynamoDbClient.CreateTable(createEventStreamTableRequest);
            var createResponse2 = _amazonDynamoDbClient.CreateTable(createEventWrapperTableRequest);
            Console.WriteLine($"{createResponse1.TableDescription.TableName} status {createResponse1.TableDescription.TableStatus}");
            Console.WriteLine($"{createResponse2.TableDescription.TableName} status {createResponse2.TableDescription.TableStatus}");
        }

        private void DeleteTable()
        {
            _amazonDynamoDbClient.DeleteTable("EventStream");
            _amazonDynamoDbClient.DeleteTable("EventWrapper");
        }
    }
}
