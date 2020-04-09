
//var gridName = '';
var validator;
var pdfExport = false;
//var gridTrigger;

$(document).ready(function () {

    //validator = $("#approvalsContainer").kendoValidator({
    //    messages: {
    //        //// Override built-in message for required report title rule.
    //        //requiredReportTitle: "Report Title must be selected.",

    //        //// Override built-in message for required report date rule.
    //        //requiredReportDate: "Report Date in MM/YYYY (Calendar Year) format must be entered.",

    //        //// Maximum date message.
    //        //maxDate: "Report Date month and year cannot be greater than current date month and year."
    //    },
    //    rules: {
    //        requiredReportTitle: function (input) {
    //            if (input.is("[name=ReportTitle]")) {
    //                var result = input.val() !== "-1";
    //                return result;
    //            } else {
    //                return true;
    //            }
    //        },
    //        requiredReportDate: function (input) {
    //            if (input.is("[name=ReportDate]")) {
    //                var val = input.val();
    //                if (!val || val.indexOf('month') > -1 || val.indexOf('year') > -1) {
    //                    return false;
    //                }
    //                else {
    //                    return true;
    //                }
    //            } else {
    //                return true;
    //            }
    //        },
    //        maxDate: function (input) {
    //            if (input.is("[name=ReportDate]")) {
    //                var inputDate = input.data("kendoDatePicker").value();
    //                var now = new Date();
    //                if (!inputDate || inputDate.getFullYear() > now.getFullYear() ||
    //                    (inputDate.getFullYear() == now.getFullYear() && inputDate.getMonth() > now.getMonth())) {
    //                    return false;
    //                } else {
    //                    return true;
    //                }
    //            } else {
    //                return true;
    //            }
    //        }
    //    }
    //}).data("kendoValidator");

    $("#Search").click(function () {
        Search();
    });

    $("#Clear").click(function () {
        Clear();
    });

    //$("#ExportToPdf").click(function () {

    //    //var result = validator.validate();
    //    //if (result) {
    //    //    ExportToPdf();
    //    //}
    //});

    // Hide all grids to start with.
    $("#ApprovalsGrid").addClass('hide');
    //$("#ReportGrid_1").addClass('hide');
    //$("#ReportGrid_2").addClass('hide');
    //$("#ReportGrid_99999").addClass('hide');
    //$("#ReportGrid_100000").addClass('hide');

    //// Tooltips
    //$("#ReportTitle").prop("title", "Select Report Title from list by clicking down arrow icon button");
    //$("#ReportDate").prop("title", "Enter Report Date in the MM/YYYY (Calendar Year) format");
    //$("#Generate").prop("title", "Click here or enter Alt key + R to Generate selected report");
    //$("#Clear").prop("title", "Click here or enter Alt key + C to Clear");
    //$("#ExportToPdf").prop("title", "Click here or enter Alt key + X to Export current report");

    // If Data API is not available overlay controls.
    if (loadingError != null && loadingError.length > 0) {
        GSA_alert(loadingError);

        // disable page if we have a loading error.
        ShowOverlay();
    }
});

function displayLoading(target, display) {
    var element = $(target);
    kendo.ui.progress(element, display);
}

//function onReportTitleSelect(e) {
//    var newGridName = '';

//    if (e.dataItem) {
//        var dataItem = e.dataItem;
//        newGridName = '#ReportGrid_' + dataItem.Value;

//        // Selected a new grid? Hide old partial.
//        if (gridName && gridName != newGridName) {
//            $(gridName).addClass('hide');
//            $(".status").html('');
//            $("#approvalsGridWrapper").addClass('hide');
//        }

//        gridName = newGridName;
//    }
//};

function Clear() {

    $(".status").html('');
    $('#OrderNumber').val('');
    $('#IDVContractNumber').val('');
    var kk1 = $("#ScheduledStartDate").data("kendoDatePicker");
    console.log(kk1);

    kk1.value(new Date(null)); kk1.trigger('change');
    //if (kk1.getDate() != new Date(null).getDate()) {
    //    kk1.value(new Date(null)); kk1.trigger('change');
    //}
   
    var kk2 = $("#ScheduledEndDate").data("kendoDatePicker");
    kk2.value(new Date(null)); kk2.trigger('change');
    $("#PDN").val('');
    $("#VendorName").val('');
    var dd1 = $("#FOApprovalStatusAvailable").data("kendoDropDownList");
    dd1.select(0); dd1.trigger('change');
    var dd2 = $("#NotificationStatusAvailable").data("kendoDropDownList");
    dd2.select(0); dd2.trigger('change');
    var dd3 = $("#ACOApprovalStatusAvailable").data("kendoDropDownList");
    kk1.value(new Date(null)); kk1.trigger('change');   dd3.select(0); dd3.trigger('change');
    //validator.hideMessages();

    $("#approvalsGridWrapper").addClass('hide');
}

function Search() {

    $(".status").html('');

    var result = true;
    //var result = validator.validate();

    if (result) {
        //var text = $("#ReportTitle").data("kendoDropDownList").text();
        //var date = $('#ReportDate').val();
        //$("#reportTitleExportLabel").html(text);
        //$("#reportDateExportLabel").html(date);
        //$("#reportRunDateLabel").html(kendo.toString(new Date(), "G"));

        //if (gridName) {
        displayLoading("#reportContainer", true);

        //// Show the selected grid partial.
        gridName = '#ApprovalsGrid';
        $(gridName).removeClass('hide');

        // Show grid wrapper if needed.
        $('#approvalsGridWrapper').removeClass('hide');

        gridTrigger = 'searchButton';

        var grid = $(gridName).data("kendoGrid");

        grid.dataSource.page(1);

        grid.setOptions({
            pageable: {
                Total: window.count
            }
        });

        var pageSizeDropDownList = grid.wrapper.children(".k-grid-pager").find("select").data("kendoDropDownList");
        var datasource = pageSizeDropDownList.dataSource;
        datasource.add({ text: "All", value: 'all' })
        datasource.sync()
        //}
    }
}

function isInArray(value, array) {
    return array.indexOf(value) > -1;
}

function ExportToPdf() {
    //pdfExport = true;
    //Generate();

    //setTimeout(exportData, 1000);
}

function exportData() {
    //var grid = $(gridName).data("kendoGrid");

    //// Temporarily show all rows before export.
    //var originalPageSize = grid.dataSource.pageSize();
    //var total = grid.dataSource.total();
    //grid.dataSource.pageSize(total);

    //grid.saveAsPDF()
    //    .done(function () {
    //        pdfExport = false;
    //        grid.dataSource.pageSize(originalPageSize);
    //    });
}

function getGridParams() {

    //stubbed
    
    var id = $('#OrderNumber').val();
    var IDV = $('#IDVContractNumber').val();
    var scheduledStartDate = $("#ScheduledStartDate").data("kendoDatePicker").value();
    var scheduledEndDate = $("#ScheduledEndDate").data("kendoDatePicker").value();
    var PDN = $("#PDN").val();
    var fOApprovalStatusAvailable = $("#FOApprovalStatusAvailable").val();
    var VendorName = $("#VendorName").val();
    var aCOApprovalStatusAvailable = $("#ACOApprovalStatusAvailable").val();
    var notificationStatusAvailable = $("#NotificationStatusAvailable").val();
   
    if (scheduledStartDate != null && scheduledStartDate.getDate() == new Date(null).getDate()) {
        scheduledStartDate = null;
    }
    if (scheduledEndDate != null && scheduledEndDate.getDate() == new Date(null).getDate()) {
        scheduledEndDate = null;
    }


    return {
        PPID: id,
        IDVContractNumber: IDV,
        ScheduledStartDate: scheduledStartDate,
        ScheduledEndDate: scheduledEndDate,
        PDN: PDN,
        VendorName: VendorName,
        FOApprovalStatus: fOApprovalStatusAvailable,
        ACOApprovalStatusAvailable: aCOApprovalStatusAvailable,
        NotificationStatusAvailable: notificationStatusAvailable
        //date: date
    }
}

function onGridDataBound(e) {

    var grid = e.sender;
    var page = grid.dataSource.page();
    var pageSize = grid.dataSource.pageSize();
    var totalRecords = grid.dataSource.total();

    displayLoading("#approvalsContainer", false);

    if (totalRecords == 0) {
        if (gridTrigger == 'generateButton') {
            $('#approvalsGridWrapper').addClass('hide');
        }

        $(".status").html('No records found');
    }
    else {
        // If there are groups, and we're not exporting to PDF,
        // and we have a full page of results, hide the last group footer.
        var lastGroup = $(".k-group-footer:last");
        if (!pdfExport && typeof pageSize != "undefined" && totalRecords > pageSize && (page * pageSize) < totalRecords) {
            lastGroup.hide();
        } else {
            lastGroup.show();
        }
    }

    gridTrigger = null;

    // Configure the grid to have 508-compliant pagination controls. We strip the '#' off the grid name.
    ConfigureKendoGridPaginationControlsFor508(gridName.substring(1, gridName.length));
}

// Show server errors.
function ajax_error_handler(e) {
    // Hide the activity indicator no matter what.
    displayLoading("#approvalContainer", false);

    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        GSA_alert(message);
    }
}

    //function subtotalTemplate(sum, group, text) {
    //    var grid = $(gridName);
    //    var gridData = grid.data("kendoGrid");
    //    var html = "";
    //    var spanClass = (sum < 0 ? ' class=negative-currency' : '');

    //    if (gridData == null) return html;

    //    html = "<div>" + text + ": <span" + spanClass + ">" + kendo.toString(sum, 'C') + "</span></div>";

    //    return html;
    //}

    //function approvalTotalTemplate(sum, text) {
    //    var grid = $(gridName);
    //    var gridData = grid.data("kendoGrid");
    //    var html = "";
    //    var spanClass = (sum < 0 ? ' class=negative-currency' : '');

    //    if (gridData == null) return html;

    //    var page = gridData.dataSource.page();
    //    var pageSize = gridData.dataSource.pageSize();
    //    var totalRecords = gridData.dataSource.total();

    //    // PDF export always shows approval totals.
    //    // Otherwise approval total only displays on the last page.
    //    if (pdfExport || typeof pageSize === "undefined" || page * pageSize >= totalRecords) {
    //        html = "<div>" + text + ": <span" + spanClass + ">" + kendo.toString(sum, 'C') + "</span></div>";
    //    }

    //    return html;
    //}

