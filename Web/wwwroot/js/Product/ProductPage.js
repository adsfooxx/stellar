//投影片
document.addEventListener('DOMContentLoaded', function() {
    const playerArea = document.getElementById('highlight_player_area');
    const mediaItems = document.querySelectorAll('.highlight-strip-row .col-3 img, .highlight-strip-row .col-3 video');

    //初始化
    if (mediaItems.length > 0) {
        
        const firstItem = mediaItems[0];
        firstItem.style.border = '2px solid white';

        const firstMediaSrc = firstItem.getAttribute('data-src');
        const initialMedia = firstMediaSrc.endsWith('.webm') || firstMediaSrc.endsWith('.mp4') ?
            `<video class="fade-in" autoplay muted controls>
                <source src="${firstMediaSrc}" type="video/webm">
                您的瀏覽器不支援 HTML5 影片。
            </video>` :
            `<img class="fade-in" src="${firstMediaSrc}" alt="" onclick="showModal(this)">
            <div id="modal" class="productmodal" onclick="hideModal()">
                <img id="modal-image" class="productmodal-image">
            </div>`;
            
        playerArea.innerHTML = initialMedia;

        const initialMediaElement = playerArea.querySelector('.fade-in');
        
        if (initialMediaElement) {
            initialMediaElement.classList.add('show');
        }
    }

    mediaItems.forEach(item => {
        item.addEventListener('click', function() {
            const mediaSrc = item.getAttribute('data-src');

            console.log('Clicked media source:', mediaSrc);  // 用來檢查是否正確抓到 data-src

            // 移除所有元素的邊框
            mediaItems.forEach(media => {
                media.style.border = 'none';
            });

            // 為當前點擊的元素添加白色邊框
            item.style.border = '2px solid white';

            // 創建一個新元素，應用淡入效果
            const newMedia = mediaSrc.endsWith('.webm') || mediaSrc.endsWith('.mp4') ?
                `<video class="fade-in" autoplay muted controls>
                    <source src="${mediaSrc}" type="video/webm">
                    您的瀏覽器不支援 HTML5 影片。
                </video>` :
                `<img class="fade-in" src="${mediaSrc}" alt="..." onclick="showModal(this)">
                <div id="modal" class="productmodal" onclick="hideModal()">
                    <img id="modal-image" class="productmodal-image">
                </div>`;

            // 清空 playerArea 的內容並插入新元素
            playerArea.innerHTML = newMedia;

            // 獲取插入的媒體元素，並添加 show 類來觸發淡入效果
            const newMediaElement = playerArea.querySelector('.fade-in');
            setTimeout(() => {
                if (newMediaElement) {
                    newMediaElement.classList.add('show');
                }
            }, 10); 
        });
    });
});

// 放大 Carousel 並顯示模態視窗
function showModal(image) {
    var modal = document.getElementById("modal");
    var modalImage = document.getElementById("modal-image");

    // 設置圖片的 src 為被點擊的圖片
    modalImage.src = image.src;

    // 顯示模態視窗
    modal.style.display = "flex"; // 使用 flex 來讓內容置中
}

function hideModal() {
    var modal = document.getElementById("modal");
    modal.style.display = "none";
}

//願望清單
//document.addEventListener('DOMContentLoaded', function() {
//    const wishListButton = document.querySelector('#add_to_wishlist_area .for_checked');

//    // Set initial button text
//    wishListButton.innerHTML = '<span>新增至您的願望清單</span>';

//    wishListButton.addEventListener('click', function() {
//        if (wishListButton.innerHTML.includes('新增至您的願望清單')) {
//            // Switch to "於願望清單內" with checkmark
//            wishListButton.innerHTML = '<span class="for_checked_box checked"></span><span>於願望清單內</span>';
//        } else {
//            // Revert to "新增至您的願望清單"
//            wishListButton.innerHTML = '<span>新增至您的願望清單</span>';
//        }
//    });
//});

// 語言
document.addEventListener('DOMContentLoaded', function() {
    function updateCollapseVisibility() {
        const collapseElement = document.getElementById('collapseWidthExample');
        
        if (window.innerWidth > 500) {
            collapseElement.classList.add('show');
        } else {
            collapseElement.classList.remove('show');
        }
    }

    updateCollapseVisibility();

    window.addEventListener('resize', updateCollapseVisibility);
});

//poppover
const popoverTriggerList = document.querySelectorAll('[data-bs-toggle="popover"]')
const popoverList = [...popoverTriggerList].map(popoverTriggerEl => new bootstrap.Popover(popoverTriggerEl))


document.addEventListener('DOMContentLoaded', function () {
    const allImages = document.querySelectorAll('img');
    let loadedImagesCount = 0;

    function checkAllImagesLoaded() {
        loadedImagesCount++;
        if (loadedImagesCount === allImages.length) {
            document.body.style.visibility = 'visible';
        }
    }

    if (allImages.length > 0) {
        allImages.forEach((img) => {
            if (img.complete) {
                // 圖片已經被緩存
                checkAllImagesLoaded();
            } else {
                // 圖片尚未加載完成
                img.addEventListener('load', checkAllImagesLoaded);
                img.addEventListener('error', checkAllImagesLoaded);
            }
        });
    } else {
        // 如果沒有圖片，直接顯示頁面
        document.body.style.visibility = 'visible';
    }
});
