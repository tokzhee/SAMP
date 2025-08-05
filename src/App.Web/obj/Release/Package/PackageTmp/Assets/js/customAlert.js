(function ($) {
    alertMessage = function (salutation, message, type) {
        "use strict";
        if (type === "info") {
            swal({
                title: salutation,
                text: message,
                icon: "info",
                button: {
                    text: "Continue",
                    value: true,
                    visible: true,
                    className: "btn btn-info btn-xs"
                }
            });
        }
        else if (type === "success") {
            swal({
                title: salutation,
                text: message,
                icon: "success",
                button: {
                    text: "Continue",
                    value: true,
                    visible: true,
                    className: "btn btn-success btn-xs"
                }
            });
        }
        else if (type === "warning") {
            swal({
                title: salutation,
                text: message,
                icon: "warning",
                button: {
                    text: "Continue",
                    value: true,
                    visible: true,
                    className: "btn btn-warning btn-xs"
                }
            });
        }
        else if (type === "error") {
            swal({
                title: salutation,
                text: message,
                icon: "error",
                button: {
                    text: "Continue",
                    value: true,
                    visible: true,
                    className: "btn btn-danger btn-xs"
                }
            });
        }
    }
})(jQuery);