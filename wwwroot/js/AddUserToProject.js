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

    var input = document.getElementById("userSearch");
    input.oninput = function (e) {

        input = e.target.value;

        if (input.length > 4) {
            var formAction = e.target.parentNode.getAttribute("action")
            var fdata = new FormData();
            fdata.append("input", input);
            var projectId = document.getElementById("projectId").value;
            fdata.append("projectId", projectId);
           
            $.ajax({
                type: 'post',
                url: formAction,
                data: fdata,
                processData: false,
                contentType: false,
            }).done(function (result) {
                console.log(result.status)

                var form = document.getElementById("usersToAdd");
                form.innerHTML = "";
                
                var addUsersButton = document.getElementById("addUsers");
                addUsersButton.style.display = "inline-block";

                for (let i = 0; i < result.users.length; i++) {

                    var html =
                        `<div class="card mb-3 mt-1">
                                <div class="card-body">
                                    <p class="card-title ml-3"><input type="checkbox"
                                        data-userId="${result.users[i].id}" 
                                        class="addUserInput form-check-input"
                                        style="align-self:center">Add </input>
                                        User: ${result.users[i].userName}
                                    </p>
                                </div>
                            </div>`
                    form.innerHTML += html;
                }

                var addUsersSubmit = document.getElementById("addUsers");
                addUsersSubmit.addEventListener("click", function (e) {
                    var usersList = e.target.parentNode.getElementsByClassName("addUserInput");
                    var addedUsers = "";
                    for (let i = 0; i < usersList.length; i++) {
                        if (usersList[i].checked == true) {
                            if (addedUsers.length == 0) {
                                addedUsers += usersList[i].getAttribute("data-userId")
                            } else {
                                addedUsers += " " + usersList[i].getAttribute("data-userId")
                            }
                        }
                    }

                    let formAction = e.target.parentNode.getAttribute("action");
                    var fdata = new FormData();
                    fdata.append("users", addedUsers);
                    var projectId = document.getElementById("projectId").value;
                    fdata.append("projectId", projectId);

                    $.ajax({
                        type: 'post',
                        url: formAction,
                        data: fdata,
                        processData: false,
                        contentType: false,
                    }).done(function (result) {
                        console.log(result.status)

                        var usersList = e.target.parentNode.getElementsByClassName("addUserInput");

                        for (let i = 0; i < usersList.length; i++) {
                            if (usersList[i].checked == true) {
                                console.log(usersList[i].parentNode.parentNode.parentNode)
                                var nodeToRemove = usersList[i].parentNode.parentNode.parentNode
                                nodeToRemove.parentNode.removeChild(nodeToRemove);
                                i--;
                            }
                        }

                        var usersHaveBeenAddedDiv = document.getElementById("UsersHaveBeenAdded");
                        usersHaveBeenAddedDiv.style.display = "block"

                    })


                })


            })


        }
    }
});
                           
