$(document).ready(function () {
    $("#grid").jqGrid({
        url: "/Admin/GetClientDetails",
        datatype: "json",
        colNames: ["Id", "Username", "Email", "Company Name", "Contact Information", "Location", "Balance", "Onboarding Status"],   //change column names
        colModel: [{ name: "Id", key: true, hidden: true },
        { name: "UserName", editable: true, search: false },
        { name: "Email", editable: true, search: false },
        { name: "CompanyName", editable: true, searchoptions: { sopt: ['eq'] } },
        { name: "ContactInformation", editable: true, search: false },
        { name: "Location", editable: true, search: false },
        { name: "Balance", editable: false, search: false },
        { name: "OnBoardingStatus", editable: false, search: false }],  //change model names
        height: "250",
        caption: "Client Information",
        pager: "#pager",
        rowNum: 5,
        rowList: [5, 10, 15],
        sortname: 'id',
        sortorder: 'asc',
        viewrecords: true,
        width: "900",
        gridComplete: function () {
            $("#grid").jqGrid('navGrid', '#pager', { edit: true, add: false, del: true, search: true, refresh: true },
                {
                    //edit
                    url: "/Admin/EditClientDetails",
                    closeAfterEdit: true,
                    width: 600,
                    afterSubmit: function (response, postdata) {
                        var result = JSON.parse(response.responseText);
                        if (result.success) {
                            alert(result.message);
                            return [true];
                        }
                        else {
                            alert(result.message);
                            return [false];
                        }
                    }
                },
                {
                    //add=false   
                },
                {
                    //delete
                    url: "/Admin/DeleteClientDetails",
                    afterSubmit: function (response, postdata) {
                        var result = JSON.parse(response.responseText);
                        if (result.success) {
                            alert(result.message);
                            return [true];
                        }
                        else {
                            alert(result.message);
                            return [false];
                        }
                    }
                },
                {
                    //search
                    multipleSearch: false,
                    closeAfterSearch: true
                }
            );
        }
    })
})