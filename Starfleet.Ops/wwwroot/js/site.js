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
    $(".panelbar").kendoPanelBar({
        expandMode: "single"
    });
});

   
        $(document).ready(function() {

            var runStrategicView = $(".strategic-view").length > 0;
            if (!runStrategicView)
                return;

            $("[data-rename-for]").each(function () {
                $(this).click(function() {

                    var id = $(this).data("rename-for");
                    $("[data-rename='" + id + "']").slideDown();

                });

            });


            var refreshFleetSelections = function() {


                var l = $("[name=includeInBattle]:checked").length;
                $(".fleet-select-count").text(l + " ships selected.");

                $(".start-battle-button").removeAttr("disabled");
                if (l === 0)
                    $(".start-battle-button").attr("disabled", "disabled");
            };

            $(".enable-fleet-selection").click(function() {
                $(".enable-fleet-selection").hide();
                $(".fleet-selector").slideDown();
                $(".start-battle-prompt").slideDown();
                refreshFleetSelections();
            });

            $("[name=includeInBattle]").click(function() {
                refreshFleetSelections();
            });

            $(".start-battle-button").click(function() {
                var act = $(".start-battle-form form").attr("action");

                var fleetIds = "?";
                $("[name=includeInBattle]:checked").each(function () {
                    var addVal = $(this).val();
                   fleetIds=fleetIds + "&fleets=" + addVal;
                });

                document.location.href = act + fleetIds;
            });

            var draggableOnDragStart = function(e) {
                $(e.element).addClass("dragging");
                $(".droptarget").addClass("droptarget-highlight");
            }

            var draggableOnDragEnd = function(e) {
                $(e.element).removeClass("dragging");
                $(".droptarget").removeClass("droptarget-highlight");
            }

            var droptargetOnDrop = function(e) {
                var shipId = $(e.draggable.element).attr("data-pawn-id");
                var fleetId = $(e.dropTarget).closest("[data-fleet-id]").attr("data-fleet-id");

                $submitForm = $(".assign-to-fleet-form form");
                $submitForm.find("[name='shipId']").val(shipId);
                $submitForm.find("[name='fleetId']").val(fleetId);
                $submitForm.submit();
                //<input required type="text" name="fleetId"/>
                //<input required type="text" name="shipId"/>

                //$(".droptarget").removeClass("droptarget-highlight");
            }

            $(".draggable").kendoDraggable({
                hint: function () {
                    var $el = $(this.element);
                    var $clone = $el.clone();
                    $clone.width($el.width());
                    return $clone;
                },
                dragstart: draggableOnDragStart,
                dragend: draggableOnDragEnd
            });

            $(".droptarget").kendoDropTarget({
                //dragenter: droptargetOnDragEnter,
                //dragleave: droptargetOnDragLeave,
                drop: droptargetOnDrop
            
            });
        });

        
