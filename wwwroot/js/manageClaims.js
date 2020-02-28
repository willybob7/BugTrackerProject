var checkboxes = document.getElementsByClassName("claimCheckBox");
for (let i = 0; i < checkboxes.length; i++) {
    checkboxes[i].addEventListener("change", function (e) {
        var checkboxes = document.getElementsByClassName("claimCheckBox");
        for (let j = 0; j < checkboxes.length; j++) {
            if (checkboxes[j].name != e.target.name && e.target.checked == true) {
                checkboxes[j].checked = false;
            }
        }
    })
}