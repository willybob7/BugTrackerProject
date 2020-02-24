// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

if (document.getElementById("projectStatus") != null) {
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
}

