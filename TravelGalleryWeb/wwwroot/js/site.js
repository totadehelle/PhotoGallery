// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
var photos = document.querySelectorAll(".photoLink");
var frames = document.querySelectorAll(".carousel-item");

photos.forEach(function (photo) {
    photo.addEventListener("click", function(){
        frames.forEach(function (frame) {
            frame.classList.remove("active");
        });
        var id = photo.id+"f";
        var frame = document.getElementById(id);
        frame.classList.add("active");
    });
});