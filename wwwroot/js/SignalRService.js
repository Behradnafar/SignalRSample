var chatBox = $("#ChatBox");


var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();

connection.start();

//connection.invoke('SendNewMessage', "Sender is ...", "Client send this msg");

//Show Chat box for users
function showChatDialog() {
    chatBox.css("display", "block");
}

function Init() {
    setTimeout(showChatDialog, 1000);

    //Get data from form 
    var NewMessageForm = $("#NewMessageForm");
    NewMessageForm.on("submit", function (e) {

        e.preventDefault();
        var message = e.target[0].value;
        e.target[0].value = '';   //Clear textbox
        sendMessage(message);
    });
}

function sendMessage(text) {
    connection.invoke('SendNewMessage', " Sender ... ", text);
}

//Get msg from server
connection.on("getNewMessage", getMessage);

function getMessage(sender, message, time) {
    //in index.cshtm ui is commented
    $("#Messages").append("<li><div><span class='name'>" + sender + "</span><span class='time'>" + time + "</span></div><div class='message'>" + message + "</div></li>")
};

$(document).ready(function () {
    Init();
});
