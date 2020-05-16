"use strict"

const divMessages = document.querySelector("#messages");
const messageInput = document.querySelector("#message-input");
const buttonStart = document.querySelector("#lobby-start");
const setupRoleList = document.querySelector("#setup-role-list")

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
            $(messageInput).prop("disabled", true);
            $(buttonStart).prop("disabled", true);
            $(".catalog-team").prop("disabled", true);
            $(".catalog-role").prop("disabled", true);
            $(".setup-role").prop("disabled", true);
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
    
    $(setupRoleList).append('<button type="button" class="btn btn-sm list-group-item setup-role" style="color: ' + color + '">' + name + '</button>')
    
    $(".setup-role").click(function() {
        $(this).remove();
    })
})