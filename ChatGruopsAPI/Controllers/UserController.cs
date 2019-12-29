using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatGroupsAPI.Models;
using ChatGruopsAPI.Models;
using DomainCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatGroupsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        DomainCore.Interfaces.IDataRepo _repo;

        public UserController(DomainCore.Interfaces.IDataRepo repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<UserViewModel> GetUserGroups(string user, string game)
        {
            CoreEngine core = new CoreEngine();
            var userObj = await core.GetUserGroupsAsync(_repo, game, user);

            UserViewModel userGroups = new UserViewModel
            {
                UserName = userObj.UserName,
                Game = userObj.Game,
                UserGroups = from ug in userObj.UserGroups select new GroupNameViewModel { Id = ug.Id, Name = ug.Name},
                OtherGroups = from og in userObj.OtherGroups select new GroupNameViewModel { Id = og.Id, Name = og.Name },
            };

            return userGroups;
        }

        [HttpPost]
        public async Task<MessageResponse> JoinOrLeaveGroup(UserGroupActionModel action)
        {
            CoreEngine core = new CoreEngine();

            var msg = await core.JoinOrLeaveGroup(_repo, action.User, action.Group, action.Location);

            var response = new MessageResponse { Message = msg };

            return response;
        }
    }
}