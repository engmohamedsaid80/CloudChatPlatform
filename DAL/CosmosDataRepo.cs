using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DomainCore.Interfaces;
using DomainCore.Models;
using Microsoft.Azure.Cosmos;

namespace DAL
{
    public class CosmosDataRepo : DomainCore.Interfaces.IDataRepo
    {
        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = "https://ms-ng-nosql.documents.azure.com:443/";
        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = "0LdqjvoMM7DKOn1XYqs2RhaIH42RSV8E6K5TfuoafEPMb9b60vA4weVXUtUKNareYdxGy91fv1AOUP6wpCn2Cw==";

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;
        private Container container;

        private string databaseId = "ChatDatabase";
        private string groupContainerId = "GroupContainer";
        private string userContainerId = "UserContainer";
        private string messageContainerId = "MsgContainer";

        #region prepare
        private async Task PrepareCosmos()
        {
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
        }

        private async Task PrepareGroupContainer()
        {
            this.container = await this.database.CreateContainerIfNotExistsAsync(groupContainerId, "/Game");
        }

        private async Task PrepareUserContainer()
        {
            this.container = await this.database.CreateContainerIfNotExistsAsync(userContainerId, "/Country");
        }

        private async Task PrepareMessageContainer()
        {
            this.container = await this.database.CreateContainerIfNotExistsAsync(messageContainerId, "/GroupName");
        }

        private async Task<IEnumerable<T>> GetItems<T>(string sqlQueryText)
        {
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<T> queryResultSetIterator = this.container.GetItemQueryIterator<T>(queryDefinition);

            List<T> result = new List<T>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<T> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (T g in currentResultSet)
                {
                    result.Add(g);
                }
            }

            return result;

        }
        #endregion

        #region Groups
        public async Task<IEnumerable<GroupModel>> GetGroupsAsync(string game)
        {
            await PrepareCosmos();
            await PrepareGroupContainer();

            return await GetItems<GroupModel>("SELECT * FROM g where g.Game = '"+game+"'");
        }

        public async Task<GroupModel> GetGroupAsync(string gid)
        {
            await PrepareCosmos();
            await PrepareGroupContainer();

            var group = await GetItems<GroupModel>("SELECT * FROM g where g.id = '" + gid + "'");
            return group.FirstOrDefault();
        }
        public async Task<string> AddGroupAsync(GroupModel g)
        {
            await PrepareCosmos();
            await PrepareGroupContainer();

            string msg = "";
            ItemResponse<GroupModel> response = null;
            try
            {
                // Read the item to see if it exists.  
                response = await this.container.ReadItemAsync<GroupModel>(g.id, new PartitionKey(g.Game));

                msg = "Group " + g.Name + " already exists";
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
                response = await this.container.CreateItemAsync<GroupModel>(g, new PartitionKey(g.Game));

                msg = "Group " + g.Name + " is created";
            }

            return msg;
        }

        public async Task<string> UpdateGroupAsync(GroupModel g)
        {
            await PrepareCosmos();
            await PrepareGroupContainer();

            await this.container.ReplaceItemAsync<GroupModel>(g, g.id, new PartitionKey(g.Game));

            return "done";
        }
        #endregion

        #region user
        public async Task<string> UpdateUserAsync(UserModel user)
        {
            await PrepareCosmos();
            await PrepareUserContainer();

            string msg = "";
            ItemResponse<UserModel> response = null;
            try
            {
                // Read the item to see if it exists.  
                response = await this.container.ReadItemAsync<UserModel>(user.id, new PartitionKey(user.Country));
                await this.container.ReplaceItemAsync<UserModel>(user, user.id, new PartitionKey(user.Country));

                msg = "User " + user.UserName + " is updated";
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
                response = await this.container.CreateItemAsync<UserModel>(user, new PartitionKey(user.Country));

                msg = "User " + user.UserName + " is created";
            }

            return msg;
        }

        public async Task<UserModel> GetUser(string u)
        {
            await PrepareCosmos();
            await PrepareUserContainer();

            var user = await GetItems<UserModel>("SELECT * FROM u where u.UserName = '" + u + "'");

            return user.FirstOrDefault();
        }

        #endregion

        #region Messages
        public async Task<IEnumerable<MessageModel>> GetMessagesAsync(string group)
        {
            await PrepareCosmos();
            await PrepareMessageContainer();

            return await GetItems<MessageModel>("SELECT * FROM m where m.GroupName = '" + group + "' ORDER BY m.MessageDT ASC");
        }

        public async Task<string> AddMessageAsync(MessageModel message)
        {
            await PrepareCosmos();
            await PrepareMessageContainer();

            string msg = "";

            await this.container.CreateItemAsync<MessageModel>(message, new PartitionKey(message.GroupName));

            msg = "message is sent to group";

            return msg;
        }
        #endregion
    }
}
