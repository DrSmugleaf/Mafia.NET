"use strict"

const divGraveyard = document.querySelector("#graveyard-list");
const headerDay = document.querySelector("#day")
const clock = document.querySelector("#clock");
const headerNotification = document.querySelector("#notification");
const divMessages = document.querySelector("#game-messages");
const inputMessage = document.querySelector("#message-input");
let clockInterval = null;

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/GameChat")
    .build();

connection.on("Death", message => {
    message = sanitizeHtml(message);
    let m = document.createElement("div");

    m.innerHTML =
        `<div class="dropdown-item">${message}</div>`;

    divGraveyard.appendChild(m);
    divMessages.scrollTop = divMessages.scrollHeight;
});

connection.on("Day", (day) => {
    $(headerDay).text(day);
});

connection.on("Clock Start", (message, duration) => {
    clearInterval(clockInterval);
    $(clock).removeClass("d-none");
    
    const startedAt = new Date() + duration * 1000;
    clockInterval = setInterval(function() {
        const timeLeft = (new Date() - startedAt) / 1000;
        $(clock).text(message + ": " + timeLeft + " seconds");
    }, 1000);
});

connection.on("Clock Stop", () => {
    clearInterval(clockInterval);
    $(clock).addClass("d-none");
});

connection.on("Notification", message => {
    $(headerNotification).text(message);
});

connection.on("Message", message => {
    message = sanitizeHtml(message);
    let m = document.createElement("div");

    m.innerHTML =
        `<div>${message}</div>`;

    divMessages.appendChild(m);
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

$(window).load(function() {
    connection.send("Loaded").then(() => {});
});