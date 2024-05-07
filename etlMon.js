if (navigator.userAgent.indexOf('iPhone') != -1) {
    addEventListener("load", function() {
        setTimeout(hideURLbar, 0);
    }, false);
}

function hideURLbar() {
    window.scrollTo(0, 1);
}

function hourglass() {
    document.body.style.cursor = "wait";
}

