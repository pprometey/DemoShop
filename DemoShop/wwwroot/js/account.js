/*
Accouts Layout js
*/
(function () {
    'use strict';

    function changeCapthaSize() {
        var reCaptchaWidth = 302;
        var containerWidth = $('.container-recaptcha').width();
        var reCaptchaScale = containerWidth / reCaptchaWidth;

        if (reCaptchaWidth > containerWidth) {
            $('.g-recaptcha').css({
                'transform': 'scale(' + reCaptchaScale + ')',
                'transform-origin': 'left top'
            });
        }
    }

    $(window).ready(function () {
        changeCapthaSize();
    });
    $(window).resize(function () {
        changeCapthaSize();
    });
}());
