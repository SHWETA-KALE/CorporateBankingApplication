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
                    <td>${employee.Salary}</td>
                   

                     <td>
                         <input type="checkbox" class="is-active-checkbox"
                                data-employeeid="${employee.Id}"
                                ${employee.IsActive ? "checked" : ""} />
                     </td>

                   
                    <td>
                    <input type="checkbox" class="is-SalaryDisbursed-checkbox"
                                data-employeeid="${employee.Id}"
                                data-salary="${employee.Salary}"
                                ${employee.SalaryDisburseSelect ? "checked" : ""} />
                    </td>

                     <td class="edit-btn-cell">
                            <button onClick="editEmployee('${employee.Id}')" class="btn btn-outline-dark edit-btn"
                            style="${employee.IsActive ? '' : 'display:none;'}">Edit</button>
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

                $(".is-SalaryDisbursed-checkbox").change(function () {
                    updateTotalSalary();
                });
                //Select all functionality 
                $("#selectAllSalaryDisbursement").change(function () {
                    var isChecked = $(this).is(":checked");
                    $(".is-SalaryDisbursed-checkbox").prop("checked", isChecked);
                    updateTotalSalary();
                });
                // Initialize total salary on page load
                updateTotalSalary();
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
        Phone: $("#newPhone").val(),
        Salary: $("#newSalary").val()
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
                $("#editSalary").val(response.employee.Salary);
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

                var editButton = employeeRow.find(".edit-btn-cell button");

                if (isActive) {
                    editButton.show();  
                } else {
                    editButton.hide();  
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
        Phone: $("#editPhone").val(),
        Salary: $("#editSalary").val()
    };
    modifyRecord(data);
});


//************** UPLOAD CSV *****************
$("#btnBulkUpload").click(function () {
    $("#csvFileInput").click();
});

$("#csvFileInput").change(function (event) {
    var formData = new FormData();
    var fileInput = event.target.files[0];
    formData.append("file", fileInput);

    $.ajax({
        url: "/Client/UploadCSV",
        type: "POST",
        contentType: false,
        processData: false,
        data: formData,
        success: function (response) {
            if (response.success) {
                alert("Employees uploaded successfully.");
                LoadEmployees(); 
            } else {
                alert("Error: " + response.message);
            }
        },
        error: function (err) {
            alert("Error uploading employees.");
            console.log(err);
        }
    });
});




/****************SALARY DISBURSEMENT**********************/

$('#disburseSalary').click(function () {

    var employeeIds = [];
   
    // Loop through all the checkboxes that are checked
    $('.is-SalaryDisbursed-checkbox:checked').each(function () {
        employeeIds.push($(this).data('employeeid')); 
    });
    var isBatch = employeeIds.length > 1;

    if (employeeIds.length > 0) {
        $.ajax({
            type: 'POST',
            url: '/Client/DisburseSalary',
           
            data: { employeeIds: employeeIds, isBatch: isBatch },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                   
                } else {
                    alert(response.message);
                }
            },
            error: function (xhr) {
                alert("Error: " + xhr.responseText);
            }
        });
    } else {
        alert("No employees selected for salary disbursement.");
    }
});

function updateTotalSalary() {
    let totalSalary = 0;
    $(".is-SalaryDisbursed-checkbox:checked").each(function () {
        let salary = parseFloat($(this).data("salary")); // Get the salary from the data attribute
        totalSalary += salary;
    });
    $("#salaryAmountInput").val(totalSalary.toFixed(2)); // Update the total salary
}