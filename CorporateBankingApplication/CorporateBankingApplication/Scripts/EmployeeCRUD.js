function LoadEmployees() {
    $.ajax({
        url: "/Client/GetAllEmployees",
        type: "GET",
        success: function (data) {
            /* console.log(data);*/
            $("#employeesTable").empty();

            if (data.length > 0) {
                $.each(data, function (index, employee) {
                    var row = `<tr>
                    <td>${employee.FirstName}</td>
                    <td>${employee.LastName}</td>
                    <td>${employee.Email}</td>
                    <td>${employee.Position}</td>
                    <td>${employee.Phone}</td>
                    
                     <td>
                         <input type="checkbox" class="is-active-checkbox"
                                data-employeeid="${employee.Id}"
                                ${employee.IsActive ? "checked" : ""} />
                     </td>

                   
                      <td class="edit-btn-cell">
                            <button onClick="editEmployee('${employee.Id}')" class="btn btn-success edit-btn"
                            style="${employee.IsActive ? '' : 'display:none;'}">Edit</button>
                      </td>

                    <td>
                    <input type="checkbox" class="is-SalaryDisbursed-checkbox"
                                data-employeeid="${employee.Id}"
                                ${employee.SalaryDisburseSelect ? "checked" : ""} />
                    </td>


                    </tr>`;
                    $("#employeesTable").append(row);
                });

                // Add event listener for all checkboxes
                $(".is-active-checkbox").change(function () {
                    var employeeId = $(this).data("employeeid");
                    var isActive = $(this).is(":checked");

                    // Call the server to update the contact status
                    updateEmployeeStatus(employeeId, isActive);
                });
            }
            else {
                $("#employeesTable").append("<tr><td colspan='5'>No employees found.</td></tr>");
            }
        },
        error: function (err) {
            console.log("Error fetching employees:", err);
        }
    });
}

function addNewEmployee() {
    var newEmployee = {
        FirstName: $("#newFName").val(),
        LastName: $("#newLName").val(),
        Email: $("#newEmail").val(),
        Position: $("#newPosition").val(),
        Phone: $("#newPhone").val()
    };

    $.ajax({
        url: "/Client/Add",
        type: "POST",
        data: newEmployee,
        success: function (response) {
            alert("New Employee added successfully");
            LoadEmployees();
            $("#newRecord").hide();
            $("#employeeList").show();
        },
        error: function (err) {
            alert("Error adding new employee");
            console.log(err);
        }
    })
}

function getEmployee(employeeId) {
    $.ajax({
        url: "/Client/GetEmployeeById",
        type: "GET",
        data: { id: employeeId },
        success: function (response) {
            if (response.success) {
                $("#editEmployeeId").val(response.employee.Id);
                $("#editFName").val(response.employee.FirstName);
                $("#editLName").val(response.employee.LastName);
                $("#editEmail").val(response.employee.Email);
                $("#editPosition").val(response.employee.Position);
                $("#editPhone").val(response.employee.Phone);
            } else {
                alert(response.message);
            }
        },
        error: function (err) {
            alert("No such data found");
        }
    });
}

function modifyRecord(modifiedEmployee) {
    $.ajax({
        url: "/Client/Edit",
        type: "POST",
        data: modifiedEmployee,
        success: function (response) {
            if (response.success) {
                alert("Employee Edited Successfully");
                LoadEmployees();
                $("#employeeList").show();
                $("#editEmployee").hide();
            } else {
                alert(response.message);
            }

        },
        error: function (err) {
            alert("Error Editing Employee Record");
        }
    });
}

function updateEmployeeStatus(employeeId, isActive) {
    $.ajax({
        url: "/Client/UpdateEmployeeStatus",
        type: "POST",
        data: { Id: employeeId, isActive: isActive },
        success: function (response) {
            if (response.success) {
                alert("Employee status updated successfully");

                // Find the row containing the employee
                var employeeRow = $(`#employeesTable tr`).filter(function () {
                    return $(this).find(".is-active-checkbox").data("employeeid") == employeeId;
                });

                // Find the edit button within the employee's row
                var editButton = employeeRow.find(".edit-btn-cell button");

                // Show or hide the button based on isActive status
                if (isActive) {
                    editButton.show();  // Show button if employee is active
                } else {
                    editButton.hide();  // Hide button if employee is inactive
                }
            } else {
                alert("Error updating employee status: " + response.message);
            }
        },
        error: function (err) {
            console.log("Error updating employee status:", err);
        }
    });
}

//showing the add form when Add New btn is clicked
$("#btnAdd").click(function () {
    $("#employeeList").hide();
    $("#newRecord").show();
});

//for edit
function editEmployee(employeeId) {
    getEmployee(employeeId);
    $("#employeeList").hide();
    $("#editRecord").show();
}

$("#btnEdit").click(() => {
    var data = {
        Id: $("#editEmployeeId").val(),
        FirstName: $("#editFName").val(),
        LastName: $("#editLName").val(),
        Email: $("#editEmail").val(),
        Position: $("#editPosition").val(),
        Phone: $("#editPhone").val()

    };
    modifyRecord(data);
});


/****************SALARY DISBURSEMENT**********************/

$("#disburseSalaryBtn").click(function () {
    var selectedEmployeeIds = [];
    var totalAmount = $("#salaryAmountInput").val();
    var clientId = $("#clientId").val();

    $(".is-SalaryDisbursed-checkbox:checked").each(function () {
        selectedEmployeeIds.push($(this).data("employeeid"));
    });

    if (selectedEmployeeIds.length == 0) {
        alert("Please select at least one employee for salary disbursement.");
        return;
    }

    if (!totalAmount || isNaN(totalAmount) || totalAmount <= 0) {
        alert("Please enter a valid salary amount.");
        return;
    }

    $.ajax({
        url: "/Client/DisburseSalaryBatch",
        type: "POST",
        traditional: true,
        data: {
            employeeIds: selectedEmployeeIds,
            totalAmount: totalAmount,
            clientId: clientId
        },

        success: function (response) {
            if (response.success) {
                alert(response.message);
                LoadEmployees(); // Reload the employee list
            } else {
                alert("Error: " + response.message);
            }
        },
        error: function (err) {
            alert("Error occurred while processing salary disbursement.");
        }
    });
});