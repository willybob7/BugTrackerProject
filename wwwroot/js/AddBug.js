$(document).ready(function () {


    function firstWidthDetect() {
        if (window.innerWidth >= 768) {
            $("#menu-toggle").html("<i id ='projectMenu' class='fas fa-less-than'></i>")
        } else if (window.innerWidth < 768) {
            $("#menu-toggle").html("<i id ='projectMenu' class='fas fa-greater-than'></i>")
            $("#wrapper").toggleClass("toggled");
        }
    }

    firstWidthDetect();

    function toggleWrapperOnWidthChange() {
        if (window.innerWidth >= 768 && $("#wrapper").hasClass("toggled")) {
            $("#wrapper").toggleClass("toggled");
        } else if (window.innerWidth < 768 && !$("#wrapper").hasClass("toggled")) {
            $("#wrapper").toggleClass("toggled");
        }
    }

    function toggleArrow() {
        if ($("#wrapper").hasClass("toggled")) {
            $("#menu-toggle").html("<i id ='projectMenu' class='fas fa-greater-than'></i>")
        } else {
            $("#menu-toggle").html("<i id='projectMenu' class='fas fa-less-than'></i>")
        }
    }

    function sizeChangeFunctions() {
        toggleWrapperOnWidthChange();
        toggleArrow();
    }

    window.onresize = sizeChangeFunctions;


    $("#menu-toggle").click(function (e) {
        e.preventDefault();
        $("#wrapper").toggleClass("toggled");
        if ($("#wrapper").hasClass("toggled")) {
            $("#menu-toggle").html("<i id='projectMenu' class='fas fa-greater-than'></i>")
        } else {
            $("#menu-toggle").html("<i id ='projectMenu' class='fas fa-less-than'></i>")
        }
    });

    $(".custom-file-input").on("change", function () {
        //var fileName = $(this).val().split("\\").pop();
        //$(this).next(".custom-file-label").html(fileName);
        var fileLabel = $(this).next(".custom-file-label");
        var files = $(this)[0].files;
        if (files.length > 1) {
            fileLabel.html(files.length + " files selected");
        } else if (files.length == 1) {
            fileLabel.html(files[0].name);
        }
    })


    var bugSeveritySelect = document.getElementById("bugSeverity")
    bugSeveritySelect.onchange = bugSeveritySelectBackground

    var bugStatusSelect = document.getElementById("bugStatus")
    bugStatusSelect.onchange = bugStatusSelectBackground

    function bugSeveritySelectBackground() {
        var bugSeveritySelectValue = bugSeveritySelect.value
        switch (bugSeveritySelectValue) {
            case "0":
                bugSeveritySelect.style.background = "#b81c04"
                bugSeveritySelect.style.color = "black"
                break;
            case "1":
                bugSeveritySelect.style.background = "#d67404"
                bugSeveritySelect.style.color = "black"
                break;
            default:
                bugSeveritySelect.style.background = "#ffee00"
                bugSeveritySelect.style.color = "black"

        }
    }

    function bugStatusSelectBackground() {
        var bugStatusSelectValue = bugStatusSelect.value
        switch (bugStatusSelectValue) {
            case "0":
                bugStatusSelect.style.background = "#375a7f"
                bugStatusSelect.style.color = "black"
                break;
            case "1":
                bugStatusSelect.style.background = "#3498DB"
                bugStatusSelect.style.color = "black"
                break;
            default:
                bugStatusSelect.style.background = "#00bc8c"
                bugStatusSelect.style.color = "black"
        }
    }
    bugSeveritySelectBackground();
    bugStatusSelectBackground();



});