$(document).ready(connectToSignalR);


function connectToSignalR() {
    console.log("Connecting to SignalR...");
    const conn = new signalR.HubConnectionBuilder().withUrl("/ownerhub?group=owners").build();

    window.notificationDivs = [];
    conn.on("GetMessage", handleMessage);
    
    conn.start().then(function () {
        console.log("SignalR has started.");
    }).catch(function (err) {
        console.log(err);
    });
}

function handleMessage(message)
{
    const owner = JSON.parse(message)
    addNewOwnerToTable(owner);
    showNotification(owner);
}

function showNotification(owner)
{
    const message = `New owner has been added:<br>`+
        `first name: ${owner.firstName}<br>`+
        `second name: ${owner.secondName}<br>`+
        `fine status: ${owner.fineStatus}`
    const $target = $("div#signalr-notifications");
    const $div = $(`<div>${message}</div>`);
    $target.prepend($div);
    window.setTimeout(function () { $div.fadeOut(4000, function () { $div.remove(); }); }, 2000);
}


function addNewOwnerToTable(owner)
{
    const propsList = ["firstName","secondName","mail","phoneNumber","vehicleRegistration"]
    for (let propName in owner)
    {
        console.log(`${propName} - ${owner[propName]}`);
    }
    const ownerTable = document.getElementById("owners-table");
    const tr = document.createElement("tr");
    tr.className = "table-row";
    for(const propName in owner)
    {
        if(propsList.includes(propName))
        {
            const td = document.createElement("td");
            td.className = "table-cell";
            td.textContent = owner[propName];
            tr.append(td)
        }
    }
    const body = ownerTable.children[1];
    body.append(tr)
}