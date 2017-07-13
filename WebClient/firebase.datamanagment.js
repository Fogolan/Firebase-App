 function initializeFirebase() {
 // Initialize Firebase
    var config = {
        apiKey: "AIzaSyCTi9F3o1gl6ytvOzOqp20vjXX7omDstGU",
        authDomain: "win10-and-web-app.firebaseapp.com",
        databaseURL: "https://win10-and-web-app.firebaseio.com",
        projectId: "win10-and-web-app",
        storageBucket: "win10-and-web-app.appspot.com",
        messagingSenderId: "869443544420"
    };
    firebase.initializeApp(config);
    return firebase.database();
 }
    function jsonToList(data, el) {
        clearList(el);
        $.each(data, function (key, val) {
            $(el).append($('<li>').text(val.Field1 + ' ' + val.Field2 + ' ' + val.Field3));
        });
    }

    function ListToJson(el, separator) {
        items = [];
        $(el).children().each(function () {
            var $this = $(this);
            var objectFields = $this.html().split(separator);
            var item = { Field1: objectFields[0], Field2: objectFields[1], Field3: objectFields[2] };
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
        database.ref().set(data);
    }