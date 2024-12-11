async function AjaxCall(url, data) {

    var Dropdown = JSON.stringify(data);
    var result;
    await $.ajax({
        type: "Post",
        url: url,
        timeout: 999999999,
        data: Dropdown,
        dataType: "Text",
        contentType: "application/json;charset=utf-8",
        cache: false,
        async: true,
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
        },
        success: function (responce) {
            if (responce == "-21") {
                window.location.href = "/SessionExpired";
            }
            else if (responce == "401") {
                window.location.href = "/Dashboard";
            }
            else {
                result = responce;
            }
            Swal.close();
        },
        error: function (error) {
            AlertTost("error", error);
            Swal.close();
        }
    });
    return result;
};


async function AjaxCallNoReturn(url, data) {
    var Dropdown = JSON.stringify(data);
    var msg;
    await $.ajax({
        type: "Post",
        url: url,
        timeout: 999999999,
        data: Dropdown,
        dataType: "Text",
        cache: false,
        async: true,
        contentType: "application/json;charset=utf-8",
        beforeSend: function () {
            Swal.fire({
                html: '<div class="loader" id="loader-7">' +
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
        },
        success: function (data) {
            if (data == "504") {
                window.location.href = "/SessionExpired";
            }
            else if (data == "401") {
                window.location.href = "/Dashboard";
            }
            else {
                msg = data;
            }
            Swal.close();


            $(".msg").find("p").remove();
            $(".msg").append(msg);
            Swal.close();
        },
        error: function (error) {
            AlertTost("error", error);
            Swal.close();
        }
    });
};

async function AjaxCallWithoutData(url) {
    var result;
    await $.ajax({
        type: "Get",
        url: url,
        dataType: "Text",
        cache: false,
        timeout: 999999999,
        async: true,
        contentType: "application/json;charset=utf-8",
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
        },
        success: function (responce) {
            if (responce == "-21") {
                window.location.href = "/SessionExpired";
            }
            else if (responce == "401") {
                window.location.href = "/Dashboard";
            }
            else {
                result = responce;
            }
            Swal.close();
        },
        error: function (error) {
            AlertTost("error", error);
            Swal.close();
        }
    });
    return result;
};



function AjaxCallNoAsync(url, data) {
    var Dropdown = JSON.stringify(data);
    var result;
    $.ajax({
        type: "Post",
        url: url,
        timeout: 999999999,
        data: Dropdown,
        dataType: "Text",
        contentType: "application/json;charset=utf-8",
        cache: false,
        async: false,
        beforeSend: function () {
            Swal.fire({
                html: '<div class="loader" id="loader-7">' +
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
        },
        success: function (responce) {
            if (responce == "-21") {
                window.location.href = "/SessionExpired";
            }
            else if (responce == "401") {
                window.location.href = "/Dashboard";
            }
            else {
                result = responce;
            }
            Swal.close();
        },
        error: function (error) {
            alert("Error:" + error);
            Swal.close();
        }
    });
    return result;
};


function AjaxCallNoReturnNoAsync(url, data) {
    var Dropdown = JSON.stringify(data);
    var msg;
    $.ajax({
        type: "Post",
        url: url,
        data: Dropdown,
        dataType: "Text",
        timeout: 999999999,
        cache: false,
        async: false,
        contentType: "application/json;charset=utf-8",
        beforeSend: function () {
            Swal.fire({
                html: '<div class="loader" id="loader-7">' +
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
        },
        success: function (data) {
            if (responce == "-21") {
                window.location.href = "/SessionExpired";
            }
            else if (responce == "401") {
                window.location.href = "/Dashboard";
            }
            else {
                msg = data;
            }


            $(".msg").find("p").remove();
            $(".msg").append(msg);
            Swal.close();
        },
        error: function (error) {
            alert("Error:" + error);
            Swal.close();
        }
    });
};

function AjaxCallWithoutDataNoAsync(url) {
    var result;
    $.ajax({
        type: "Post",
        url: url,
        timeout: 999999999,
        dataType: "Text",
        cache: false,
        async: false,
        contentType: "application/json;charset=utf-8",
        beforeSend: function () {
            Swal.fire({
                html: '<div class="loader" id="loader-7">' +
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
        },
        success: function (responce) {
            if (responce == "-21") {
                window.location.href = "/SessionExpired";
            }
            else if (responce == "401") {
                window.location.href = "/Dashboard";
            }
            else {
                result = responce;
            }

            Swal.close();
        },
        error: function (error) {
            alert("Error:" + error);
            Swal.close();
        }
    });
    return result;
};