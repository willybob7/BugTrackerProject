// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
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

    var bugSeveritySelect = document.getElementById("bugSeverity")
    bugSeveritySelect.onchange = bugSeveritySelectBackground

    var bugStatusSelect = document.getElementById("bugStatus")
    bugStatusSelect.onchange = bugStatusSelectBackground

    function bugSeveritySelectBackground() {
        var bugSeveritySelectValue = bugSeveritySelect.value
        switch (bugSeveritySelectValue) {
            case "0":
                bugSeveritySelect.style.background = "#b81c04"
                break;
            case "1":
                bugSeveritySelect.style.background = "#d67404"
                break;
            default:
                bugSeveritySelect.style.background = "#ffee00"
        }
    }

    function bugStatusSelectBackground() {
        var bugStatusSelectValue = bugStatusSelect.value
        switch (bugStatusSelectValue) {
            case "0":
                bugStatusSelect.style.background = "#375a7f"
                break;
            case "1":
                bugStatusSelect.style.background = "#3498DB"
                break;
            default:
                bugStatusSelect.style.background = "#00bc8c"
        }
    }

    bugStatusSelectBackground()
    bugSeveritySelectBackground()

    var childElements = document.getElementById("links")
    var deleteLinks = document.getElementById("deleteScreenshot").getElementsByTagName("span");

    var screenshotSrc = [];
    for (let i = 0; i < childElements.childNodes.length; i++) {
        if (childElements.childNodes[i].nodeName == "IMG") {
            screenshotSrc.push(childElements.childNodes[i].src)
        }
    }

    while (childElements.firstChild) {
        childElements.removeChild(childElements.firstChild);
    }
    var deleteLinkDiv = document.getElementById("deleteScreenshot");
    deleteLinkDiv.parentNode.removeChild(deleteLinkDiv);


    if (deleteLinks.length == 0) {
        screenshotSrc.forEach((s, i) => {
            childElements.insertAdjacentHTML("beforeend",
                `<a class="screenshotdiv" href="${s}" >
                     <img class="screenshot" src="${s}" />
                </a>`
            )
        });

    } else {
        screenshotSrc.forEach((s, i) => {
            childElements.insertAdjacentHTML("beforeend",
                `<a class="screenshotdiv" href="${s}" >
                    <img class="screenshot" src="${s}" />
                    ${deleteLinks[i].outerHTML}
                </a>`
            )
        });
    }

    

    var deleteScreenShotSpan = document.getElementsByClassName("deleteScreenShot")
    for (let i = 0; i < deleteScreenShotSpan.length; i++) {
        deleteScreenShotSpan[i].addEventListener("click", function (e) {
            e.preventDefault();
            var action;
            var spanNode;
            if (e.target.tagName == "I") {
                action = e.target.parentNode.getAttribute("data-deleteAction")
                spanNode = e.target.parentNode
            } else {
                action = e.target.getAttribute("data-deleteAction")
                spanNode = e.target
            }

            $.ajax({
                type: "post",
                url: action,
            }).done(function (result) {
                if (result.status == "success") {
                    var screenShotToRemove = spanNode.parentNode;
                    screenShotToRemove.parentNode.removeChild(screenShotToRemove);
                }

            })

        })
    }



    document.getElementById('links').onclick = function (event) {
        event = event || window.event
        if (event.target.tagName == "IMG") {
            var target = event.target || event.srcElement,
                link = target.src ? target.parentNode : target,
                options = { index: link, event: event },
                links = this.getElementsByTagName('a')

            blueimp.Gallery(links, options)
        }
    }


    var deleteButton = document.getElementById("deleteButton")
    if (deleteButton != null) {
        deleteButton.onclick = function () {
            document.getElementById("deleteAlert").style.display = "block";
        }
    }
   
    var closeAlert = document.getElementById("deleteAlertClose")
    if (closeAlert != null) {
        closeAlert.onclick = function () {
            document.getElementById("deleteAlert").style.display = "none";

        }
    }
    
    var deletePhraseInput = document.getElementById("deletePhrase")
    if (deletePhraseInput != null) {
        deletePhraseInput.oninput = function (e) {
            var bugName = document.getElementById("bugName").value
            if (e.target.value == bugName) {
                document.getElementById("delete").style.display = "block";
            } else {
                document.getElementById("delete").style.display = "none";

            }
        }
    }
    

    document.getElementById("summernote").innerHTML = "";
    $('#summernote').summernote();

    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    document.getElementById("sendButton").disabled = true;

    connection.start().then(function () {
        console.log("connection started");
        document.getElementById("sendButton").disabled = false;

    }).catch(function (err) {
        return console.error(err.toString());
    });


    connection.on("ReceiveMessage", function (user, message) {

        message = message.split("%%%");
        comment = message[0];
        createdDate = message[1];
        userId = message[2];
        commentId = message[3];
        actionUrl = message[4];

        addComment(comment, createdDate, userId, commentId, actionUrl)
    });


    $("#commentForm").submit(function (e) {
        e.preventDefault();

        var formAction = $(this).attr("action");
        var fdata = new FormData();
        var markupStr = $('#summernote').summernote('code');
        var userId = document.getElementById("currentUserName").value;
        var associatedProject = document.getElementById("associatedProjectId").value;
        var bugId = document.getElementById("bugId").value;
        fdata.append("comment", markupStr);
        fdata.append("userId", userId);
        fdata.append("associatedProject", associatedProject);
        fdata.append("associatedBug", bugId);
        $.ajax({
            type: 'post',
            url: formAction,
            data: fdata,
            processData: false,
            contentType: false,
        }).done(function (result) {
            if (result.status === "success") {
                //addComment(result.comment, result.createdDate, result.userId, result.commentId, result.actionUrl)
                //Im using %%% as a delimiter so I can run the addComment only once, not twice
                var message = result.comment + "%%%" + result.createdDate + "%%%" + result.userId + "%%%"
                    + result.commentId + "%%%" + result.actionUrl;
                connection.invoke("SendMessage", userId, message).catch(function (err) {
                    return console.error(err.toString());
                });
            //event.preventDefault();
                $('#summernote').summernote('reset');
            } else {
                console.log(result.message);
            }
        });
    });

    function formatDate(date) {
        let diff = new Date() - date; // the difference in milliseconds

        if (diff < 1000) { // less than 1 second
            return 'right now';
        }

        let sec = Math.floor(diff / 1000); // convert diff to seconds

        if (sec < 60) {
            return sec + ' sec. ago';
        }

        let min = Math.floor(diff / 60000); // convert diff to minutes
        if (min < 60) {
            return min + ' min. ago';
        }

        // format the date
        // add leading zeroes to single-digit day/month/hours/minutes
        let d = date;

        d = [
            '0' + (d.getMonth() + 1),
            '0' + d.getDate(),
            '' + d.getFullYear(),
            '0' + d.getHours(),
            '0' + d.getMinutes()
        ].map(component => component.slice(-2)); // take last 2 digits of every component

        var timeOfDay = "am"
        if (d[3] > 12) {
            d[3] = d[3] - 12
            timeOfDay = "pm"
        }

        // join the components into date
        return d.slice(0, 3).join('.') + ' ' + d.slice(3,5).join(':') + timeOfDay;
    }

   


    function addComment(comment, createdDate, userId, commentId, actionUrl) {
        var commentSection = document.getElementById("comments");

        if (managerLevel == "True") {
            commentSection.insertAdjacentHTML("beforeend",
                `<div class="comment" id="comment${commentId}">
                    <hr style="height:1px; border:none; color:rgb(52, 152, 219); background-color:rgb(52, 152, 219);">
                    <div>User: ${userId}</div>
                    <div id="comment${commentId}updateSection">${comment}</div>
                    <div class="justify-content-between" style="display:flex;"><p>${formatDate(new Date(createdDate))}</p>
                        <span>
                            <span id="comment${commentId}delete" style="cursor: pointer;">
                                <i data-deleteUrl="${actionUrl}" class="far fa-trash-alt" ></i>
                            </span>
                            <span id="comment${commentId}update" class="ml-3" style="cursor: pointer;">
                                <i class="fas fa-edit"></i>
                            </span>
                        </span>
                    </div>
                 </div>`)

            var updateIcon = document.getElementById(`comment${commentId}update`)
            updateIcon.addEventListener("click", editComment)


            var deleteIcon = document.getElementById(`comment${commentId}delete`)
            deleteIcon.addEventListener("click", deleteComment)

        } else {
            commentSection.insertAdjacentHTML("beforeend",
                `<div class="comment" id="comment${commentId}">
                    <hr style="height:1px; border:none; color:rgb(52, 152, 219); background-color:rgb(52, 152, 219);">
                    <div>User: ${userId}</div>
                    <div id="comment${commentId}updateSection">${comment}</div>
                    <div class="justify-content-between" style="display:flex;">
                        <p>${formatDate(new Date(createdDate))}</p>
                    </div>
                </div>`)

        }

        var newCommentSection = document.getElementById(`comment${commentId}`)
        var newCommentSectionImages = newCommentSection.getElementsByTagName("img");

        for (let i = 0; i < newCommentSectionImages.length; i++) {
            newCommentSectionImages[i].style.width = "100%";
        }
    }



    


    function commentSection() {
        
        var comments = document.getElementsByClassName("downloadedComment")

        for (let i = 0; i < comments.length; i++) {
            var commentInfo = comments[i].innerHTML
            var domParser = new DOMParser();
            addComment(
                domParser.parseFromString(commentInfo, "text/html").documentElement.textContent,
                comments[i].getAttribute("data-createdDate"),
                comments[i].getAttribute("data-userId"),
                comments[i].getAttribute("data-commentId"),
                comments[i].getAttribute("data-actionUrl"),
            )
        }
    }
    commentSection();


    function deleteComment(e) {
        var action = e.target.getAttribute("data-deleteUrl")
        var commentId = action.match(/\d+/)[0];
        $.ajax({
            type: 'post',
            url: action,
            processData: false,
            contentType: false,
        }).done(function (result) {
            if (result.status === "success") {
                var commentToRemove = document.getElementById("comment" + commentId);
                commentToRemove.parentNode.removeChild(commentToRemove)
            } else {
                alert(result.message);
            }
        });
    }

    function editComment(e) {
        var sectionSelector = "#" + e.target.parentNode.getAttribute("id") + "Section"
        $(sectionSelector).summernote({ focus: true });
        var commentId = sectionSelector.match(/(\d+)/)[0]; 

        var clearButton = document.getElementById(`save${commentId}`)
        if (clearButton != null) {
            clearButton.parentNode.removeChild(clearButton)
        }

        document.getElementById(e.target.parentNode.getAttribute("id") + "Section").insertAdjacentHTML("afterend",
            `<button id="save${commentId}" data-updateUrl="/bugcomment/updatecomment" class="btn btn-primary d-block ml-auto">Update Comment</button>`
        )
        document.getElementById(`save${commentId}`).addEventListener("click", function () {

            var fdata = new FormData();
            var markupStr = $(sectionSelector).summernote('code');
            var action = document.getElementById(`save${commentId}`).getAttribute("data-updateUrl")
            var userId = document.getElementById("currentUserName").value;
            var associatedProject = document.getElementById("associatedProjectId").value;
            var bugId = document.getElementById("bugId").value;
            fdata.append("comment", markupStr);
            fdata.append("userId", userId);
            fdata.append("associatedProject", associatedProject);
            fdata.append("associatedBug", bugId);
            fdata.append("commentId", commentId);

            $.ajax({
                type: 'post',
                url: action,
                data: fdata,
                processData: false,
                contentType: false,
            }).done(function (result) {
                if (result.status === "success") {
                    console.log("update worked")
                } else {
                    console.log(result.message);
                }
            });

            $(sectionSelector).summernote('destroy');
            var updateButton = document.getElementById(`save${commentId}`)
            updateButton.parentNode.removeChild(updateButton)

            var newCommentSection = document.getElementById(`comment${commentId}`)
            var newCommentSectionImages = newCommentSection.getElementsByTagName("img");

            for (let i = 0; i < newCommentSectionImages.length; i++) {
                newCommentSectionImages[i].style.width = "100%";
            }


        })
    }

});











