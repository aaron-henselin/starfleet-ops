// Write your JavaScript code.

$(document).on("click",
    "[data-pawn-input-role='take-damage']",
    function () {
        var $form = $(this).closest("form");

        kendo.ui.progress($form, true);

        $form.find("[name='BrowserGameState']").val(localStorage.getItem('gs'));

        var data = $form.serialize();
        $.ajax({
            type: 'POST',
            url: $form.attr("action"),
            data: data,
            success: function (data) {
                var $parent = $form.parent();
                
                $parent.html(data);
                var newState = $parent.find("[name='PostActionGameState']").val();
                localStorage.setItem('gs', newState);
            }
        });

    });


$(document).ready(function () {
    $("#panelbar").kendoPanelBar({
        expandMode: "single"
    });
});
