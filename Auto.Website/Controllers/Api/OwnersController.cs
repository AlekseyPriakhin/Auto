using System.Dynamic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using Auto.Data;
using Auto.Data.Entities;
using Auto.Website.Models;
using Microsoft.AspNetCore.Mvc;

namespace Auto.Website.Controllers.Api
{
    [Route("api/owners")]
    [ApiController]
    public class OwnersController : ControllerBase {
        private readonly IAutoDatabase db;

        public OwnersController(IAutoDatabase db) {
            this.db = db;
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
                vehicle = $"/api/vehicles/{owner.VehicleCode}"
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

        public IActionResult Post([FromBody] OwnerDTO dto)
        {
            var newOwner = new Owner
            {
                Name = dto.Name,
                SecondName = dto.SecondName,
                Mail = dto.Mail,
                PhoneNumber = dto.PhoneNumber,
                VehicleCode = dto.VehicleCode
            };
            var ownerId = db.CreateOwner(newOwner);
            if (ownerId == null) return BadRequest();
            return Get(ownerId);
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
                VehicleCode = dto.VehicleCode
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

