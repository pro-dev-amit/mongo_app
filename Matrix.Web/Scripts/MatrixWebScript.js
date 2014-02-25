

function getContent(targetDivId, url, isPanelOpen) {
    var targetDiv = "#" + targetDivId; //vendor-history";
    $(targetDiv).html('<p><img id="ajax-loader" src="@Url.Content("~/Content/themes/base/images/wait.gif")" align="left">Loading...</p>');

    $.get(url, function (result) {
        $(targetDiv).html(result);
        //if (isPanelOpen == "True") {
        //    OpenAndScrollControl(targetDivId);
        //}
    });
}

function getAutoComplete(actionUrl , autoCompTextBoxId, autoCompHiddenCtrlId) {
    $("#" + autoCompTextBoxId).live('input', function () {
        var content = $("#" + autoCompTextBoxId).val();
        if (content == "") {
            $("#" + autoCompHiddenCtrlId).val(null);
        }
    })

    $(function () {
        $("#" + autoCompTextBoxId).autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: actionUrl, type: "GET", dataType: "json",
                    data: { term: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return { label: item.Text, value: item.Value, id: item.Value };
                        }))
                    }
                })
            },

            focus: function (event, ui) {
                $("#" + autoCompTextBoxId).val(ui.item.label);
                return false;
            },

            minLength: 3,
            autofill: true,
            select: function (event, ui) {
                $("#" + autoCompTextBoxId + "\"").val(ui.item.label);
                $("#" + autoCompHiddenCtrlId).val(ui.item.value);
                event.preventDefault();
            }

        });

    });
}