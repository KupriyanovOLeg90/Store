$('a.delete').on('click', function () {
    if (!confirm("Вы действительно хотите удалить?"))
        return false;
});

//Sorting script
$("table#pages tbody").sortable({
    items: "tr:not(.home)",
    placeholder: "ui-state-highlight",
    update: function () {
        var ids = $('table#pages tbody').sortable('serialize');
        var url = "/Admin/Pages/ReorderPages";

        $.post(url, ids, function (data) { })
    }
});