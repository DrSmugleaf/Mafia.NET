"use strict"

const divMessages = document.querySelector("#messages");
const messageInput = document.querySelector("#message-input");

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

connection.start().catch(err => document.write(err));

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
    $("#lobby-start").click(() => {
        connection.send("Start")
    })
})