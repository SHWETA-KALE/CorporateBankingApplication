

$(document).ready(function () {
    $("#grid").jqGrid({
        url: "/Admin/GetClientDetails",
        datatype: "json",
        colNames: ["Id", "Username", "Email", "Company Name", "Contact Information", "Location", "Balance", "Account Number", "IFSC", "Onboarding Status", "IsActive"],
        colModel: [
            { name: "Id", key: true, hidden: true },
            { name: "UserName", editable: true, search: false },
            { name: "Email", editable: true, search: false },
            { name: "CompanyName", editable: true, searchoptions: { sopt: ['eq'] } },
            { name: "ContactInformation", editable: true, search: false },
            { name: "Location", editable: true, search: false },
            { name: "Balance", editable: false, search: false },
            { name: "AccountNumber", editable: false, search: false },
            { name: "IFSC", editable: false, search: false },
            { name: "OnboardingStatus", editable: false, search: false },
            { name: "IsActive", editable: false, formatter: "checkbox", editoptions: { value: "true:false" }, align: "center", formatoptions: { disabled: false }, classes: "isActive-checkbox" }
        ],
        height: "250",
        caption: "Client Information",
        pager: "#pager",
        rowNum: 5,
        rowList: [5, 10, 15],
        sortname: 'Id',
        sortorder: 'asc',
        viewrecords: true,
        width: "1500",
        gridComplete: function () {
            // Attach event to checkboxes
            $(".isActive-checkbox").off("change").on("change", function () {
                var isChecked = $(this).is(':checked');
                var rowId = $(this).closest("tr.jqgrow").attr("id"); // Get the row ID

                $.ajax({
                    url: '/Admin/UpdateClientIsActive',
                    type: 'POST',
                    data: {
                        id: rowId,
                        isActive: isChecked
                    },
                    success: function (response) {
                        if (response.success) {
                            alert("Client's status has been updated successfully.");
                        } else {
                            alert("Failed to update the Client's status: " + response.message);
                        }
                    },
                    error: function (xhr) {
                        console.log("Error details:", xhr.responseText);
                        alert("Error in updating Client's status: " + xhr.responseText);
                    }
                });
            });
        }
    });
    $("#grid").jqGrid('navGrid', '#pager', { edit: true, add: false, del: false, search: true, refresh: true },
        {
            // Edit
            url: "/Admin/EditClientDetails",
            closeAfterEdit: true,
            width: 600,
            afterSubmit: function (response) {
                var result = JSON.parse(response.responseText);
                if (result.success) {
                    alert(result.message);
                    return [true];
                } else {
                    alert(result.message);
                    return [false];
                }
            }
        },
        {
            // Add false
        },
        {
            // Delete false
        },
        {
            // Search
            multipleSearch: false,
            closeAfterSearch: true
        }
    );
});