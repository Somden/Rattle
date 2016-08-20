using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Rattle.Tests
{
    //[TestFixture]
    //public class EventStoreTests
    //{
    //    private AmazonDynamoDBClient _amazonDynamoDbClient;
    //    private EventStore _eventStore;
    //    private UserAccountRepository _userAccountRepository;
        
    //    [SetUp]
    //    public void SetUp()
    //    {
    //        try
    //        {
    //            AmazonDynamoDBConfig ddbConfig = new AmazonDynamoDBConfig();
    //            ddbConfig.ServiceURL = "http://localhost:8000";
    //            _amazonDynamoDbClient = new AmazonDynamoDBClient("AKIAJY7EDMWJPJZLVK4A", "D4IfaYo8efzejbwgc+Js5+QqB+VdHthbGg6sfgB2", ddbConfig);             
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine(e);
    //        }
            
    //        CreateTable();
    //        _eventStore = new EventStore(_amazonDynamoDbClient);
    //        _userAccountRepository = new UserAccountRepository(_eventStore);
    //    }

    //    [TearDown]
    //    public void TearDown()
    //    {
    //        DeleteTable();
    //        _amazonDynamoDbClient.Dispose();
    //    }
        
    //    [Test]
    //    public void Should_Create_User_Accout()
    //    {
    //        var id = Guid.NewGuid();
    //        var userAccount = new UserAccount(id);
    //        var userName = "ololo";
    //        userAccount.ChangeUserName(userName);
    //        _userAccountRepository.Create(userAccount);

    //        UserAccount getUac = _userAccountRepository.Get(id);

    //        Assert.That(getUac, Is.Not.Null);
    //        Assert.That(getUac.Id, Is.EqualTo(id));
    //        Assert.That(getUac.UserName, Is.EqualTo(userName));
    //        Assert.That(getUac.Version, Is.EqualTo(2));
    //    }

    //    [Test]
    //    public void Should_Update_User_Accout()
    //    {
    //        var id = Guid.NewGuid();
    //        _userAccountRepository.Create(new UserAccount(id));

    //        UserAccount userAccount = _userAccountRepository.Get(id);
    //        var userName = "ololo";
    //        userAccount.ChangeUserName(userName);
    //        _userAccountRepository.Save(userAccount);

    //        Assert.That(userAccount, Is.Not.Null);
    //        Assert.That(userAccount.Id, Is.EqualTo(id));
    //        Assert.That(userAccount.UserName, Is.EqualTo(userName));
    //        Assert.That(userAccount.Version, Is.EqualTo(2));
    //    }

    //    [Test]
    //    public void Should_Use_Snapshot()
    //    {
    //        UserAccountSnapshot snapshot = null;
    //        var userAccountId = Guid.NewGuid();
    //        var streamName = typeof(UserAccount).ToStreamName(userAccountId);

    //        // aggregate not exists yet
    //        snapshot = _eventStore.GetLatestSnapshot<UserAccountSnapshot>(streamName);
    //        Assert.That(snapshot, Is.Null);

    //        // aggregate created but theren't any snapshots
    //        var userAccount = new UserAccount(userAccountId);
    //        userAccount.ChangeUserName("userName1");
    //        _userAccountRepository.Create(userAccount);
    //        snapshot = _eventStore.GetLatestSnapshot<UserAccountSnapshot>(streamName);
    //        Assert.That(snapshot, Is.Null);

    //        // take first snapshot
    //        userAccount = _userAccountRepository.Get(userAccountId);
    //        TaskSnapshot(userAccountId);
    //        snapshot = _eventStore.GetLatestSnapshot<UserAccountSnapshot>(streamName);
    //        Assert.That(snapshot.Version, Is.EqualTo(userAccount.Version));

    //        // make change but not take snapshot
    //        userAccount = _userAccountRepository.Get(userAccountId);
    //        userAccount.ChangeUserName("userName2");
    //        _userAccountRepository.Save(userAccount);
    //        snapshot = _eventStore.GetLatestSnapshot<UserAccountSnapshot>(streamName);
    //        Assert.That(snapshot.Version, Is.EqualTo(userAccount.Version - 1));
    //        Assert.That(userAccount.UserName, Is.EqualTo("userName2"));
    //        Assert.That(snapshot.UserName, Is.EqualTo("userName1"));

    //        // change name an take snapshot
    //        userAccount = _userAccountRepository.Get(userAccountId);
    //        userAccount.ChangeUserName("userName3");
    //        _userAccountRepository.Save(userAccount);
    //        TaskSnapshot(userAccountId);
    //        snapshot = _eventStore.GetLatestSnapshot<UserAccountSnapshot>(streamName);
    //        Assert.That(snapshot.Version, Is.EqualTo(userAccount.Version));
    //        Assert.That(userAccount.UserName, Is.EqualTo(snapshot.UserName));
    //    }

    //    private void TaskSnapshot(Guid userAccountId)
    //    {
    //        var userAccount = _userAccountRepository.Get(userAccountId);
    //        var snapshot = userAccount.GetUserAccountSnapshot();
    //        _eventStore.AddSnapshot(typeof(UserAccount).ToStreamName(userAccount.Id), snapshot);
    //    }

    //    private void CreateTable()
    //    {
    //        CreateTableRequest createEventStreamTableRequest = new CreateTableRequest
    //        {
    //            TableName = "EventStream",
    //            AttributeDefinitions = new List<AttributeDefinition>()
    //            {
    //                new AttributeDefinition
    //                {
    //                    AttributeName = "Id",
    //                    AttributeType = "S"
    //                }
    //            },
    //            KeySchema = new List<KeySchemaElement>()
    //            {
    //                new KeySchemaElement
    //                {
    //                    AttributeName = "Id",
    //                    KeyType = "HASH"
    //                }
    //            },
    //            ProvisionedThroughput = new ProvisionedThroughput(1, 1),
    //        };

    //        CreateTableRequest createEventWrapperTableRequest = new CreateTableRequest
    //        {
    //            TableName = "EventWrapper",
    //            AttributeDefinitions = new List<AttributeDefinition>()
    //            {
    //                new AttributeDefinition
    //                {
    //                    AttributeName = "EventStreamId",
    //                    AttributeType = "S"
    //                },
    //                new AttributeDefinition
    //                {
    //                    AttributeName = "EventNumber",
    //                    AttributeType = "N"
    //                }
    //            },
    //            KeySchema = new List<KeySchemaElement>()
    //            {
    //                new KeySchemaElement
    //                {
    //                    AttributeName = "EventStreamId",
    //                    KeyType = "HASH"
    //                },
    //                new KeySchemaElement
    //                {
    //                    AttributeName = "EventNumber",
    //                    KeyType = "RANGE"
    //                }
    //            },
    //            ProvisionedThroughput = new ProvisionedThroughput(1, 1),
    //        };

    //        CreateTableRequest createSnapshoCreateTableRequest = new CreateTableRequest
    //        {
    //            TableName = "SnapshotWrapper",
    //            AttributeDefinitions = new List<AttributeDefinition>()
    //            {
    //                new AttributeDefinition
    //                {
    //                    AttributeName = "StreamName",
    //                    AttributeType = "S"
    //                }
    //            },
    //            KeySchema = new List<KeySchemaElement>()
    //            {
    //                new KeySchemaElement
    //                {
    //                    AttributeName = "StreamName",
    //                    KeyType = "HASH"
    //                }
    //            },
    //            ProvisionedThroughput = new ProvisionedThroughput(1, 1),
    //        };

    //        // SnapshotWrapper
    //        var createResponse1 = _amazonDynamoDbClient.CreateTable(createEventStreamTableRequest);
    //        var createResponse2 = _amazonDynamoDbClient.CreateTable(createEventWrapperTableRequest);
    //        var createResponse3 = _amazonDynamoDbClient.CreateTable(createSnapshoCreateTableRequest);
    //    }

    //    private void DeleteTable()
    //    {
    //        _amazonDynamoDbClient.DeleteTable("EventStream");
    //        _amazonDynamoDbClient.DeleteTable("EventWrapper");
    //        _amazonDynamoDbClient.DeleteTable("SnapshotWrapper");
    //    }
    //}
}
