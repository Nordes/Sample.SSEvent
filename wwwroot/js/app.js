
// subscribe for messages
var source = new EventSource('/events');

// handle messages if not handled by the "addEventListener" (see below)
source.onmessage = function(event) {
	// Do something with the data:
    console.log(event); // event.data;
};

source.onopen = function (event) {
	// Happens when receiving the first message.
    console.log('onopen');
};

source.onerror = function (event) {
	// Happens when server close or issues.
    console.log('onerror');
}

source.addEventListener("snackbar", function(event) {
	// do stuff with data
	console.log(event.data);
	handleSnackNotification(JSON.parse(event.data));
});

function handleSnackNotification(notif) {
	// Get the snackbar DIV
	var x = document.getElementById("snackbar");

	x.innerText = notif.message;
	// Add the "show" class to DIV
	x.className = "show";

	// After 3 seconds, remove the show class from DIV
	setTimeout(function(){ x.className = x.className.replace("show", ""); }, 2700);
}

var broadcastMessageBox = document.getElementById("broadMessage");
function handleBroadcast() {
	var msg = broadcastMessageBox.value;
	var xhr = new XMLHttpRequest();
		
	xhr.open("POST", "/events/broadcast", true);
	xhr.setRequestHeader("Content-Type", "application/json");
	var notification = {
	    'id': 1,
	    'message': msg,
		'level': 1
	};

	xhr.send(JSON.stringify(notification));
    broadcastMessageBox.value = "";
}

broadcastMessageBox.focus();

// Execute a function when the user releases a key on the keyboard
broadcastMessageBox.addEventListener("keyup", function (event) {
    // Number 13 is the "Enter" key on the keyboard
    if (event.keyCode === 13) {
        // Cancel the default action, if needed
        event.preventDefault();
        // Trigger the button element with a click
		document.getElementById("btnBroadcast").click();
    }
});