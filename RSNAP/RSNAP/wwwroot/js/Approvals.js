
//var gridName = '';
var validator;
var pdfExport = false;
var isPostBack = true;
var runtime = 0;
var pagIndex = 1;
//var gridTrigger;
function Search(_rangeControl,_size) {
    if (!_rangeControl) {
        ProActIds = null;
    }
    runtime++;
    if (runtime > 1) {
        isPostBack = false;
    }
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

        gridTrigger = 'searchButton';

        var grid = $(gridName).data("kendoGrid");

        //grid.dataSource.page(1);



        var pageSizeDropDownList = grid.wrapper.children(".k-grid-pager").find("select").data("kendoDropDownList");
        var datasource = pageSizeDropDownList.dataSource;
        if (_size == -1) {
            grid.dataSource.page(1);
            grid.dataSource.pageSize(_size);
        } else {
            grid.dataSource.pageSize(_size);
        }
        
        if (isPostBack) {
             datasource.add({ text: "All", value: 'all' })
        }
        
        datasource.sync()
        //}
    }

}

function inItRODropDown() {
    var dd1 = $("#FOApprovalStatusAvailable").data("kendoDropDownList");
    dd1.select(3);
    var dd2 = $("#NotificationStatusAvailable").data("kendoDropDownList");
    dd2.select(1);
    var dd3 = $("#ACOApprovalStatusAvailable").data("kendoDropDownList");
    dd3.select(3); 
}
function inItCODropDown() {
    var dd1 = $("#FOApprovalStatusAvailable").data("kendoDropDownList");
    dd1.select(3);
    var dd2 = $("#NotificationStatusAvailable").data("kendoDropDownList");
    dd2.select(1);
}
function inItFODropDown() {
    var dd1 = $("#FOApprovalStatusAvailable").data("kendoDropDownList");
    dd1.select(2);
    var dd2 = $("#NotificationStatusAvailable").data("kendoDropDownList");
    dd2.select(1);
}



$(document).ready(function () {
    setTimeout("Search(null,-1);", 500);
    //setTimeout("$(\"#pagerDropDown\").val(-1);", 500);

    setTimeout(function () {
        $('#pagerDropDown').change(function () {
            Search(null, $('#pagerDropDown').val());
        });
    }, 500);

   

    //if (roleText=="RO") {
    //    setTimeout("inItRODropDown();", 500);
    //}

    //if (roleText == "CO") {
    //    setTimeout("inItCODropDown();", 500);
    //}

    //if (roleText == "FO") {
    //    setTimeout("inItFODropDown();", 500);
    //}

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
        Search(false);
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

function onPaging(arg) {
    pagIndex = arg.page
}




function displayLoading(target, display) {
    var element = $(target);
    kendo.ui.progress(element, display);
}
var sortModel = { field: null, dir: null };
function onSorting(arg) {
    sortModel.field = arg.sort.field;
    sortModel.dir = arg.sort.dir;
}

function Clear() {

    $(".status").html('');
    $('#OrderNumber').val('');
    $('#IDVContractNumber').val('');
    var kk1 = $("#ScheduledStartDate").data("kendoDatePicker");

    kk1.value(null);

    var kk2 = $("#ScheduledEndDate").data("kendoDatePicker");

    kk2.value(null);


    $("#PDN").val('');
    $("#VendorName").val('');
    var dd1 = $("#FOApprovalStatusAvailable").data("kendoDropDownList");
    dd1.select(0); dd1.trigger('change');
    var dd2 = $("#NotificationStatusAvailable").data("kendoDropDownList");
    dd2.select(0); dd2.trigger('change');
    var dd3 = $("#ACOApprovalStatusAvailable").data("kendoDropDownList");
    dd3.select(0); dd3.trigger('change');
    //validator.hideMessages();


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
        ACOApprovalStatus: aCOApprovalStatusAvailable,
        NotificationStatus: notificationStatusAvailable,
        IsPostBack: isPostBack,
        IdList: ProActIds,
        Field: sortModel.field,
        Dir: sortModel.dir
    }
}
var dataList = [],
    disabledItemIds = [] //ProActID
    ;
function onGridDataBound(e) {

    var grid = e.sender;
    var page = grid.dataSource.page();
    var pageSize = grid.dataSource.pageSize();
    var totalRecords = grid.dataSource.total();
    if (pageSize == -1) {
        $("#pager-info").text(`1 - ${totalRecords} of ${totalRecords} items`);
    } else {
        $("#pager-info").text(`${page} - ${pageSize} of ${totalRecords} items`);
    }
    

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
    dataList = e.sender._data;
    for (var i = 0; i < dataList.length; i++) {
        if (dataList[i].CheckboxStatus === false) {
            disabledItemIds.push(dataList[i].ProActID);
            var _row = e.sender.tbody[0].rows[i];
            e.sender.tbody[0].rows[i].childNodes[0].children[0].disabled = true;
            $(_row).addClass('select-disabled');
        }

    }
    
    if (roleText == "FO" || roleText == "CO") {

        var _thead = e.sender.thead[0].rows[0];

        $(_thead.childNodes[14].children[0]).css({ "color": "#a9aeb1" });
        $(_thead.childNodes[15].children[0]).css({ "color": "#a9aeb1" });
        $(_thead.childNodes[16].children[0]).css({ "color": "#a9aeb1" });

       
        for (var j = 0; j < dataList.length; j++) {
            var _FOrow = e.sender.tbody[0].rows[j];
            $(_FOrow.childNodes[14]).css({ "cssText":"color:#a9aeb1!important;" });
            $(_FOrow.childNodes[15]).css({ "cssText": "color:#a9aeb1!important;" });
            $(_FOrow.childNodes[16]).css({ "cssText": "color:#a9aeb1!important;" });
        }
    }

    for (var i = 0; i < dataList.length; i++) {
        if (dataList[i].CheckboxStatus === false) {
            disabledItemIds.push(dataList[i].ProActID);
            var _row = e.sender.tbody[0].rows[i];
            e.sender.tbody[0].rows[i].childNodes[0].children[0].disabled = true;
            $(_row).addClass('select-disabled');
        }

    }


    // Configure the grid to have 508-compliant pagination controls. We strip the '#' off the grid name.
    ConfigureKendoGridPaginationControlsFor508(gridName.substring(1, gridName.length));
    $('.k-i-arrow-end-left').text('');
    $('.k-i-arrow-60-left').text('');
    $('.k-i-arrow-60-right').text('');
    $('.k-i-arrow-end-right').text('');
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
var selectedDataItems = [];
function onChange(e) {
    var selectedRows = this.select();
    var currentList = [];
    for (var i = 0; i < selectedRows.length; i++) {
        currentList.push({ 'id': selectedRows[i].children[2].innerText, 'newComments': selectedRows[i].children[13].children[0].value, 'ProId': selectedRows[i].children[1].innerText });

    }

    selectedDataItems = currentList;
}
var ProActIds;

function getSelectedProActIds(selectedItems) {
    if (selectedItems == null) {
        return null;
    }
    var selectedProActIds = [];
    for (var i = 0; i < selectedItems.length; i++) {
        if (disabledItemIds.indexOf(selectedItems[i].id) == -1) {
            selectedProActIds.push(selectedItems[i]);
        }
    }
    ProActIds = selectedProActIds;
    return selectedProActIds;
}

function Approve() {
    proActIds = null;
    var proActIds = getSelectedProActIds(this.selectedDataItems);
    if (proActIds == null||proActIds.length == 0) {
        GSA_alert("No items selected.");
        return;
    }
    
    $.post("/rsnap/Approvals/ApproveProcess", { modes: proActIds }, function (data) {
        GSA_alert(data);
        Clear();
        Search(true);
        
    });
    this.selectedDataItems = null;
}

function Unapprove() {
    proActIds = null;
    console.log(this.selectedDataItems);
    var proActIds = getSelectedProActIds(this.selectedDataItems);
    if (proActIds == null ||proActIds.length == 0) {
        GSA_alert("No items selected.");

        return;
    }
   
    $.post("/rsnap/Approvals/NnapprovedProcess", { modes: proActIds }, function (data) {
        GSA_alert(data);
        Clear();
        Search(true);
    });
    this.selectedDataItems = null;
}

function UnderReview() {
    proActIds = null;
    var proActIds = getSelectedProActIds(this.selectedDataItems);
    if (proActIds == null ||proActIds.length == 0) {
        GSA_alert("No items selected.");
        return;
    }
    
    $.post("/rsnap/Approvals/UnderReviewProcess", { modes: proActIds }, function (data) {
        GSA_alert(data);
        Clear();
        Search(true);
        this.selectedDataItems = null;
    });
}

function ExportExcel() {
    $.post("/rsnap/Approvals/ExportExcelData", getGridParams(), function (data) {
        if (data.Success) {
            location.href = "/rsnap/Approvals/GetExcel/" + data.Id;
        } else {
            GSA_alert("Please try again later.");
        }

    });
}

function SaveComments() {

    gridName = '#ApprovalsGrid';
    var grid = $(gridName).data("kendoGrid");

    var dataList = [];
    
    for (var i = 0; i < grid.tbody[0].childNodes.length; i++) {
        var row = grid.tbody[0].childNodes[i];
        if (row.childNodes[13].childNodes[0].value) {
            dataList.push({ 'id': row.children[2].innerText, 'newComments': row.children[13].children[0].value, 'ProId': row.children[1].innerText })
        }
        

    }

    $.post("/rsnap/Approvals/SaveComments", { modes: dataList }, function (data) {
        GSA_alert(data);
        Search(true);
    });

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

