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


    var imageTypeArr = ["tiff", "pjp", "pjpeg", "jfif", "tif",
        "svg", "bmp", "png", "jpeg", "svgz", "jpg", "webp", "ico", "xbm", "dib"];

    function validFileType(file) {
        var fileType = file.type.split("/")[1];

        if (file) {

            var i = 0;
            var length = imageTypeArr.length;
            var answer = false;

            while (i < length) {
                if (fileType === imageTypeArr[i]) {
                    answer = true;
                    return answer;
                }
                i++;
            }
            return answer;
        }
    }

    function dataURItoBlob(dataURI) {
        'use strict'
        var byteString,
            mimestring

        if (dataURI.split(',')[0].indexOf('base64') !== -1) {
            byteString = atob(dataURI.split(',')[1])
        } else {
            byteString = decodeURI(dataURI.split(',')[1])
        }

        mimestring = dataURI.split(',')[0].split(':')[1].split(';')[0]

        var content = new Array();
        for (var i = 0; i < byteString.length; i++) {
            content[i] = byteString.charCodeAt(i)
        }

        return new Blob([new Uint8Array(content)], { type: mimestring });
    }

    function clearScreenShots() {
        preview.innerHTML = "";
        filesToUpload = [];
        screenShotInput.value = "";
        document.querySelector(".fileUploadLabel").innerHTML = "Choose Screenshots...";
        clearPictures.style.display = "none";

    }


    var filesToUpload = [];
    var screenShotInput = document.getElementById("screenShotInput");
    let preview = document.querySelector(".preview");
    let inputMessage = document.querySelector(".inputMessage");
    var clearPictures = document.querySelector(".clearPictures");
    clearPictures.onclick = clearScreenShots;


    



    function processScreenShot(e) {
        var files = e.target.files;

        console.log(files)

        console.log(filesToUpload)

        var allImages = true;

        for (let i = 0; i < files.length; i++) {
            if (validFileType(files[i]) == false) {
                allImages = false;
                break;
            }
        }


        if (allImages == true) {

            var j = 0;

            for (var i = 0; i < files.length; i++) {

                fileName = files[i].name
             
                loadImage(files[i], {
                    maxWidth: 1900,
                    maxHeight: 1080,
                    pixelRatio: window.devicePixelRatio,
                    downsamplingRatio: 0.5,
                    orientation: true,
                    canvas: true,
                    imageSmoothingQuality: "medium"
                }).then(function (data) {

                    preview.innerHtml = "";
                    preview.append(data.image);
                    data.image.className = "imagePreview";

                    var dataURL = data.image.toDataURL('image/jpeg', 1);
                    var blob = dataURItoBlob(dataURL);
                    filesToUpload.push(new File([blob], files[j].name));

                    j++;

                })
            }

            clearPictures.style.display = "flex";
           
        } else {
            screenShotInput.files = null;
            inputMessage.innerHTML = "Please select an image files"
        }

    }

    screenShotInput.onchange = processScreenShot


    $('#addBugFormSubmit').click(function () {

    var Data = new FormData();
    var action = document.getElementById("StoreInitialScreenShots")
        .getAttribute("data-url")


    if (filesToUpload.length > 0) {
        for (let i = 0; i < filesToUpload.length; i++) {
            var FileData = filesToUpload[i];
            Data.append("Attachments", FileData);
        }
    }

    $.ajax({
        type: 'post',
        url: action,
        data: Data,
        processData: false,
        contentType: false,
    }).done(function (result) {
        if (result.status === "success") {
            $("#addBugForm").submit();
        } else if (result.status === "fileTooLarge") {
            clearScreenShots()
            preview.innerHTML = result.message;
        } else if (result.status === "fileNotImage") {
            clearScreenShots();
            preview.innerHTML = result.message;
        }
    });



});



});