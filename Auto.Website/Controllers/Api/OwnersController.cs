using System;
using System.Dynamic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;
using Auto.Data;
using Auto.Data.Entities;
using Auto.Messages;
using Auto.Website.Models;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;

namespace Auto.Website.Controllers.Api
{
    [Route("api/owners")]
    [ApiController]
    public class OwnersController : ControllerBase {
        private readonly IAutoDatabase db;
        private readonly IBus bus;

        public OwnersController(IAutoDatabase db,IBus bus) {
            this.db = db;
            this.bus = bus;
        }
        
        private dynamic Paginate(string url, int index , int total,int count = 5) {
            dynamic links = new ExpandoObject();
            links.self = new { href = url };
            links.final = new { href = $"{url}?index={total - (total % count)}&count={count}" };
            links.first = new { href = $"{url}?index=0&count={count}" };
            if (index > 0) links.previous = new { href = $"{url}?index={index - count}&count={count}" };
            if (index + count < total) links.next = new { href = $"{url}?index={index + count}&count={count}" };
            return links;
        }
        
        [HttpGet]
        [Produces("application/hal+json")]
        public IActionResult Get(int index = 0,int count = 5)
        {
            var items = db.ListOwners().Skip(index).Take(count);
            var totalItems = db.CountOwners();
            var _links = Paginate("/api/owners/", index, totalItems, count);
            var _actions = new
            {
                create = new
                {
                    method = "POST",
                    type = "application/json",
                    name = "Add a new owner",
                    href = "/api/owners"
                },
                delete = new
                {
                    method = "DELETE",
                    name = "Delete an owner",
                    href = "/api/owners/{id}"
                }
            };
            var result = new
            {
                _links,
                _actions,
                total = totalItems,
                index,
                count,
                items
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var owner = db.FindOwner(id);
            if (owner == null) return NotFound();
            
            var result = owner.ToDynamic();

            result._links = new
            {
                self = $"/api/owners/{owner.OwnerId}",
                vehicle = $"/api/vehicles/{owner.VehicleRegistration}"
            };

            result._actions = new
            {
                put = new
                {
                    method = "PUT",
                    href = $"/api/owners/{id}",
                    accept = "/application/json"
                },
                delete = new
                {
                    method = "DELETE",
                    href = $"/api/owners/{id}",
                }
            };
            return Ok(result);
        }

        [HttpPost]
        public async  Task<IActionResult> Post([FromBody] OwnerDTO dto)
        {
            var newOwner = new Owner
            {
                Name = dto.Name,
                SecondName = dto.SecondName,
                Mail = dto.Mail,
                PhoneNumber = dto.PhoneNumber,
                VehicleRegistration = dto.VehicleRegistration
            };
            var ownerId = db.CreateOwner(newOwner);
            var o = db.FindOwner(ownerId);
            Console.WriteLine(o.OwnerId);
            await PublishNewOwnerMessage(db.FindOwner(ownerId));
            if (ownerId == null) return BadRequest();
            return Get(ownerId);
        }

        private async Task PublishNewOwnerMessage(Owner owner)
        {
            var newOwnerMessage = new NewOwnerMessage
            {
                Name = owner.Name,
                SecondName = owner.SecondName,
                Mail = owner.Mail,
                PhoneNumber = owner.PhoneNumber,
                OwnerId = owner.OwnerId,
                VehicleRegistration = owner.VehicleRegistration,
                CreatedAtUtc = DateTime.UtcNow
            };
            await bus.PubSub.PublishAsync(newOwnerMessage);
        }
        
        
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] OwnerDTO dto)
        {
            var owner = db.FindOwner(id);
            if (owner == null) return NotFound();
            var updatedOwner = new Owner
            {
                OwnerId = id,
                Name = dto.Name,
                SecondName = dto.SecondName,
                Mail = dto.Mail,
                PhoneNumber = dto.PhoneNumber,
                VehicleRegistration = dto.VehicleRegistration
            };
            db.UpdateOwner(updatedOwner);
            return Get(id);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            db.DeleteOwner(id);
            return Ok();
        }
    }
}

