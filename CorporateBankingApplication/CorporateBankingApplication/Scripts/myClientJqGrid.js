$(document).ready(function () {
    $("#grid").jqGrid({
        url: "/Admin/GetClientDetails",
        datatype: "json",
        colNames: ["Id", "Username", "Email", "Company Name", "Contact Information", "Location", "Balance", "Onboarding Status", "Is Active"],   //change column names
        colModel: [{ name: "Id", key: true, hidden: true },
        { name: "UserName", editable: true, search: false },
        { name: "Email", editable: true, search: false },
        { name: "CompanyName", editable: true, searchoptions: { sopt: ['eq'] } },
        { name: "ContactInformation", editable: true, search: false },
        { name: "Location", editable: true, search: false },
        { name: "Balance", editable: false, search: false },
        { name: "OnBoardingStatus", editable: false, search: false },
        { name: "IsActive", editable: false, search: false }],  //change model names
        height: "250",
        caption: "Client Information",
        pager: "#pager",
        rowNum: 5,
        rowList: [5, 10, 15],
        sortname: 'id',
        sortorder: 'asc',
        viewrecords: true,
        width: "1500",
        gridComplete: function () {
            $("#grid").jqGrid('navGrid', '#pager', { edit: true, add: false, del: true, search: true, refresh: true },
                {
                    //edit
                    url: "/Admin/EditClientDetails",
                    closeAfterEdit: true,
                    width: 600,
                    afterSubmit: function (response, postdata) {
                        var result = JSON.parse(response.responseText);
                        if (result.success) {
                            alert(result.message);
                            return [true];
                        }
                        else {
                            alert(result.message);
                            return [false];
                        }
                    }
                },
                {
                    //add=false
                },
                {
                    //delete
                    url: "/Admin/DeleteClientDetails",
                    afterSubmit: function (response, postdata) {
                        var result = JSON.parse(response.responseText);
                        if (result.success) {
                            alert(result.message);
                            return [true];
                        }
                        else {
                            alert(result.message);
                            return [false];
                        }
                    }
                },
                {
                    //search
                    multipleSearch: false,
                    closeAfterSearch: true
                }
            );
        }
    })
})


//checkbox trial

//$(document).ready(function () {
//    $("#grid").jqGrid({
//        url: "/Admin/GetClientDetails",
//        datatype: "json",
//        colNames: ["Id", "Username", "Email", "Company Name", "Contact Information", "Location", "Balance", "Onboarding Status", "Is Active"],   //change column names
//        colModel: [{ name: "Id", key: true, hidden: true },
//        { name: "UserName", editable: true, search: false },
//        { name: "Email", editable: true, search: false },
//        { name: "CompanyName", editable: true, searchoptions: { sopt: ['eq'] } },
//        { name: "ContactInformation", editable: true, search: false },
//        { name: "Location", editable: true, search: false },
//        { name: "Balance", editable: false, search: false },
//        { name: "OnBoardingStatus", editable: false, search: false },
//        {
//            name: "IsActive",
//            editable: true,
//            edittype: "checkbox",
//            formatter: "checkbox",
//            formatoptions: { disabled: false },
//            align: "center",
//            editoptions: { value: "true:false" },
//            formatter: function (cellvalue, options, rowObject) {
//                // Convert boolean to string "true"/"false" for jqGrid checkbox
//                var isChecked = cellvalue === true || cellvalue === "true" ? "checked" : "";
//                return `<input type='checkbox' ${isChecked} data-id='${rowObject.Id}' class='isActive-checkbox'/>`;
//            }

//        }],  //change model names
//        height: "250",
//        caption: "Client Information",
//        pager: "#pager",
//        rowNum: 5,
//        rowList: [5, 10, 15],
//        sortname: 'id',
//        sortorder: 'asc',
//        viewrecords: true,
//        width: "1500",
//        gridComplete: function () {
//            // Attach event to checkboxes
//            $(".isActive-checkbox").on("change", function () {
//                var isChecked = $(this).is(":checked");
//                var clientId = $(this).data("id");

//                // AJAX call to update the IsActive status
//                $.ajax({
//                    url: '/Admin/UpdateClientIsActive',
//                    type: 'POST',
//                    data: {
//                        id: clientId,
//                        isActive: isChecked
//                    },
//                    success: function (response) {
//                        if (response.success) {
//                            alert("Client status has been updated.");
//                        } else {
//                            alert("Failed to update the Client status.");
//                        }
//                    },
//                    error: function () {
//                        alert("Error in updating Client status.");
//                    }
//                });
//            });
//            $("#grid").jqGrid('navGrid', '#pager', { edit: true, add: false, del: true, search: true, refresh: true },
//                {
//                    //edit
//                    url: "/Admin/EditClientDetails",
//                    closeAfterEdit: true,
//                    width: 600,
//                    afterSubmit: function (response, postdata) {
//                        var result = JSON.parse(response.responseText);
//                        if (result.success) {
//                            alert(result.message);
//                            return [true];
//                        }
//                        else {
//                            alert(result.message);
//                            return [false];
//                        }
//                    }
//                },
//                {
//                    //add=false   
//                },
//                {
//                    //delete
//                    url: "/Admin/DeleteClientDetails",
//                    afterSubmit: function (response, postdata) {
//                        var result = JSON.parse(response.responseText);
//                        if (result.success) {
//                            alert(result.message);
//                            return [true];
//                        }
//                        else {
//                            alert(result.message);
//                            return [false];
//                        }
//                    }
//                },
//                {
//                    //search
//                    multipleSearch: false,
//                    closeAfterSearch: true
//                }
//            );
//        }
//    })
//})



