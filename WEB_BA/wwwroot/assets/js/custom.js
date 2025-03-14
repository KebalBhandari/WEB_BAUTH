﻿function AlertMessage(messagetype, message) {

    $("#AlertMessage").empty();
    var cssclass;
    
    switch (messagetype) {
        case 'Success':
            cssclass = 'alert-success';
            break;
        case 'Error':
            cssclass = 'alert-danger';
            break;
        case 'Warning':
            cssclass = 'alert-warning';
            break;
        default:
            cssclass = 'alert-info';
    }
    $("#AlertMessage").append('<div id="alert_div" style="margin: 0 0.5%; -webkit-box-shadow: 3px 4px 6px #999;" class="alert fade in ' + cssclass + '"><a href="#" class="close" data-dismiss="alert" aria-label="close" style="text-decoration: none;">&times;</a><strong>' + messagetype + ' !</strong><span> ' + message + '</span></div>');
    Alert_fadeto();
};


function Alert_fadeto() {
    $("#alert_div").fadeTo("slow", 10).fadeOut(5000, function () {
        $("#alert_div").remove();
    });
};

async function AlertTost(messagetype, message) {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toastr-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "4000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };

    switch (messagetype) {
        case 'error':
            toastr.error(message);
            break;
        case 'success':
            toastr.success(message);
            break;
        case 'warning':
            toastr.warning(message);
            break;
        case 'info':
            toastr.info(message);
            break;
        default:
        //    toastr.info(message)
    }
}

function fnExcelParmReport(TableToExport, profile, parameter) {
    var tab_text = '<table border="0.5px">';
    var textRange;
    var j = 0;
    var lines = TableToExport.rows.length;
    var columncount = TableToExport.rows[0].cells.length;
    if (lines > 0) {
        tab_text = tab_text + "<tr> <td align=left  colspan=" + columncount + " > " + profile + "<br/>" + parameter + "</td></tr>";
        tab_text = tab_text + '<tr>' + TableToExport.rows[0].innerHTML + '</tr>';
    }
    for (j = 1; j < lines; j++) {
        tab_text = tab_text + "<tr>" + TableToExport.rows[j].innerHTML + "</tr>";
    }
    tab_text = tab_text + "</table>";
    tab_text = tab_text.replace(/<A[^>]*>|<\/A>/g, "")
    tab_text = tab_text.replace(/<img[^>]*>/gi, "");
    tab_text = tab_text.replace(/<input[^>]*>|<\/input>/gi, "");
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        txtArea1.document.open("txt/html", "replace");
        txtArea1.document.write(tab_text);
        txtArea1.document.close();
        txtArea1.focus();
        sa = txtArea1.document.execCommand("SaveAs", true, "DataTableExport.xls");
    }
    else
        sa = window.open('data:application/vnd.ms-excel,' + encodeURIComponent(tab_text));
    return (sa);
}


function fnExcelReport(TableToExport) {
    var tab_text = '<table border="1px" style="font-size:20px" ">';
    var textRange;
    var j = 0;
    var lines = TableToExport.rows.length;
    if (lines > 0) {
        tab_text = tab_text + '<tr bgcolor="#DFDFDF">' + TableToExport.rows[0].innerHTML + '</tr>';
    }
    for (j = 1; j < lines; j++) {
        tab_text = tab_text + "<tr>" + TableToExport.rows[j].innerHTML + "</tr>";
    }

    tab_text = tab_text + "</table>";
    tab_text = tab_text.replace(/<A[^>]*>|<\/A>/g, "")
    tab_text = tab_text.replace(/<img[^>]*>/gi, "");
    tab_text = tab_text.replace(/<input[^>]*>|<\/input>/gi, "");
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        txtArea1.document.open("txt/html", "replace");
        txtArea1.document.write(tab_text);
        txtArea1.document.close();
        txtArea1.focus();
        sa = txtArea1.document.execCommand("SaveAs", true, "DataTableExport.xls");
    }
    else
        sa = window.open('data:application/vnd.ms-excel,' + encodeURIComponent(tab_text));
    return (sa);
}


function AlertAnimation() {
    $("#alert_div").fadeTo("fast", 100).slideUp(2000, function () {
        $("#alert_div").css("display", "none")
        $(".Error").css("display", "none");
        $(".Success").css("display", "none");
        $(".Warning").css("display", "none");
        $(".TokenNotFound").css("display", "none");
    });
}


function AjaxAlertMessage(AffectedRows) {
    $(".AlertMessage").empty();
    var MessageHtml = `
<!--Success-->
<div id="Notification" class="messagealert Success1" style="display:none">
	<div id="alert_div" style="margin: 0 0.5%; -webkit-box-shadow: 3px 4px 6px #999;" class="alert fade in Success">
		<a href="#" class="close" data-dismiss="alert" aria-label="close" style="text-decoration: none;">&times;</a>
		<strong>Success!</strong> <span>Saved Successfully !!!</span>
	</div>
</div>
<!--Error-->
<div id="Notification" class="messagealert Error1" style="display:none">
	<div id="alert_div" style="margin: 0 0.5%; -webkit-box-shadow: 3px 4px 6px #999;" class="alert fade in Error">
		<a href="#" class="close" data-dismiss="alert" aria-label="close" style="text-decoration: none;">&times;</a>
		<strong>Error!</strong> <span>Not Success !!!</span>
	</div>
</div>
<!--Unauthorized-->
<div id="Notification" class="messagealert Warning1" style="display:none">
	<div id="alert_div" style="margin: 0 0.5%; -webkit-box-shadow: 3px 4px 6px #999;" class="alert fade in Warning">
		<a href="#" class="close" data-dismiss="alert" aria-label="close" style="text-decoration: none;">&times;</a>
		<strong>Warning!</strong> <span>Sorry You Are Not authorized!!!</span>
	</div>
</div>
<!--Token Not Found-->
<div id="Notification" class="messagealert TokenNotFound" style="display:none">
	<div id="alert_div" style="margin: 0 0.5%; -webkit-box-shadow: 3px 4px 6px #999;" class="alert fade in Error">
		<a href="#" class="close" data-dismiss="alert" aria-label="close" style="text-decoration: none;">&times;</a>
		<strong>Warning!</strong> <span>Token Not Found!!!Please Sign In Again....</span>
	</div>
</div>
`;
    $(".AlertMessage").append(MessageHtml);
    if (AffectedRows == "Success") {
        $(".Success1").css("display", "Block");
        AlertAnimation();
    }
    if (AffectedRows == "NotSuccess") {
        $(".Error1").css("display", "Block");
        AlertAnimation();
    }
    if (AffectedRows == "Unauthorized") {
        $(".Warning1").css("display", "Block");
        AlertAnimation();
    }
    if (AffectedRows == "TokenNotFound") {
        $(".TokenNotFound").css("display", "Block");
        AlertAnimation();
    }
}


function AlertAnimation() {
    $("#alert_div").fadeTo("fast", 100).slideUp(2000, function () {
        $("#alert_div").css("display", "none")
        $(".Error").css("display", "none");
        $(".Success").css("display", "none");
        $(".Warning").css("display", "none");
        $(".TokenNotFound").css("display", "none");
    });
}


function AjaxAlertMessage(AffectedRows) {
    $(".AlertMessage").empty();
    var MessageHtml = `
<!--Success-->
<div id="Notification" class="messagealert Success1" style="display:none">
	<div id="alert_div" style="margin: 0 0.5%; -webkit-box-shadow: 3px 4px 6px #999;" class="alert fade in Success">
		<a href="#" class="close" data-dismiss="alert" aria-label="close" style="text-decoration: none;">&times;</a>
		<strong>Success!</strong> <span>Saved Successfully !!!</span>
	</div>
</div>
<!--Error-->
<div id="Notification" class="messagealert Error1" style="display:none">
	<div id="alert_div" style="margin: 0 0.5%; -webkit-box-shadow: 3px 4px 6px #999;" class="alert fade in Error">
		<a href="#" class="close" data-dismiss="alert" aria-label="close" style="text-decoration: none;">&times;</a>
		<strong>Error!</strong> <span>Not Success !!!</span>
	</div>
</div>
<!--Unauthorized-->
<div id="Notification" class="messagealert Warning1" style="display:none">
	<div id="alert_div" style="margin: 0 0.5%; -webkit-box-shadow: 3px 4px 6px #999;" class="alert fade in Warning">
		<a href="#" class="close" data-dismiss="alert" aria-label="close" style="text-decoration: none;">&times;</a>
		<strong>Warning!</strong> <span>Sorry You Are Not authorized!!!</span>
	</div>
</div>
<!--Token Not Found-->
<div id="Notification" class="messagealert TokenNotFound" style="display:none">
	<div id="alert_div" style="margin: 0 0.5%; -webkit-box-shadow: 3px 4px 6px #999;" class="alert fade in Error">
		<a href="#" class="close" data-dismiss="alert" aria-label="close" style="text-decoration: none;">&times;</a>
		<strong>Warning!</strong> <span>Token Not Found!!!Please Sign In Again....</span>
	</div>
</div>
`;
    $(".AlertMessage").append(MessageHtml);
    if (AffectedRows == "Success") {
        $(".Success1").css("display", "Block");
        AlertAnimation();
    }
    if (AffectedRows == "NotSuccess") {
        $(".Error1").css("display", "Block");
        AlertAnimation();
    }
    if (AffectedRows == "Unauthorized") {
        $(".Warning1").css("display", "Block");
        AlertAnimation();
    }
    if (AffectedRows == "TokenNotFound") {
        $(".TokenNotFound").css("display", "Block");
        AlertAnimation();
    }
}



function DisableInput() {
    $("body").find("input").attr('disabled', true);
    $("body").find("submit").attr('disabled', true);
    $("body").find("select").attr('disabled', true);
    $("body").find("TxtQuestion").attr('disabled', true);
    $("body").find("button").not(".BtnAdd ").attr('disabled', true);
    $("body").find("textarea").attr('disabled', true);
}

function EnableInput() {

    $("body").find("input").removeAttr('disabled');
    $("body").find("select").removeAttr('disabled');
    $("body").find("textarea").removeAttr('disabled');
    $("body").find("submit").removeAttr('disabled');
    $("body").find("textarea").removeAttr('disabled');
    $("body").find("button").not(".BtnAdd").removeAttr('disabled');
}



async function AjaxCallFileUpload(FileDom, FolderName) {
    var files = FileDom[0].files;
    var formData = new FormData();
    for (var i = 0; i != files.length; i++) {
        formData.append("files", files[i]);
    }
    await $.ajax({
        url: "/Helper/FileUpload?FolderName=" + FolderName + "",
        data: formData,
        processData: false,
        contentType: false,
        timeout: 999999999,
        dataType: "Text",
        type: "POST",
        beforeSend: function () {
            Swal.fire({
                html: '<div class="loader" id="loader-6">' +
                    '<span></span>' +
                    '<span></span>' +
                    '<span></span>' +
                    '<span></span>' +
                    '</div>' +
                    '<div>' +
                    '<p style="color:#fff; font-size:20px;">PLEASE WAIT...</p>' +
                    '</div>',
                background: 'unset',
                allowOutsideClick: false,
                showConfirmButton: false,
            });
            DisableInput();
        },
        success: function (data) {
            $(FileDom).attr("data-filename", data);
            $(FileDom).attr('value', data);
            EnableInput();
            Swal.close();
        },
        error: function () {
            alert("Error:" + error);
            EnableInput();
            Swal.close();
        }
    });
}








