// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {

    var bugStatusArr = document.getElementById("bug-status").dataset.bugstatus.split(" ");
    var openBugStatusArr = bugStatusArr.filter(a => a == "open");
    var closedBugStatusArr = bugStatusArr.filter(a => a == "closed");
    var toBeTestedBugStatusArr = bugStatusArr.filter(a => a == "to_be_tested");
    var bugSeverityArr = document.getElementById("bug-severity").dataset.bugseverity.split(" ");
    var criticalBugStatusArr = bugSeverityArr.filter(a => a == "critical");
    var majorBugStatusArr = bugSeverityArr.filter(a => a == "major");
    var minorBugStatusArr = bugSeverityArr.filter(a => a == "minor");

    var config1 = {
        type: 'doughnut',
        data: {
            labels: ["Open", "To Be Tested", "Closed"],
            datasets: [{
                data: [openBugStatusArr.length, toBeTestedBugStatusArr.length, closedBugStatusArr.length],
                backgroundColor: ['#375a7f', '#3498DB', '#00bc8c'],
                hoverBackgroundColor: ['#12457a', '#2883bf', '#056b51'],
                //hoverBorderColor: "rgba(234, 236, 244, 1)",
                borderColor: ['#375a7f', '#3498DB', '#00bc8c'],
                borderAlign: "inner",
                borderWidth: 4,
                label: "bugStatus"
            }],
        },
        options: {
            maintainAspectRatio: true,
            tooltips: {
                backgroundColor: "#000000",
                bodyFontColor: "#3498DB",
                borderColor: '#545454',
                borderWidth: 3,
                xPadding: 15,
                yPadding: 15,
                displayColors: true,
                caretPadding: 10,
            },
            legend: {
                display: true,
                position: "right",
                labels: {
                    fontSize: 16,
                    fontColor: "#3498DB"
                }
            },
            cutoutPercentage: 60,
            animation: {
                animateScale: true,
                animateRotate: true
            }
        }
    }

    var config2 = {
        type: 'doughnut',
        data: {
            labels: ["Critical", "Major", "Minor"],
            datasets: [{
                data: [criticalBugStatusArr.length, majorBugStatusArr.length, minorBugStatusArr.length],
                backgroundColor: ['#b81c04', '#d67404', '#d0db00'],
                hoverBackgroundColor: ['#6b1103', '#70560d', '#798000'],
                //hoverBorderColor: "rgba(234, 236, 244, 1)",
                borderColor: ['#b81c04', '#d67404', '#d0db00'],
                borderAlign: "inner",
                borderWidth: 4,
                label: "bugSeverity"
            }],
        },
        options: {
            maintainAspectRatio: true,
            tooltips: {
                backgroundColor: "#000000",
                bodyFontColor: "#3498DB",
                borderColor: '#545454',
                borderWidth: 3,
                xPadding: 15,
                yPadding: 15,
                displayColors: true,
                caretPadding: 10,
            },
            legend: {
                display: true,
                position: "right",
                labels: {
                    fontSize: 16,
                    fontColor: "#3498DB"
                }
            },
            cutoutPercentage: 60,
            animation: {
                animateScale: true,
                animateRotate: true
            }
        },
    }
    



    function firstWidthDetect() {
        if (window.innerWidth >= 768) {
            $("#menu-toggle").html("<i id ='projectMenu' class='fas fa-less-than'></i>")
        } else if (window.innerWidth < 768) {
            $("#menu-toggle").html("<i id ='projectMenu' class='fas fa-greater-than'></i>")
            $("#wrapper").toggleClass("toggled");
            config1.options.legend.display = false;
            config2.options.legend.display = false;
        }
    }

    firstWidthDetect();

    var ctx1 = document.getElementById("bugStatusChart");
    var myPieChart1 = new Chart(ctx1, config1);

    var ctx2 = document.getElementById("bugSeverityChart");
    var myPieChart2 = new Chart(ctx2, config2);

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

    function toggleLegendDisplayOnWidthChange() {
        if (window.innerWidth >= 768 && config1.options.legend.display == false) {
            config1.options.legend.display = true;
            config2.options.legend.display = true;
            window.myPieChart1.update();
            window.myPieChart2.update();
        } else if (window.innerWidth < 768 && config1.options.legend.display == true) {
            config1.options.legend.display = false;
            config2.options.legend.display = false
            window.myPieChart1.update();
            window.myPieChart2.update();
        }
    }



    function sizeChangeFunctions() {
        toggleWrapperOnWidthChange();
        toggleArrow();
        toggleLegendDisplayOnWidthChange();
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

    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })


    var projectStatusSelect = document.getElementById("projectStatus")
    projectStatusSelect.onchange = projectStatusSelectBackground

    function projectStatusSelectBackground() {
        var projectStatusSelectValue = projectStatusSelect.value
        switch (projectStatusSelectValue) {
            case "0":
                projectStatusSelect.style.color = "black"
                projectStatusSelect.style.background = "#3498DB"
                break;
            case "1":
                projectStatusSelect.style.color = "black"
                projectStatusSelect.style.background = "#3498DB"
                break;
            case "2":
                projectStatusSelect.style.color = "black"
                projectStatusSelect.style.background = "#E74C3C"
                break;
            case "3":
                projectStatusSelect.style.color = "black"
                projectStatusSelect.style.background = "#F39C12"
                break;
            case "4":
                projectStatusSelect.style.color = "#3498DB"
                projectStatusSelect.style.background = "#444"
                break;
            case "5":
                projectStatusSelect.style.color = "#3498DB"
                projectStatusSelect.style.background = "#303030"
                break;
            case "6":
                projectStatusSelect.style.color = "black"
                projectStatusSelect.style.background = "#999"
                break;
            case "7":
                projectStatusSelect.style.color = "black"
                projectStatusSelect.style.background = "#375a7f"
                break;
            default:
                projectStatusSelect.style.background = "#303030"
                projectStatusSelect.style.color = "#3498DB"
        }
    }

    projectStatusSelectBackground();



    var deleteButton = document.getElementById("deleteButton")
    deleteButton.onclick = function () {
        document.getElementById("deleteAlert").style.display = "block";
    }
    var closeAlert = document.getElementById("deleteAlertClose")
    closeAlert.onclick = function () {
        document.getElementById("deleteAlert").style.display = "none";
       
    }
    var deletePhraseInput = document.getElementById("deletePhrase")
    deletePhraseInput.oninput = function (e) {
        var projectName = document.getElementById("projectName").value
        if (e.target.value == projectName) {
            document.getElementById("delete").style.display = "block";
        } else {
            document.getElementById("delete").style.display = "none";

        }
    }

    


    

});
