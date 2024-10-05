//function loadInboundBeneficiaries() {
//    $.ajax({
//        url: "/Client/GetAllInboundBeneficiaries",
//        type: "GET",
//        success: function (data) {
//            $("#inboundBeneficiaryTblBody").empty()
//            if (data.length <= 0) {
//                alert("No inbound beneficiaries found.");
//                window.history.back(); // Go back to the previous page
//            }
//            else {
//                $.each(data, function (index, item) {
//                    console.log(data)


//                    var row = `<tr>
//                    <td><input type="checkbox" class="inbound-checkbox" data-beneficiaryid="${item.Id}" /></td>
//                        <td>${item.CompanyName}</td>
//                        <td>${item.AccountNumber}</td>
//                        <td>${item.IFSC}</td>
//                        <td>${item.BeneficiaryStatus}</td>
//                    </tr>`;

//                    $("#inboundBeneficiaryTblBody").append(row);
//                });
//            }
//        },
//        error: function (err) {
//            $("#inboundBeneficiaryTblBody").empty();
//            alert("No data available");
//        }
//    });
//}

//// Get selected checkboxes
//function getSelectedInbounds() {
//    var selectedIds = [];
//    //$(".inbound-checkbox:checked").each(function () {
//    //    selectedIds.push($(this).data('beneficiaryid'));
//    //});
//    //console.log(selectedIds)

//    $(".inbound-checkbox:checked").change(function () {
//        var inboundId = $(this).data('beneficiaryid');
//        if ($(this).is(":checked")) {
//            if (!selectedIds.includes(inboundId)) {
//                selectedIds.push(inboundId);
//            }
//        } else {
//            selectedIds = selectedIds.filter(id => id !== inboundId);
//        }
//        console.log("Selected Inbounds: ", selectedIds);
//    });

//    $("#selectAllInbounds").off('change').on('change', function () {
//        selectedIds = []; // Clear selected IDs before selecting all

//        if ($(this).is(":checked")) {
//            $(".inbound-checkbox").prop("checked", true).each(function () {
//                var inboundId = $(this).data('beneficiaryid');
//                if (!selectedIds.includes(inboundId)) {
//                    selectedIds.push(inboundId); // Add all client IDs
//                }
//            });
//        } else {
//            $(".inbound-checkbox").prop("checked", false);
//        }
//        console.log("Selected Inbounds: ", selectedIds);
//    });
//    return selectedIds;
//}

//// Approve selected disbursements
//$('#addInboundSelected').click(function () {
//    var selectedIds = getSelectedInbounds();
//    console.log(selectedIds)
//    if (selectedIds.length === 0) {
//        alert("Please select at least one beneficiary.");
//        return;
//    }

//    $.ajax({
//        type: "POST",
//        url: "/Client/AddInboundBeneficiary",
//        data: { beneficiaryIds: selectedIds },
//        traditional: true, // This ensures the array is sent correctly
//        success: function (response) {
//            if (response.success) {
//                alert(response.message);
//                window.location.href = `/Client/ViewAllOutboundBeneficiaries`;
//            } else {
//                alert(response.message);
//            }
//        },
//        error: function (xhr, status, error) {
//            alert("An error occurred while adding the beneficiary: " + error);
//        }
//    });
//});
// Declare selectedIds outside the function to keep track of the selected IDs globally
var selectedIds = [];

// Function to load inbound beneficiaries
function loadInboundBeneficiaries() {
    $.ajax({
        url: "/Client/GetAllInboundBeneficiaries",
        type: "GET",
        success: function (data) {
            $("#inboundBeneficiaryTblBody").empty();
            if (data.length <= 0) {
                alert("No inbound beneficiaries found.");
                window.history.back(); // Go back to the previous page
            } else {
                $.each(data, function (index, item) {
                    var row = `<tr>
                        <td><input type="checkbox" class="inbound-checkbox" data-beneficiaryid="${item.Id}" /></td>
                        <td>${item.CompanyName}</td>
                        <td>${item.AccountNumber}</td>
                        <td>${item.IFSC}</td>
                        <td>${item.BeneficiaryStatus}</td>
                    </tr>`;
                    $("#inboundBeneficiaryTblBody").append(row);
                });

                // Re-attach change event after populating the table
                attachCheckboxChangeEvents();
            }
        },
        error: function (err) {
            $("#inboundBeneficiaryTblBody").empty();
            alert("No data available");
        }
    });
}

// Attach change events to checkboxes
function attachCheckboxChangeEvents() {
    $(".inbound-checkbox").change(function () {
        var inboundId = $(this).data('beneficiaryid');
        if ($(this).is(":checked")) {
            if (!selectedIds.includes(inboundId)) {
                selectedIds.push(inboundId); // Add to selectedIds if checked
            }
        } else {
            selectedIds = selectedIds.filter(id => id !== inboundId); // Remove from selectedIds if unchecked
        }
        console.log("Selected Inbounds: ", selectedIds);
    });

    // Handle select all checkbox
    $("#selectAllInbounds").off('change').on('change', function () {
        selectedIds = []; // Clear selected IDs before selecting all
        if ($(this).is(":checked")) {
            $(".inbound-checkbox").prop("checked", true).each(function () {
                var inboundId = $(this).data('beneficiaryid');
                selectedIds.push(inboundId); // Add all IDs to selectedIds
            });
        } else {
            $(".inbound-checkbox").prop("checked", false);
        }
        console.log("Selected Inbounds: ", selectedIds);
    });
}

// Approve selected disbursements
$('#addInboundSelected').click(function () {
    console.log(selectedIds); // Display selected IDs in console
    if (selectedIds.length === 0) {
        alert("Please select at least one beneficiary.");
        return;
    }

    $.ajax({
        type: "POST",
        url: "/Client/AddInboundBeneficiary",
        data: { beneficiaryIds: selectedIds },
        traditional: true, // This ensures the array is sent correctly
        success: function (response) {
            if (response.success) {
                alert(response.message);
                window.location.href = `/Client/ViewAllOutboundBeneficiaries`;
            } else {
                alert(response.message);
            }
        },
        error: function (xhr, status, error) {
            alert("An error occurred while adding the beneficiary: " + error);
        }
    });
});

$(document).ready(() => {
    loadInboundBeneficiaries();
});

