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

    var deleteUser = document.getElementsByClassName("deleteUserMaybe");

    for (let i = 0; i < deleteUser.length; i++) {
        deleteUser[i].addEventListener("click", function (e) {
            e.preventDefault();
            $(this).parent().hide()
            $(this).parent().prev().show()
        })
    }

    var deleteUserNope = document.getElementsByClassName("deleteUserNope")

    for (let i = 0; i < deleteUserNope.length; i++) {
        deleteUserNope[i].addEventListener("click", function (e) {
            e.preventDefault();
            $(this).parent().hide();
            $(this).parent().next().show();
        })
    }

    var deleteUserYep = document.getElementsByClassName("deleteUserYep")

    for (let i = 0; i < deleteUserYep.length; i++) {
        deleteUserYep[i].addEventListener("click", function (e) {

            var formAction = e.target.getAttribute("data-url");
            var userId = e.target.getAttribute("data-userId");
            var projectId = e.target.getAttribute("data-projectId");
            var fdata = new FormData();
            fdata.append("userId", userId);
            fdata.append("projectId", projectId)

            $.ajax({
                type: 'post',
                url: formAction,
                data: fdata,
                processData: false,
                contentType: false,
            }).done(function (result) {
                console.log(result.status)
                var userDiv = document.getElementById(result.userDiv);
                //console.log(userDiv);
                userDiv.parentNode.removeChild(userDiv);



                //now remove the user from the dom


            })



        })
    }








    //function end
});