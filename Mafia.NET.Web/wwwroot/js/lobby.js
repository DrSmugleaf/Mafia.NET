"use strict"

const divMessages = document.querySelector("#lobby-messages");
const messageInput = document.querySelector("#message-input");
const buttonStart = document.querySelector("#lobby-start");
const setupRoleList = document.querySelector("#setup-role-list")
const addRoleButton = document.querySelector("#add-role-button")
const removeRoleButton = document.querySelector("#remove-role-button")

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/LobbyChat")
    .build();

connection.on("Message", (message) => {
    message = sanitizeHtml(message);
    let m = document.createElement("div");

    m.innerHTML =
        `<div>${message}</div>`;

    divMessages.appendChild(m);
    divMessages.scrollTop = divMessages.scrollHeight;
});

connection.on("Start", () => {
    $(location).attr("href", "Game")
})

connection.start().then(() => {
    $(messageInput).prop("disabled", false);
    $(buttonStart).prop("disabled", false);
}).catch(err => document.write(err));

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
    $(buttonStart).click(() => {
        connection.send("Start").then(() => {
            $(".disable-on-start").prop("disabled", true);
            $("input").prop("disabled", true);
            $("select").prop("disabled", true);
        })
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
    
    $(".setup-role").click(function() {
        $(this).remove();
    })
})

$(addRoleButton).click(function() {
    const roleEntry = $("#role-name");
    const roleName = roleEntry.text();
    if (!roleName) return;
    
    const color = roleEntry.css("color");
    const role = $('<button type="button" class="btn btn-sm list-group-item setup-role disable-on-start" data-name="' + roleName + '" style="color: ' + color + '">' + roleName + '</button>');
    $(setupRoleList).append(role);
    
    role.click(function() {
        $(this).remove();
    })
})

$(removeRoleButton).click(function() {
    const roleEntry = $("#role-name");
    const roleName = roleEntry.text();
    if (!roleName) return;
    
    const role = $('.setup-role[data-name="' + roleName + '"]').last();
    role.remove();
})