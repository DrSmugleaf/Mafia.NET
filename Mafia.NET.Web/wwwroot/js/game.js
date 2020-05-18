"use strict"

const divMessages = document.querySelector("#game-messages");
const headerNotification = document.querySelector("#notification");
const inputMessage = document.querySelector("#message-input");
const divGraveyard = document.querySelector("#graveyard-list");

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/GameChat")
    .build();

connection.on("Message", message => {
    message = sanitizeHtml(message);
    let m = document.createElement("div");

    m.innerHTML =
        `<div>${message}</div>`;

    divMessages.appendChild(m);
    divMessages.scrollTop = divMessages.scrollHeight;
});

connection.on("Notification", message => {
    $(headerNotification).text(message);
});

connection.on("Death", message => {
    message = sanitizeHtml(message);
    let m = document.createElement("div");

    m.innerHTML =
        `<div class="dropdown-item">${message}</div>`;

    divGraveyard.appendChild(m);
    divMessages.scrollTop = divMessages.scrollHeight;
});

connection.start().then(() => {});

inputMessage.addEventListener("keyup", e => {
    if (e.key === "Enter") {
        send();
    }
});

function send() {
    connection.send("NewMessage", inputMessage.value)
        .then(() => inputMessage.value = "");
}