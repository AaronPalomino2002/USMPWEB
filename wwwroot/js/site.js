// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
const menu = document.querySelector("#toggle-btn");
menu.addEventListener("click", function () {
  document.querySelector("#sidebar").classList.toggle("expand");
});

const currentPath = window.location.pathname;

const sidebarItems = document.querySelectorAll(".sidebar-item");

sidebarItems.forEach((item) => {
  const link = item.querySelector("a");
  if (link && link.getAttribute("href") === currentPath) {
    item.classList.add("active");
  } else {
    item.classList.remove("active");
  }
});

document.getElementById("dropdown-btn").addEventListener("click", function () {
  document.querySelector(".dropdown-container").classList.toggle("show");
});

window.onclick = function (event) {
  if (!event.target.matches("#dropdown-btn")) {
    var dropdowns = document.getElementsByClassName("dropdown-content");
    for (var i = 0; i < dropdowns.length; i++) {
      var openDropdown = dropdowns[i];
      if (openDropdown.style.display === "block") {
        openDropdown.style.display = "none";
      }
    }
  }
};
