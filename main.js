let toggler = document.getElementsByClassName("left-list");  
let body = document.getElementsByTagName("body");
let frame = document.getElementsByClassName("fa-solid");
let collapse = document.getElementsByClassName("collapse");

    for (let i = 0; i < toggler.length; i++){
        toggler[i].addEventListener(`click`, ()=> {
            // collapse[i].style.height = "200px";
            frame[i].classList.remove("fa-angle-down");
            frame[i].classList.add("fa-angle-up");
            collapse[i].classList.add("fade-collapse");
            // setTimeout(function(){collapse[i].classList.add("fade");}, 500); 
            })   
            document.addEventListener(`click`, e=> {
        if (!e.composedPath().includes(toggler[i]) && !e.composedPath().includes(collapse[i])){
            // collapse[i].style.height = "0";
            collapse[i].classList.remove("fade-collapse");
            frame[i].classList.add("fa-angle-down");
            frame[i].classList.remove("fa-angle-up");
            }
    })
    }

    // || e.composedPath().includes(toggler[i]) && collapse[i].classList.contains("fade-collapse")

    let searchBtn = document.querySelector(".search-btn");
    let searchBigger = document.querySelector(".search-bigger");

    searchBtn.addEventListener(`click`, ()=> {
        searchBigger.style.display = "flex";
        document.querySelector(".shadowed-background").classList.add("search-active");
    })

    document.addEventListener(`click`, e=> {
        if (!e.composedPath().includes(searchBtn) && !e.composedPath().includes(searchBigger)){
            searchBigger.style.display = "none";
            document.querySelector(".shadowed-background").classList.remove("search-active"); 
        }
    })


let slideIndex = 0;
showSlides();

function showSlides() {
  let slides = document.getElementsByClassName("mySlides");
  for (let i = 0; i < slides.length; i++) {
    slides[i].style.display = "none";  
  }
  slideIndex++;
  if (slideIndex > slides.length) {slideIndex = 1}    
  slides[slideIndex-1].style.display = "flex";  
  setTimeout(showSlides, 3000); 

}

//

let slides = document.getElementsByClassName("mySlides");
let boxes = document.getElementsByClassName("box");

for (let i = 0; i < boxes.length; i++) {
    boxes[i].addEventListener('click', () => {
        for (let i = 0; i < slides.length; i++) {
            slides[i].style.display = 'none';
        }
        slides[i].style.display = 'flex'; 
        slideIndex = i;
    });
}
