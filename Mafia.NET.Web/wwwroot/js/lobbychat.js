"use strict"

const divMessages = document.querySelector("#messages");
const messageInput = document.querySelector("#message-input");
const buttonStart = document.querySelector("#lobby-start");

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
        connection.send("Start")
            .then($(buttonStart).prop("disabled", true))
    })
})