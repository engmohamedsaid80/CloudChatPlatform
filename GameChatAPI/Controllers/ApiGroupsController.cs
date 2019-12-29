using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainCore;
using GameChatAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameChatAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiGroupsController : ControllerBase
    {
        DomainCore.Interfaces.IDataRepo _repo;

        public ApiGroupsController(DomainCore.Interfaces.IDataRepo repo)
        {
            _repo = repo;
        }

        // GET: api/ApiGroups
        [HttpGet]
        public async Task<IEnumerable<GroupViewModel>> GetAllGroups(string game)
        {
            CoreEngine core = new CoreEngine();

            var result = await core.GetGameGroupsAsync(_repo, game);

            IEnumerable<GroupViewModel> groups = from res in result
                                                 select new GroupViewModel
                                                 {
                                                     Name = res.Name,
                                                     Owner = res.Owner,
                                                     Game = res.Game
                                                 };
            return groups;
        }

        // POST: api/ApiGroups
        [HttpPost]
        public async Task CreateGroup(GroupViewModel group)
        {
            CoreEngine core = new CoreEngine();

            DomainCore.Models.GroupModel g = new DomainCore.Models.GroupModel
            {
                Game = group.Game,
                Name = group.Name,
                Owner = group.Owner,
                MemberCount = 1
            };
            core.CreateGroup(_repo, g);
        }

        // PUT: api/ApiGroups/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
