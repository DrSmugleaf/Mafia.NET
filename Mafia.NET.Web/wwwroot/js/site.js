// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {
    $('[data-toggle="tooltip"]').tooltip()
})

$('.keep_open').click(function(e) {
    const target = $(this).data('target');
    $(target).collapse('toggle');
});

$('.no-unclick').on('hide.bs.dropdown', function (e) {
    if (e.clickEvent) {
        e.stopPropagation();
        e.preventDefault();
    }
});