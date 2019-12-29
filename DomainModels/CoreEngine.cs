﻿using DomainCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace DomainCore
{
    public class CoreEngine
    {
        public async Task<string> CreateGroup(Interfaces.IDataRepo repo, Models.GroupModel g)
        {
            g.id = g.Name.Replace(' ', '-').ToLower();
            g.MemberCount = 1;

            return await repo.AddGroupAsync(g);
        }

        public async Task<IEnumerable<Models.GroupModel>> GetGameGroupsAsync(Interfaces.IDataRepo repo, string game)
        {
            return await  repo.GetGroupsAsync(game);
        }

        public async Task<UserGroupsModel> GetUserGroupsAsync(Interfaces.IDataRepo repo, string game, string user)
        {
            var userObj = await repo.GetUser(user);
            var gameGroups = await repo.GetGroupsAsync(game);

            UserGroupsModel userGroups = new UserGroupsModel { UserName = user, Game = game};

            userGroups.UserGroups = from g in gameGroups
                                    join ug in userObj.Groups on g.id equals ug
                                     select new GroupNameModel
                                     {
                                         Id = g.id,
                                         Name = g.Name
                                     };

            userGroups.OtherGroups = from g in gameGroups.Where(g => !userObj.Groups.Contains(g.id))
                                     select new GroupNameModel
                                     {
                                         Id = g.id,
                                         Name = g.Name
                                     };

            return userGroups;
        }

        public async Task<string> JoinOrLeaveGroup(Interfaces.IDataRepo repo, string u, string g, string country)
        {
            string msg = "";

            string groupId = g;

            var group = await repo.GetGroupAsync(groupId);
            
            UserModel user = await repo.GetUser(u);

            if (user == null)
            {
                user = new UserModel
                {
                    id = u.ToLower(),
                    UserName = u,
                    Country = country,
                    Groups = new List<string>()
                };
            }

            if (group.MemberCount >= 20)
            {
                msg = group.Name + " reached maximum number of members";
            }
            else if(user.Groups.Contains(groupId))
            {
                // leave group
                user.Groups.Remove(groupId);

                await repo.UpdateUserAsync(user);

                group.MemberCount--;

                msg = u + " left group " +group.Name ;

                await repo.UpdateGroupAsync(group);
            }
            else
            {
                // join group
                user.Groups.Add(groupId);

                await repo.UpdateUserAsync(user);

                group.MemberCount++;

                msg = u + " joined group " + group.Name;

                await repo.UpdateGroupAsync(group);
            }

            

            return msg;
        }

        public async Task<IEnumerable<Models.MessageModel>> GetGroupMessagesAsync(Interfaces.IDataRepo repo, string group)
        {
            return await repo.GetMessagesAsync(group);
        }

        public async Task<string> SendMessageToGroupAsync(Interfaces.IDataRepo repo, Models.MessageModel message)
        {
            return await repo.AddMessageAsync(message);
        }
    }
}
