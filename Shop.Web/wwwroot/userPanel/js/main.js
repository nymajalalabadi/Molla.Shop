(function($) {
    "use strict";
    jQuery(document).on('ready', function() {
        /* Header Sticky
        ========================================================*/
        $(window).on('scroll', function() {
            if ($(this).scrollTop() > 70) {
                $('.header-sticky').addClass("is-sticky");
            } else {
                $('.header-sticky').removeClass("is-sticky");
            }
        });

        // Nav Active Code
        /*==============================================================*/
        if ($.fn.classyNav) {
            $('#EduStudyNav').classyNav({
                theme: 'light'
            });
        };

        // Search Popup Js
        /*==============================================================*/
        $(function() {
            $('a[href="#search"]').on("click", function(event) {
                event.preventDefault();
                $("#search-area").addClass("open");
                $('#search-area > form > input[type="search"]').focus();
            });

            $("#search-area, #search-area button.close").on("click keyup", function(event) {
                if (
                    event.target == this ||
                    event.target.className == "close" ||
                    event.keyCode == 27
                ) {
                    $(this).removeClass("open");
                }
            });

            $("form").on("submit", function(event) {
                event.preventDefault();
                return false;
            });
        });

        /* Home Slides
        ========================================================*/
        $('.home-slides').owlCarousel({
            items: 1,
            loop: true,
            autoplay: true,
            nav: true,
            rtl: true,
            animateOut: 'fadeOut',
            animateIn: 'fadeInUp',
            responsiveClass: true,
            dots: false,
            autoplayHoverPause: true,
            mouseDrag: true,
            navText: [
                "<i class='icofont-rounded-left'></i>",
                "<i class='icofont-rounded-right'></i>"
            ],
        });

        /* Home Slides Two
        ========================================================*/
        $('.home-slides-two').owlCarousel({
            items: 1,
            loop: true,
            autoplay: true,
            nav: true,
            rtl: true,
            animateOut: 'fadeOut',
            responsiveClass: true,
            dots: false,
            autoplayHoverPause: true,
            mouseDrag: true,
            navText: [
                "<i class='icofont-rounded-left'></i>",
                "<i class='icofont-rounded-right'></i>"
            ],
        });

        /* Popup Video
        ========================================================*/
        $('.popup-video').magnificPopup({
            disableOn: 320,
            type: 'iframe',
            mainClass: 'mfp-fade',
            removalDelay: 160,
            preloader: false,
            fixedContentPos: false
        });

        // Counter
        /*==============================================================*/
        $(".count").counterUp({
            delay: 20,
            time: 1500
        });


        // News Slides
        /*==============================================================*/
        $(".news-slider").owlCarousel({
            nav: true,
            dots: false,
            center: false,
            touchDrag: false,
            rtl: true,
            mouseDrag: true,
            autoplay: true,
            smartSpeed: 750,
            autoplayHoverPause: true,
            loop: true,
            navText: [
                "<i class='icofont-thin-right'></i>",
                "<i class='icofont-thin-left'></i>"
            ],
            responsive: {
                0: {
                    items: 1,
                },
                768: {
                    items: 2,
                },
                1200: {
                    items: 3,
                }
            }
        });

        // Partner Slides
        /*==============================================================*/
        $(".partner-slider").owlCarousel({
            nav: false,
            dots: false,
            mouseDrag: true,
            autoplay: true,
            rtl: true,
            smartSpeed: 750,
            autoplayHoverPause: true,
            loop: true,
            responsive: {
                0: {
                    items: 1,
                },
                768: {
                    items: 3,
                },
                1200: {
                    items: 6,
                }
            }
        });

        // Testimonials Slides
        /*==============================================================*/
        $(".testimonials-slider").owlCarousel({
            nav: true,
            dots: false,
            center: true,
            touchDrag: false,
            items: 4,
            rtl: true,
            mouseDrag: true,
            autoplay: true,
            smartSpeed: 750,
            autoplayHoverPause: true,
            loop: true,
            navText: [
                "<i class='icofont-rounded-left'></i>",
                "<i class='icofont-rounded-right'></i>"
            ],
            responsive: {
                0: {
                    items: 1,
                },
                768: {
                    items: 2,
                }
            }
        });

        /* About Slides
        ========================================================*/
        $('.about-slider').owlCarousel({
            items: 1,
            loop: true,
            autoplay: true,
            nav: true,
            responsiveClass: true,
            dots: false,
            rtl: true,
            autoplayHoverPause: true,
            mouseDrag: true,
            navText: [
                "<i class='icofont-rounded-left'></i>",
                "<i class='icofont-rounded-right'></i>"
            ],
        });

        /* Tooltip
        ========================================================*/
        $(function() {
            $('[data-toggle="tooltip"]').tooltip()
        });

        // Course Tabs
        /*==============================================================*/
        $("#tabs li").on("click", function() {
            var tab = $(this).attr("id");
            if ($(this).hasClass("inactive")) {
                $(this).removeClass("inactive");
                $(this).addClass("active");
                $(this).siblings().removeClass("active").addClass("inactive");
                $("#" + tab + "_content").addClass("show");
                $("#" + tab + "_content").siblings("div").removeClass("show");
            }
        });

        // Scroll Top link
        /*==============================================================*/
        $(window).on('scroll', function() {
            if ($(this).scrollTop() > 300) {
                $('.scrolltop').fadeIn();
            } else {
                $('.scrolltop').fadeOut();
            }
        });
        $('.scrolltop').on('click', function() {
            $("html, body").animate({
                scrollTop: 0
            }, 1000);
            return false;
        });
        $('.scroll-down a, .slide-inner-content a, .cta-area a').on('click', function(e) {
            var anchor = $(this);
            $('html, body').stop().animate({
                scrollTop: $(anchor.attr('href')).offset().top - 70
            }, 1500);
            e.preventDefault();
        });
    });

    /* Preloader
    ========================================================*/
    jQuery(window).on('load', function() {
        $('.preloader-area').fadeOut();
    });


}(jQuery));
$(function() {

    $('.main-menu nav > span').click(function() {
        $('.main-menu nav > ul').slideToggle();
    });

});

$(document).load($(window).bind("resize", checkPosition));

function checkPosition() {
    if (window.matchMedia('(max-width: 991px)').matches) {
        $('.main-menu nav > ul').css('display', 'none');
    } else {
        $('.main-menu nav > ul').css('display', 'block');
    }
}

function checkMenu() {
    var windowSize = $(window).width();
    if (windowSize <= 991) {
        $(".caret-icon").remove();
        $('.main-menu nav > ul > li').each(function() {
            if ($(this).find('ul').length > 0) {
                $(this).append("<span class='caret-icon'><i class='zmdi zmdi-chevron-down'></i></span>");
            }
        });
        $('.main-menu nav > ul > li > .caret-icon').click(function() {
            $(this).parent().find('ul').slideToggle();
        });

    } else {

        $('.main-menu nav > ul > li').hover(
            function() {
                $(this).find('ul').fadeIn('fast');
            },
            function() {
                $(this).find('ul').fadeOut('fast');
            }
        );

    }
}

checkMenu();

$(window).resize(function(event) {
    event.preventDefault();
    checkMenu();
});