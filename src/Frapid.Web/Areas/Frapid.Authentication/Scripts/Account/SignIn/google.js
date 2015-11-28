﻿function googleSignIn(id, email, name, token) {
    $("#SignInSegment").addClass("loading");

    function request() {
        var url = "/account/google/sign-in";

        var loginDetails = {
            Email: email,
            Name: name,
            Token: token
        };

        var data = JSON.stringify(loginDetails);

        return window.getAjaxRequest(url, "POST", data);
    };

    var ajax = request();

    ajax.success(function (response) {
        if (response.Status) {
            window.location = "/dashboard";
        } else {
            alert("Sorry, google registration is not allowed at this time.");
        };
    });

    ajax.fail(function () {
        $("#SignInSegment").removeClass("loading");
    });
};

function onSignIn(googleUser) {
    var request = googleUser.getBasicProfile();
    var id = request.getId();
    var email = request.getEmail();
    var name = request.getName();
    var token = googleUser.getAuthResponse().id_token;

    googleSignIn(id, email, name, token);
};
(function () {
    var po = document.createElement('script');
    po.type = 'text/javascript'; po.async = true;
    po.src = 'https://apis.google.com/js/client:plusone.js?onload=render';
    var s = document.getElementsByTagName('script')[0];
    s.parentNode.insertBefore(po, s);
})();