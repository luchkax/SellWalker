$("#myHref").on('click', function(event) {
    // document.getElementById(".reviewInput").style.visibility = "visible";
    var x = document.getElementById("reviewInput");
    if (x.style.display === "none") {
        x.style.display = "block";
    } else {
        x.style.display = "none";
    }
});