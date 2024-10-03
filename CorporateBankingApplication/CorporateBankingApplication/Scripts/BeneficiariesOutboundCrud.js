function loadOutboundBeneficiaries() {
    $.ajax({
        url: "/Client/GetAllOutboundBeneficiaries",
        type: "GET",
        success: function (data) {
            $("#beneficiaryTblBody").empty()
            if (data.length > 0) {
                $.each(data, function (index, item) {
                    console.log(data)
                    var documentLinks = item.DocumentUrls.map(function (url) {
                        var fileName = url.split('/').pop(); // Extract file name from URL
                        return `<a href="#" class="document-link" data-url="${url}">${fileName}</a>`; // Include data-url
                    }).join("<br/> ");

                    var row = `<tr>
                        <td>${item.BeneficiaryName}</td>
                        <td>${item.AccountNumber}</td>
                        <td>${item.BankIFSC}</td>
                        <td>${item.BeneficiaryStatus}</td>
                        <td>${item.BeneficiaryType}</td>
                        <td>
                            <input type="checkbox" class="is-active-checkbox" data-beneficiaryid="${item.Id}" ${item.IsActive ? "checked" : ""} />
                        </td>
                        <td>${documentLinks}</td>
                        <td class="editbeneficiary-btn-cell">
                            <button onclick="editBeneficiary('${item.Id}')" class="btn btn-outline-dark edit-btn" style="${item.IsActive ? '' : 'display:none;'}">Edit</button>
                        </td>
                    </tr>`;

                    $("#beneficiaryTblBody").append(row);
                });

                $(".is-active-checkbox").change(function () {
                    var beneficiaryId = $(this).data("beneficiaryid");
                    var isActive = $(this).is(":checked");
                    updateBeneficiaryStatus(beneficiaryId, isActive);
                });
            } else {
                $("#beneficiaryTblBody").append("<tr><td colspan='5'>No outbound beneficiaries found.</td></tr>");
            }
        },
        error: function (err) {
            $("#beneficiaryTblBody").empty();
            alert("No data available");
        }
    });
}


$(document).on("click", ".document-link", function (e) {
    e.preventDefault(); // Prevent the default link behavior
    var url = $(this).data("url"); // Get the URL from the data attribute
    console.log("Document URL:", url); // Log the URL to the console for debugging
    $("#documentIframe").attr("src", url); // Set the src of the iframe
    $("#documentModal").modal("show"); // Show the modal
});

//isactive updation
function updateBeneficiaryStatus(beneficiaryId, isActive) {
    $.ajax({
        url: "/Client/UpdateBeneficiaryStatus",
        type: "POST",
        data: { Id: beneficiaryId, isActive: isActive },
        success: function (response) {
            if (response.success) {
                alert("Beneficiary status updated successfully");

                // Find the row containing the employee
                var beneficiaryRow = $(`#beneficiaryTblBody tr`).filter(function () {
                    return $(this).find(".is-active-checkbox").data("beneficiaryid") == beneficiaryId;
                });

                var editButton = beneficiaryRow.find(".editbeneficiary-btn-cell button");

                if (isActive) {
                    editButton.show();
                } else {
                    editButton.hide();
                }
            } else {
                alert("Error updating beneficiary status: " + response.message);
            }
        },
        error: function (err) {
            console.log("Error updating beneficiary status:", err);
        }
    });
}


//add new beneficiary

function addNewBeneficiary() {
    var formData = new FormData();

    // Append normal input fields
    formData.append("BeneficiaryName", $("#newBeneficiaryName").val());
    formData.append("AccountNumber", $("#newAccountNumber").val());
    formData.append("BankIFSC", $("#newBankIFSC").val());

    // Append file inputs
    var idProofFile = $("#BeneficiaryIdProof")[0].files[0]; // Access file input
    var addressProofFile = $("#BeneficiaryAddressProof")[0].files[0]; // Access file input

    formData.append("uploadedDocs1", idProofFile);
    formData.append("uploadedDocs2", addressProofFile);
    console.log(formData)
    $.ajax({
        url: "/Client/AddNewBeneficiary",
        type: "POST",
        data: formData,
        processData: false,  // Prevent jQuery from automatically transforming the data into a query string
        contentType: false,  // Prevent jQuery from setting Content-Type header; the browser will set it correctly
        success: function (response) {
            console.log(response)
            console.log(response.errors)
            if (response.success === false) {
                // Create an array to collect all error messages
                let errorMessages = [];

                // Iterate over each key in the errors object
                for (let key in response.errors) {
                    if (response.errors.hasOwnProperty(key)) {
                        errorMessages.push(`${key}: ${response.errors[key].join(", ")}`);
                    }
                }

                alert("Errors: " + errorMessages.join("\n"));
                console.log(response.errors);
                return;
            }

            alert("New Beneficiary added successfully");
            loadOutboundBeneficiaries();
            $("#addNewBeneficiary").hide();
            $("#beneficiaryList").show();
        },
        error: function (err) {
            alert("Error adding new Beneficiary");
            console.log(err);
        }
    });
}





//edit button onclick
function editBeneficiary(beneficiaryId) {
    getBeneficiary(beneficiaryId);
    $("#beneficiaryList").hide();
    $("#editBeneficiary").show();
}

function getBeneficiary(beneficiaryId) {
    $.ajax({
        url: "/Client/GetBeneficiaryById",
        type: "GET",
        data: { id: beneficiaryId },
        success: function (response) {
            console.log(response)
            if (response.success) {
                $("#editBeneficiaryId").val(response.beneficiary.Id);
                $("#editedBeneficiaryName").val(response.beneficiary.BeneficiaryName);
                $("#editedAccountNumber").val(response.beneficiary.AccountNumber);
                $("#editedBankIFSC").val(response.beneficiary.BankIFSC);
                //    console.log(beneficiary.BeneficiaryName)
            } else {
                alert(response.message);
            }
        },
        error: function (err) {
            alert("No such data found");
        }
    });
}

function modifyBeneficiary(formData) {
    $.ajax({
        url: "/Client/EditBeneficiary",
        type: "POST",
        data: formData,
        contentType: false,  // Prevent jQuery from overriding the content type
        processData: false,  // Prevent jQuery from processing the data
        success: function (response) {
            if (response.success === false) {
                // Create an array to collect all error messages
                let errorMessages = [];

                // Iterate over each key in the errors object
                for (let key in response.errors) {
                    if (response.errors.hasOwnProperty(key)) {
                        if (key !== "Id") {
                            errorMessages.push(`${key}: ${response.errors[key].join(", ")}`);
                        }
                    }
                }

                alert("Errors: " + errorMessages.join("\n"));
                console.log(response.errors);
                return;
            }
            alert("Beneficiary Details Edited Successfully");
            loadOutboundBeneficiaries();
            $("#beneficiaryList").show();
            $("#editBeneficiary").hide();

        },
        error: function (err) {
            alert("Error in Editing Beneficiary Details");
        }
    });
}
/*
function modifyBeneficiary(modifiedBeneficiary) {
    $.ajax({
        url: "/Client/EditBeneficiary",
        type: "POST",
        data: modifiedBeneficiary,
        success: function (response) {
            if (response.success) {
                alert("Beneficiary Details Edited Successfully");
                loadOutboundBeneficiaries();
                $("#beneficiaryList").show();
                $("#editBeneficiary").hide();
            } else {
                alert(response.message);
            }

        },
        error: function (err) {
            alert("Error in Editing Beneficiary Details");
        }
    });
}
*/