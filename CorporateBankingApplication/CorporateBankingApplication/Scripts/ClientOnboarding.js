// Global variable to store selected client IDs
var selectedIds = [];

function loadClientsForVerification() {
    $.ajax({
        url: "/Admin/GetClientsForVerification",
        type: "GET",
        success: function (data) {
            $("#clientToBeVerifiedTblBody").empty();
            
            if (data.length === 0) {
                // If no clients to verify, display message
                var noClientsMessage = `<tr><td colspan="7" class="text-center">No clients left to be verified</td></tr>`;
                $("#clientToBeVerifiedTblBody").append(noClientsMessage);
                $("#approveClientBtn").hide()
                $("#rejectClientBtn").hide()
            } else {
                $.each(data, function (index, item) {
                    // Create a list of document links
                    var documents = item.DocumentPaths.map(function (docPath) {
                        var fileName = docPath.split('/').pop(); // Extract file name from path
                        return `<a href="#" class="open-document" data-filepath="${docPath}" target="_blank">${fileName}</a><br>`;
                    }).join('');

                    var row = `<tr>
                    <td>
                        <input type="checkbox" class="is-ClientSelected-checkbox" data-clientid="${item.Id}" ${item.ClientSelect ? "checked" : ""} />
                    </td>
                        <td>${item.UserName}</td>
                        <td>${item.Email}</td>
                        <td>${item.CompanyName}</td>
                        <td>${item.ContactInformation}</td>
                        <td>${item.Location}</td>
                        <td>${documents}</td>
                    </tr>`;
                    $("#clientToBeVerifiedTblBody").append(row);
                });

                // Initialize checkbox change events
                initializeCheckboxEvents();

                // Add click event to open modal and display document
                $(".open-document").click(function (e) {
                    e.preventDefault();
                    var filePath = $(this).data('filepath');

                    // Load the document in the iframe
                    $('#documentFrame').attr('src', filePath);

                    // Show the modal
                    $('#documentModal').modal('show');
                });
            }
        },
        error: function (err) {
            $("#clientToBeVerifiedTblBody").empty();
            var errorMessage = `<tr><td colspan="7" class="text-center">No clients waiting to be verified</td></tr>`;
            $("#clientToBeVerifiedTblBody").append(errorMessage);
            alert("Error occurred while loading clients for verification.");
        }
    });
}

function initializeCheckboxEvents() {
    // Reset the selected IDs array
    selectedIds = [];

    // Handle individual client checkbox selection
    $(".is-ClientSelected-checkbox").change(function () {
        var clientId = $(this).data('clientid');
        if ($(this).is(":checked")) {
            if (!selectedIds.includes(clientId)) {
                selectedIds.push(clientId);
            }
        } else {
            selectedIds = selectedIds.filter(id => id !== clientId);
        }
        console.log("Selected Clients: ", selectedIds);
    });

    // Select/Deselect all clients
    $("#selectAllClients").off('change').on('change', function () {
        selectedIds = []; // Clear selected IDs before selecting all

        if ($(this).is(":checked")) {
            $(".is-ClientSelected-checkbox").prop("checked", true).each(function () {
                var clientId = $(this).data('clientid');
                if (!selectedIds.includes(clientId)) {
                    selectedIds.push(clientId); // Add all client IDs
                }
            });
        } else {
            $(".is-ClientSelected-checkbox").prop("checked", false);
        }
        console.log("Selected Clients: ", selectedIds);
    });
}

function showLoader() {
    $("#loader").show();
}

function hideLoader() {
    $("#loader").hide();
}

// Document modal closing button
$('.close').on('click', function () {
    $('#documentModal').modal('hide');
});

$('.closeRejectionPopup').on('click', function () {
    $('#rejectionReasonModal').modal('hide');
});


function getCheckedClientId() {
    // Simply return the selectedIds array
    return selectedIds;
}

function approveClient() {
    var clientIds = getCheckedClientId(); // Get the selected IDs
    if (clientIds.length > 0) {
        updateClientStatus(clientIds, 'APPROVED'); // Proceed if there are selected clients
    } else {
        alert("No clients selected for approval.");
    }
}

//function rejectClient() {
//    var clientIds = getCheckedClientId(); // Get the selected IDs
//    if (clientIds.length > 0) {
//        updateClientStatus(clientIds, 'REJECTED'); // Proceed if there are selected clients
//    } else {
//        alert("No clients selected for rejection.");
//    }
//}

function rejectClient() {
    var clientIds = getCheckedClientId(); // Get the selected IDs
    if (clientIds.length > 0) {
        // Show the rejection reason modal
        $('#rejectionReasonModal').modal('show');

        // Handle submission of rejection reason
        $('#submitRejectionReason').off('click').on('click', function () {
            var reason = $('#rejectionReason').val();
            if (reason.trim() === "") {
                alert("Please provide a reason for rejection.");
                return;
            }
            // Proceed to reject clients with the reason
            updateClientStatus(clientIds, 'REJECTED', reason);
        });
    } else {
        alert("No clients selected for rejection.");
    }
}


function updateClientStatus(clientIds, status, reason = '') {
    if (!clientIds || clientIds.length === 0) {
        return; // Exit if no clients are selected
    }

    showLoader();
    $.ajax({
        url: "/Admin/UpdateClientOnboardingStatus",
        type: "POST",
        data: { id: clientIds, status: status, rejectionReason: reason }, // Pass the status, client IDs, and reason
        success: function () {
            setTimeout(function () {
                hideLoader();
                alert(`Client(s) ${status === 'APPROVED' ? 'Approved' : 'Rejected'} successfully`);
                loadClientsForVerification(); // Reload the list after successful action
            }, 5000);
            $('#rejectionReasonModal').modal('hide');
        },
        error: function () {
            setTimeout(function () {
                hideLoader();
                alert(`Failed to ${status === 'APPROVED' ? 'Approve' : 'Reject'} the Client(s).`);
            }, 5000);
            $('#rejectionReasonModal').modal('hide');
        }
    });
}



//function updateClientStatus(clientIds, status) {
//    if (!clientIds || clientIds.length === 0) {
//        return; // Exit if no clients are selected
//    }

//    showLoader();
//    $.ajax({
//        url: "/Admin/UpdateClientOnboardingStatus",
//        type: "POST",
//        data: { id: clientIds, status: status }, // Pass the status and client IDs
//        success: function () {
//            setTimeout(function () {
//                hideLoader();
//                alert(`Client(s) ${status === 'APPROVED' ? 'Approved' : 'Rejected'}`);
//                loadClientsForVerification(); // Reload the list after successful action
//            }, 5000);
//        },
//        error: function () {
//            setTimeout(function () {
//                hideLoader();
//                alert(`Failed to ${status === 'APPROVED' ? 'Approve' : 'Reject'} the Client(s).`);
//            }, 5000);
//        }
//    });
//}


