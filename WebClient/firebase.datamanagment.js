function initializeFirebase() {
    // Initialize Firebase this config you can find it here https://console.firebase.google.com/project/win10-and-web-app/authentication/users if you click "Web Setup"
    var config = {
        apiKey: "AIzaSyCTi9F3o1gl6ytvOzOqp20vjXX7omDstGU",
        authDomain: "win10-and-web-app.firebaseapp.com",
        databaseURL: "https://win10-and-web-app.firebaseio.com",
        projectId: "win10-and-web-app",
        storageBucket: "win10-and-web-app.appspot.com",
        messagingSenderId: "869443544420"
    };
    firebase.initializeApp(config); //Initializing Firebase application
    return firebase.database();
}
function jsonToList(data, el) {
    var difference = getDiffernceFromFirebase(ListToJson(el, ' '), data);

    clearList(el);
    $.each(data, function (key, val) {
        $(el).append($('<li>').text(val.Field1 + ' ' + val.Field2 + ' ' + val.Field3).attr('id', val.Id));
    });

    animateDiffRows(difference);
}

function getDiffernceFromFirebase(currentData, newData) { //You need to know, what rows have changed
    var difference = [];
    currentData.forEach(function (item, i, arr) {
        if (item.Id != newData[i].Id) {
            difference.push(newData[i].Id);
        }
    });
    return difference;
}

function animateDiffRows(differenceArray) {
    differenceArray.forEach(function (item, i, arr) {
        $("#" + item).animate({
            backgroundColor: "#aa0000",
            color: "#fff",
        }, 1000);
        $("#" + item).animate({
            backgroundColor: "#fff",
            color: "#000",
        }, 1000);
    });
}

function ListToJson(el, separator) {
    items = [];
    $(el).children().each(function () {
        var $this = $(this);
        var objectFields = $this.html().split(separator);
        var item = { Id: $this.attr('id'), Field1: objectFields[0], Field2: objectFields[1], Field3: objectFields[2] };
        items.push(item);
    });
    return items;
}

function clearList(el) {
    $.each(data, function (key, val) {
        $(el + ' li').remove();
    });
}

function setToDatabase(database, data) {
    database.ref().set(data); //send data to Firebase 
}