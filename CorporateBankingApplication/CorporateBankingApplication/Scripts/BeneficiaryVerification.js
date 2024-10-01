function loadBeneficiaryForVerification() {
    $.ajax({
        url: "/Admin/GetOutboundBeneficiaryForVerification",
        type: "GET",
        success: function (data) {
            $("#beneficiaryToBeVerifiedTblBody").empty();
            if (data.length === 0) {
                var noBeneficiariesMessage = `<tr><td colspan="7" class="text-center">No beneficiaries left to be verified</td></tr>`;
                $("#beneficiaryToBeVerifiedTblBody").append(noBeneficiariesMessage);
            } else {
                $.each(data, function (index, item) {
                    // Create a list of document links
                    var documents = item.DocumentUrls.map(function (docPath) {
                        var fileName = docPath.split('/').pop(); // Extract file name from path
                        return `<a href="#" class="open-document" data-filepath="${docPath}" target="_blank">${fileName}</a><br>`;
                    }).join('');

                    var row = `<tr>
                        <td>${item.BeneficiaryName}</td>
                        <td>${item.AccountNumber}</td>
                        <td>${item.BankIFSC}</td>
                        <td>${item.BeneficiaryType}</td>
                        <td>${documents}</td>
                        <td><div class="d-flex">
                            <button onclick="approveBeneficiary('${item.Id}', 'APPROVED')" class="btn btn-outline-dark mx-3">Approve</button>
                            <button onclick="rejectBeneficiary('${item.Id}', 'REJECTED')" class="btn btn-outline-dark">Reject</button>
                        </div></td>
                    </tr>`;
                    $("#beneficiaryToBeVerifiedTblBody").append(row);
                });

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
            $("#beneficiaryToBeVerifiedTblBody").empty();
            var errorMessage = `<tr><td colspan="7" class="text-center">No beneficiaries waiting to be verified</td></tr>`;
            $("#beneficiaryToBeVerifiedTblBody").append(errorMessage);
            alert("Error occurred while loading beneficiaries for verification.");
        }
    });
}

//document modal closing button
$('.close').on('click', function () {
    $('#documentModal').modal('hide');
});

// Use the same function for both approve and reject buttons
function approveBeneficiary(beneficiaryId) {
    updateBeneficiaryStatus(beneficiaryId, 'APPROVED');
}

function rejectBeneficiary(beneficiaryId) {
    updateBeneficiaryStatus(beneficiaryId, 'REJECTED');
}
// Combined function for approving or rejecting client
function updateBeneficiaryStatus(beneficiaryId, status) {
    $.ajax({
        url: "/Admin/UpdateOutboundBeneficiaryOnboardingStatus",
        type: "POST",
        data: { id: beneficiaryId, status: status }, // Pass the status (APPROVED or REJECTED)
        success: function () {
            alert(`Beneficiary ${status === 'APPROVED' ? 'Approved' : 'Rejected'}`);
            loadBeneficiaryForVerification();
        },
        error: function () {
            alert(`Failed to ${status === 'APPROVED' ? 'Approve' : 'Reject'} the Beneficiary`);
        }
    });
}




