using Auto.Data;
using Auto.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApplication1.Hubs;

namespace WebApplication1.Controllers;

public class OwnersController : Controller
{
    private readonly IAutoDatabase db;
    private readonly IHubContext<OwnerHub> hub;

    public OwnersController(IAutoDatabase db, IHubContext<OwnerHub> hub)
    {
        this.hub = hub;
        this.db = db;
    }
    [HttpGet]
    public IActionResult Index()
    {
        var list = db.ListOwners();
        return View(list);
    }
    
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind("Name,SecondName,Mail,PhoneNumber,VehicleRegistration")] OwnerViewModel ownerViewModel)
    {
        if (ModelState.IsValid)
        {
            var newOwner = new Owner
            {
                FirstName = ownerViewModel.Name,
                SecondName = ownerViewModel.SecondName,
                VehicleRegistration = ownerViewModel.VehicleRegistration,
                PhoneNumber = ownerViewModel.PhoneNumber,
                Mail = ownerViewModel.Mail
            };
            db.CreateOwner(newOwner);
            //await hub.Clients.Group("owners").SendAsync("GetMessage", ownerViewModel);
            return Redirect("/owners");
        }
        else
        {
            return View(ownerViewModel);
        }
    }

}