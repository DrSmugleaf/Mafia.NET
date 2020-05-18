"use strict"

const divMessages = document.querySelector("#lobby-messages");
const messageInput = document.querySelector("#message-input");
const buttonStart = document.querySelector("#lobby-start");
const setupRoleList = document.querySelector("#setup-role-list");
const addRoleButton = document.querySelector("#add-role-button");
const removeRoleButton = document.querySelector("#remove-role-button");
const divSetupInformation = document.querySelector("#preset-information");
const divSetupName = document.querySelector("#preset-name");
const divSetupDescription = document.querySelector("#preset-description");
const buttonPresetUse = document.querySelector("#preset-use");
const listPlayers = document.querySelector("#player-list");

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/LobbyChat")
    .build();

connection.on("Message", message => {
    message = sanitizeHtml(message);
    let m = document.createElement("div");

    m.innerHTML =
        `<div>${message}</div>`;

    divMessages.appendChild(m);
    divMessages.scrollTop = divMessages.scrollHeight;
});

connection.on("Start", () => {
    $(location).attr("href", "Game");
})

connection.on("Players", names => {
    $(listPlayers).children().remove();
    
    for (const name of names) {
        const player = $('<li class="list-group-item player-entry"></li>');
        player.text(name);
        $(listPlayers).append(player);
    }
})

connection.on("HostPlayer", host => {
    $(listPlayers).children().removeClass("active");
    $('li.player-entry:contains("' + host + '")').addClass("active");
})

connection.on("Host", () => {
    $(".host-only").prop("disabled", false);
})

connection.on("Unhost", () => {
    $(".host-only").prop("disabled", true);
})

connection.on("Join", name => {
    const player = $('<li class="list-group-item player-entry"></li>');
    player.text(name);
    $(listPlayers).append(player);
})

connection.on("Leave", name => {
    $('li.player-entry:contains("' + name + '")').first().remove();
})

connection.start().then(function () {
    $(messageInput).prop("disabled", false);
})

messageInput.addEventListener("keyup", (e) => {
    if (e.key === "Enter") {
        send();
    }
});

function send() {
    connection.send("NewMessage", messageInput.value)
        .then(() => messageInput.value = "");
}

$(document).ready(() => {
    (function($){
        $.event.special.destroyed = {
            remove: function(o) {
                if (o.handler) {
                    o.handler.apply(this, arguments);
                }
            }
        }
    })(jQuery);
    
    $(".host-only").prop("disabled", true);
    
    $(buttonStart).click(() => {
        // connection.send("Start").then(() => {
        //     $(".disable-on-start").prop("disabled", true);
        //     $("input").prop("disabled", true);
        //     $("select").prop("disabled", true);
        // })
    })
})

$(".catalog-team").click(function() {
    const team = $(this).data("team");
    $(".catalog-roles-list").addClass("d-none");
    $('*[data-team="' + team + '"]').removeClass("d-none");
})

$(".catalog-role").click(function() {
    const name = $(this).data("name");
    const color = $(this).data("color");
    const summary = $(this).data("summary");
    const abilities = $(this).data("abilities");
    const goal = $(this).data("goal");
    
    $("#role-name").text(name).css("color", color);
    $("#role-summary").text(summary);
    $("#role-abilities").text(abilities);
    $("#role-goal").text(goal);
    $(addRoleButton).removeClass("d-none");
    $(removeRoleButton).removeClass("d-none");
    
    $(".setup-role").click(function() {
        $(this).remove();
    })
})

$(addRoleButton).click(function() {
    const roleEntry = $("#role-name");
    const roleName = roleEntry.text();
    if (!roleName) return;
    
    const color = roleEntry.css("color");
    const newIndex = $(setupRoleList).children("button.setup-role").length;
    
    const newRole = $('<button type="button" class="btn btn-sm list-group-item setup-role disable-on-start host-only" data-index="' + newIndex + '" data-name="' + roleName + '" style="color: ' + color + '">' + roleName + '</button>');
    $(setupRoleList).append(newRole);
    
    const input = $('<input type="hidden" data-index="' + newIndex + '" data-role="' + roleName + '" id="Roles_' + newIndex + '_" name="Roles[' + newIndex + ']">')
    input.val(roleName);
    $(setupRoleList).append(input);
    
    newRole.click(function() {
        $(this).remove();
    })
    
    $(newRole).bind("destroyed", function() {
        input.remove();
        
        const index = $(this).attr("data-index");
        const roles = $(setupRoleList).children("button.setup-role");
        let i = 0;
        for (const role of roles) {
            const oldRole = $(role);
            const oldIndex = oldRole.attr("data-index");
            if (oldIndex === index) continue;

            const oldInput = $('input[type=hidden][data-index="' + oldIndex + '"]');
            oldRole.attr("data-index", i);
            oldInput.attr("data-index", i);
            oldInput.attr("id", "Roles_" + i + "_");
            oldInput.attr("name", "Roles[" + i + "]");
            
            i++;
        }
    })
})

$(removeRoleButton).click(function() {
    const roleEntry = $("#role-name");
    const roleName = roleEntry.text();
    if (!roleName) return;
    
    const role = $('.setup-role[data-name="' + roleName + '"]').last();
    role.remove();
})

$(".preset-entry").click(function() {
    const setupName = $(this).text();
    $(divSetupName).text(setupName);
    
    const setupDescription = $(this).data("description");
    $(divSetupDescription).text(setupDescription);
    
    $(buttonPresetUse).removeClass("d-none");
    
    const setupId = $(this).data("preset");
    $(divSetupInformation).data("preset", setupId);
})

$(buttonPresetUse).click(function() {
    const presetId = $(divSetupInformation).data("preset");
    
    $(".catalog-role").removeClass("d-none");
    const presetButton = $('button[data-preset="'+ presetId + '"]').first();
    const disabledRoles = presetButton.data("disabled-roles");
    if (disabledRoles) {
        for (const role of disabledRoles) {
            $('.catalog-role[data-name="' + role + '"]').addClass("d-none");
            $('.setup-role[data-name="' + role + '"]').remove();

            const selectedRole = $("#role-name");
            if (selectedRole.text() !== role) continue;

            selectedRole.text("").css("color", "");
            $("#role-summary").text("");
            $("#role-abilities").text("");
            $("#role-goal").text("");
            $(addRoleButton).addClass("d-none");
            $(removeRoleButton).addClass("d-none");
        }
    }
    
    $("button.preset-entry").removeClass("btn-primary");
    presetButton.addClass("btn-primary");
})