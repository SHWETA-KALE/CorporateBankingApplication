function loadClientsForVerification() {
    $.ajax({
        url: "/Admin/GetClientsForVerification",
        type: "GET",
        success: function (data) {
            $("#clientToBeVerifiedTblBody").empty()
            //added for checking if there are any clients or not
            if (data.length === 0) {
                alert("No Clients Waiting To Be Verified");
                window.location.href = '/Admin/AdminDashboard';
                return;
            }
            
            $.each(data, function (index, item) {
                
                // Create a list of document links
                var documents = item.DocumentPaths.map(function (docPath) {
                    var fileName = docPath.split('/').pop(); // Extract file name from path
                   // return `<a href="${docPath}" target="_blank">${fileName}</a><br>`; // Use URL for document path
                    return `<a href="#" class="open-document" data-filepath="${docPath}" target="_blank">${fileName}</a><br>`;}).join('');

                var row = `<tr>
                <td>${item.UserName}</td>
                <td>${item.Email}</td>
                <td>${item.CompanyName}</td>
                <td>${item.ContactInformation}</td>
                <td>${item.Location}</td>
                <td>${documents}</td>
                <td><button onclick="approveClient('${item.Id}', 'APPROVED')" class="btn btn-outline-dark">Approve</button></td>
                <td><button onclick="rejectClient('${item.Id}', 'REJECTED')" class="btn btn-outline-dark">Reject</button></td>
                </tr>`
                $("#clientToBeVerifiedTblBody").append(row)
            })
            // Add click event to open modal and display document
            $(".open-document").click(function (e) {
                e.preventDefault();
                var filePath = $(this).data('filepath');

                // Load the document in the iframe
                $('#documentFrame').attr('src', filePath);

                // Show the modal
                $('#documentModal').modal('show');
            });
        },

        error: function (err) {
            $("#clientToBeVerifiedTblBody").empty()
            alert("No Clients Waiting To Be Verified")
        }

    })
}
//document modal closing button
$('.close').on('click', function () {
    $('#documentModal').modal('hide');
});

// Use the same function for both approve and reject buttons
function approveClient(clientId) {
    updateClientStatus(clientId, 'APPROVED');
}

function rejectClient(clientId) {
    updateClientStatus(clientId, 'REJECTED');
}
// Combined function for approving or rejecting client
function updateClientStatus(clientId, status) {
    $.ajax({
        url: "/Admin/UpdateClientOnboardingStatus",
        type: "POST",
        data: { id: clientId, status: status }, // Pass the status (APPROVED or REJECTED)
        success: function () {
            alert(`Client ${status === 'APPROVED' ? 'Approved' : 'Rejected'}`);
            loadClientsForVerification();
        },
        error: function () {
            alert(`Failed to ${status === 'APPROVED' ? 'Approve' : 'Reject'} the Client`);
        }
    });
}




