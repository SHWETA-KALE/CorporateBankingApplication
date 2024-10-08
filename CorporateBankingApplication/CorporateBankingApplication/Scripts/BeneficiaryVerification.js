var selectedIds = [];
function loadBeneficiaryForVerification(page = 1) {
    $.ajax({
        url: `/Admin/GetOutboundBeneficiaryForVerification?page=${page}`,
        type: "GET",
        success: function (data) {
            $("#beneficiaryToBeVerifiedTblBody").empty();

            if (data.Data.length === 0) {
                var noBeneficiariesMessage = `<tr><td colspan="7" class="text-center">No beneficiaries left to be verified</td></tr>`;
                $("#beneficiaryToBeVerifiedTblBody").append(noBeneficiariesMessage);
                $("#approveOutboundBtn").hide();
                $("#rejectOutboundBtn").hide();
            } else {
                $.each(data.Data, function (index, item) {
                    var documents = item.DocumentUrls.map(function (docPath) {
                        var fileName = docPath.split('/').pop();
                        return `<a href="#" class="open-document" data-filepath="${docPath}" target="_blank">${fileName}</a><br>`;
                    }).join('');

                    var row = `<tr>
                <td><input type="checkbox" class="is-OutboundSelected-checkbox" data-outboundid="${item.Id}" ${item.OutboundSelect ? "checked" : ""} /></td>
                <td>${item.ClientName}</td>
                <td>${item.BeneficiaryName}</td>
                <td>${item.AccountNumber}</td>
                <td>${item.BankIFSC}</td>
                <td>${item.BeneficiaryType}</td>
                <td>${documents}</td>
            </tr>`;

                    $("#beneficiaryToBeVerifiedTblBody").append(row);
                });

                initializeCheckboxEvents();

                // Attach click events for document links
                $(".open-document").click(function (e) {
                    e.preventDefault();
                    var filePath = $(this).data('filepath');
                    $('#documentFrame').attr('src', filePath);
                    $('#documentModal').modal('show');
                });

                // Render pagination links after updating the table
                renderPaginationLinks(data.TotalPages, data.CurrentPage);
            }
        },

        error: function (err) {
            $("#beneficiaryToBeVerifiedTblBody").empty();
            var errorMessage = `<tr><td colspan="7" class="text-center">No beneficiaries waiting to be verified</td></tr>`;
            $("#beneficiaryToBeVerifiedTblBody").append(errorMessage);
            alert("Error occurred while loading beneficiaries for verification.");
        }
    });
}

function renderPaginationLinks(totalPages, currentPage) {
    const paginationContainer = $("#paginationLinks");
    paginationContainer.empty();

    for (let page = 1; page <= totalPages; page++) {
        const activeClass = page === currentPage ? 'active' : '';
        const pageLink = `<li class="page-item ${activeClass}"><a class="page-link" href="javascript:void(0)" onclick="loadBeneficiaryForVerification(${page})">${page}</a></li>`;
        paginationContainer.append(pageLink);
    }
}
function initializeCheckboxEvents() {
    // Reset the selected IDs array
    selectedIds = [];

    // Handle individual client checkbox selection
    $(".is-OutboundSelected-checkbox").change(function () {
        var outboundId = $(this).data('outboundid');
        if ($(this).is(":checked")) {
            if (!selectedIds.includes(outboundId)) {
                selectedIds.push(outboundId);
            }
        } else {
            selectedIds = selectedIds.filter(id => id !== outboundId);
        }
        console.log("Selected Outbounds: ", selectedIds);
    });

    // Select/Deselect all clients
    $("#selectAllOutbounds").off('change').on('change', function () {
        selectedIds = []; // Clear selected IDs before selecting all

        if ($(this).is(":checked")) {
            $(".is-OutboundSelected-checkbox").prop("checked", true).each(function () {
                var outboundId = $(this).data('outboundid');
                if (!selectedIds.includes(outboundId)) {
                    selectedIds.push(outboundId); // Add all client IDs
                }
            });
        } else {
            $(".is-OutboundSelected-checkbox").prop("checked", false);
        }
        console.log("Selected Outbounds: ", selectedIds);
    });
}


function showLoader() {
    $("#loader").show();
}
function hideLoader() {
    $("#loader").hide();
}





function getCheckedOutboundId() {
    // Simply return the selectedIds array
    return selectedIds;
}

// Use the same function for both approve and reject buttons
function approveBeneficiary() {
    var outboundIds = getCheckedOutboundId(); // Get the selected IDs
    if (outboundIds.length > 0) {
        updateBeneficiaryStatus(outboundIds, 'APPROVED');
    } else {
        alert("No outbound beneficiary/s selected for approval.");
    }
}

//function rejectBeneficiary(beneficiaryId) {
//    var outboundIds = getCheckedOutboundId(); // Get the selected IDs
//    if (outboundIds.length > 0) {
//        updateBeneficiaryStatus(outboundIds, 'REJECTED'); 
//    } else {
//        alert("No outbound beneficiary/s selected for rejection.");
//    }
//}
//// Combined function for approving or rejecting beneficiary
//function updateBeneficiaryStatus(beneficiaryIds, status) {
//    if (!beneficiaryIds || beneficiaryIds.length === 0) {
//        return; 
//    }
//    showLoader()
//    $.ajax({
//        url: "/Admin/UpdateOutboundBeneficiaryOnboardingStatus",
//        type: "POST",
//        data: { id: beneficiaryIds, status: status }, // Pass the status (APPROVED or REJECTED)
//        success: function () {
//            setTimeout(function () {
//                hideLoader();
//                alert(`Beneficiary ${status === 'APPROVED' ? 'Approved' : 'Rejected'}`);
//                loadBeneficiaryForVerification();
//            }, 5000);
            
//        },
//        error: function () {
//            setTimeout(function () {
//                hideLoader();
//                alert(`Failed to ${status === 'APPROVED' ? 'Approve' : 'Reject'} the Beneficiary`);
//            }, 5000);
//        }
//    });
//}

function rejectBeneficiary() {
    var outboundIds = getCheckedOutboundId(); // Get the selected IDs
    if (outboundIds.length > 0) {
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
            updateBeneficiaryStatus(outboundIds, 'REJECTED', reason);
        });
    } else {
        alert("No outbound beneficiary/s selected for rejection.");
    }
}

function updateBeneficiaryStatus(beneficiaryIds, status, reason = '') {
    if (!beneficiaryIds || beneficiaryIds.length === 0) {
        return;
    }

    showLoader();
    $.ajax({
        url: "/Admin/UpdateOutboundBeneficiaryOnboardingStatus",
        type: "POST",
        data: { id: beneficiaryIds, status: status, rejectionReason: reason }, // Pass the status, client IDs, and reason
        success: function () {
            setTimeout(function () {
                hideLoader();
                alert(`Beneficiary ${ status === 'APPROVED' ? 'Approved' : 'Rejected'}`);
            loadBeneficiaryForVerification();
        }, 5000);
    $('#rejectionReasonModal').modal('hide');

},
error: function () {
    setTimeout(function () {
        hideLoader();
        alert(`Failed to ${ status === 'APPROVED' ? 'Approve' : 'Reject'} the Beneficiary`);
}, 5000);
$('#rejectionReasonModal').modal('hide');
        }
    });
}
//document modal closing button
$('.close').on('click', function () {
    $('#documentModal').modal('hide');
});

$('.closeRejectionPopup').on('click', function () {
    $('#rejectionReasonModal').modal('hide');
});


