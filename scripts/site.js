
var reportserverUri = "blue32";
var crudServiceBaseUrl = "blue32";
var crudServiceBaseUrlHttp = "blue32";
var EmailDomainRestrictions = "";

var autoScrollCaro;
var verticalView = false;
var autoCloseCharm;
var refresh = true;
var selectedNode;
var selectedFlag;

Array.prototype.removeValue = function (x) {
    var i;
    for (i in this) {
        if (this[i].toString() === x.toString()) {
            this.splice(i, 1);
        }
    }
}

function filterLocations(box) {
    var filter = box.value.toLowerCase();
    var items = $("#LocationPicker").find(".tile");
    $.each(items,
        function (k, v) {
            var text = $(v).data("name").toLowerCase();
            if (text.indexOf(filter) === -1) {
                v.style.display = "none";
            } else {
                v.style.display = "block";
            }
        });
}

$.fn.hasAnyClass = function () {
    var classes = arguments[0].split(" ");
    for (var i = 0; i < classes.length; i++) {
        if (this.hasClass(classes[i])) {
            return true;
        }
    }
    return false;
}

$(document).on("click", ".charm", function (e) {
    //e.preventDefault();
    e.stopPropagation();
});
//
//$(document).on('click', function (e) {

//    var incharm = false;
//    var element = e.target;
//    console.log($(element));
//    while (element.parentNode) {
       
//        if ($(element).hasAnyClass("k-item k-link k-icon k-i-close") || $(element)["0"].tagName === "rect" || $(element)["0"].tagName === "path") {
//            incharm = true;
//        }
//        element = element.parentNode;

//    }
//    if (!incharm) {

//        $('[data-role=charm]').each(function (i, el) {
//            if (!$(el).hasClass('keep-open') && $(el).data('displayed') === true) {
//                $(el).data('charm').close();
//            }
//        });
//    }
//});



function processTable(data, idField, foreignKey, rootLevel) {
    var hash = {};

    for (var i = 0; i < data.length; i++) {
        var item = data[i];
        var id = item[idField];
        if (item[foreignKey] === null)  
        item[foreignKey] = -1;
        var parentId = item[foreignKey];
     
        hash[id] = hash[id] || [];
        hash[parentId] = hash[parentId] || [];

        item.items = hash[id];
        hash[parentId].push(item);
    }

    return hash[rootLevel];
}

function is_email(email) {
    var emailReg = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i;
    return emailReg.test(email);
} 



function stringToBoolean(s) {
    switch (s.toLowerCase().trim()) {
        case "true":
        case "yes":
        case "1":
            return true;
        case "false":
        case "no":
        case "0":
        case null:
            return false;
        default:
            return Boolean(string);
    }
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) === 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

// ReSharper disable once NativeTypePrototypeExtending
String.prototype.trunc =
    function (n, useWordBoundary) {
        var isTooLong = this.length > n,
            s = isTooLong ? this.substr(0, n - 1) : this;
        s = (useWordBoundary && isTooLong) ? s.substr(0, s.lastIndexOf(' ')) : s;
        return isTooLong ? s + '  ...' : s;
    };

function quoteAndEscape(str) {
    return '' +
        '&#39;' // open quote '
        +
        ('' + str) // force string
        .replace(/\\/g, '\\\\') // double \
        .replace(/"/g, '\\&quot;') // encode "
        .replace(/'/g, '\\&#39;') // encode '
        +
        '&#39;'; // close quote '
}


function stopScroll(element) {
    //var activeElement;

    //$(document).bind('mousewheel DOMMouseScroll', function (e) {
    //    var scrollTo = null;

    //    if (!$(activeElement).closest(".k-popup").length) {
    //        return;
    //    }

    //    if (e.type == 'mousewheel') {
    //        scrollTo = (e.originalEvent.wheelDelta * -1);
    //    }
    //    else if (e.type == 'DOMMouseScroll') {
    //        scrollTo = 40 * e.originalEvent.detail;
    //    }

    //    if (scrollTo) {
    //        e.preventDefault();
    //        element.scrollTop(scrollTo + element.scrollTop());
    //    }
    //});

    //$(document).on('mouseover', function (e) {
    //    activeElement = e.target;
    //});
}