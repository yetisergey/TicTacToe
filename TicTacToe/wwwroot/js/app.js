window.onload = function () {
    canvas = document.getElementById("squares");
    canvas.width = clientWidth;
    canvas.height = clientHeight;
    context = canvas.getContext("2d");
    drawFrame();
    canvas.addEventListener('click', function (ev) {
        hubConnection.invoke("Move", ev.offsetX, ev.offsetY);
    }, false);
    getUserName();
}

let hubUrl = window.location.origin + '/game';
const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(hubUrl)
    .configureLogging(signalR.LogLevel.Information)
    .build();

hubConnection.on("StartGame", function (data) {
    gameState = data;
    $("#game").show();
});

function getUserName() {
    $.ajax({
        type: "GET",
        url: "/api/Login",
        contentType: 'application/json',
        success: function (data) {
            if (data !== "") {
                $("#userName").val(data)
                hubConnection.start();
                $("#userName").prop('disabled', true);
                $("#startGame").prop('disabled', true);
            }
        },
        error: function (e) {
            console.log(e);
        }
    });
}

$("#startGame").click(function () {
    $.ajax({
        type: "POST",
        url: "/api/Login",
        contentType: 'application/json',
        data: JSON.stringify(document.getElementById("userName").value),
        success: function () {
            hubConnection.start();
            $("#userName").prop('disabled', true);
            $("#startGame").prop('disabled', true);
        },
        error: function (e) {
            console.log(e);
        }
    });
});

hubConnection.on("GetMove", function (data) {
    UpdateGameField(GameObject(data.abscissa, data.ordinate, data.state));
});

const WIDTHQUAD = 20;

var canvas;
var context;
var clientWidth = 500;
var clientHeight = 500;
var gameState;
var gameObjects = [new GameObject(0, 0, null)];

function GameObject(offsetX, offsetY, state) {
    return {
        offsetX: offsetX,
        offsetY: offsetY,
        state: state
    }
}

function AddFieldIfNotExists(offsetX, offsetY) {
    if (!IsGameObjectExists(offsetX, offsetY))
        gameObjects.push(new GameObject(offsetX, offsetY, null))
}

function IsGameObjectExists(offsetX, offsetY) {
    for (var i = 0; i < gameObjects.length; i++)
        if (gameObjects[i].offsetX == offsetX &&
            gameObjects[i].offsetY == offsetY) {
            return true;
        }
    return false;
}

function UpdateGameField(ev) {
    for (let i = 0; i < gameObjects.length; i++) {
        const element = gameObjects[i];
        if (element.offsetX <= ev.offsetX && element.offsetX + WIDTHQUAD >= ev.offsetX &&
            element.offsetY <= ev.offsetY && element.offsetY + WIDTHQUAD >= ev.offsetY) {
            gameObjects[i].state = ev.state;
            AddFieldIfNotExists(element.offsetX + WIDTHQUAD, element.offsetY + WIDTHQUAD, null);
            AddFieldIfNotExists(element.offsetX - WIDTHQUAD, element.offsetY + WIDTHQUAD, null);
            AddFieldIfNotExists(element.offsetX + WIDTHQUAD, element.offsetY - WIDTHQUAD, null);
            AddFieldIfNotExists(element.offsetX - WIDTHQUAD, element.offsetY - WIDTHQUAD, null);
            AddFieldIfNotExists(element.offsetX - WIDTHQUAD, element.offsetY, null);
            AddFieldIfNotExists(element.offsetX + WIDTHQUAD, element.offsetY, null);
            AddFieldIfNotExists(element.offsetX, element.offsetY - WIDTHQUAD, null);
            AddFieldIfNotExists(element.offsetX, element.offsetY + WIDTHQUAD, null);
            break;
        }
    }
    drawFrame();
}

function drawFrame() {
    context.clearRect(0, 0, canvas.width, canvas.height);
    for (let i = 0; i < gameObjects.length; i++) {
        const element = gameObjects[i];
        if (element.state == null)
            context.fillStyle = "#FFF";

        if (element.state == 0)
            context.fillStyle = "#00F";

        if (element.state == 1)
            context.fillStyle = "#F00";

        context.strokeRect(element.offsetX, element.offsetY, WIDTHQUAD, WIDTHQUAD);
        context.fillRect(element.offsetX, element.offsetY, WIDTHQUAD, WIDTHQUAD);
    }
}