// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let height;
const header;

const Init = () => {
    height =  = window.innerHeight;
    header = document.getElementsByTagName("header")
    HeaderResize();
}
const HeaderResize = () => {
    header.innerHeight = height;
    console.log("Resized");
}
window.onresize = HeaderResize;
window.addEventListener("DOMContentLoaded", Init);