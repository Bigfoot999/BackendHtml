
function generateRecipe() {
  alert("Hãy thư giản và để bếp trưởng 5 sao michelin tạo ra một công thức nấu ăn cho bạn!");
}


let topBtn = document.getElementById("topBtn");
window.onscroll = function () {
  topBtn.style.display = document.documentElement.scrollTop > 100 ? "block" : "none";
};
function topFunction() {
  document.documentElement.scrollTop = 0;
}
