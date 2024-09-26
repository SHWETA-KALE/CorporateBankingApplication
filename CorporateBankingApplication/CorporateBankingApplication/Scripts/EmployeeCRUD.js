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
                    <td>
                        <button onClick="editEmployee('${employee.Id}')" class="btn btn-success">Edit</button>
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
        data:  { Id: employeeId, isActive: isActive },
        success: function (response) {
            if (response.success) {
                alert("Employee status updated successfully");
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