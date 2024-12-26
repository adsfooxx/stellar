
var { ref, onMounted, computed, onBeforeUnmount, watch, onUnmounted } = Vue;
const App = {
    setup() {
        const menuVisible = ref(false);




        const isDragAccordion = ref(false);
        const sortOrder = ref('依照條件排序');
        const isAccordionOpen = ref({
            price: true,
            tags: true,
            categorys: true,
        });


        const currentPage = ref(1);
        const isLoading = ref(false);
        const totalCount = ref(0);
        const queryTag = ref('');

        const showDiscounted = ref(false);
        const hideFreeItems = ref(false);



        const getQueryParam = (param) => {
            const urlParams = new URLSearchParams(window.location.search);
            return urlParams.get(param);
        };




        const min = 0;
        const max = 10;
   

        const defaultMax = 10000;
   
        const priceMultiplier = 300;
      
        const typeBy = ref(getQueryParam('typeBy'));
        const categoryId = ref(getQueryParam('categoryId'));
        const tagId = ref(getQueryParam('tagId'));
        const categoryName = ref(getQueryParam('categoryName'));
        const tagName = ref(getQueryParam('tagName'));
        const query = ref(getQueryParam('query'));

        const minPrice = ref(getQueryParam('minPrice') || Number.parseInt(min));
        const maxPrice = ref(getQueryParam('maxPrice') || Number.parseInt(max));
        const priceValue = ref([min, max]);
        const isZero = ref(priceValue.value[1] === 0); 

        watch(priceValue, (newValue) => {
            isZero.value = newValue[1] === 0;
        });


        const selectedCategories = ref([]);
        const selectedTags = ref([]);
        const searchQuery = ref('');
        const tags = ref([]);
        const categorys = ref([]);
        const products = ref([]);

       
     
        const priceRange = computed(() => {
            if (isAnyPrice.value) {
                return [0, 0];
            }
            return [priceValue.value[0] * priceMultiplier, priceValue.value[1] * priceMultiplier];
        });


        const formattedPriceRange = computed(() => {
            const [min, max] = priceRange.value;
            return `${formatPrice(min)} - ${formatPrice(max)}`;
        });
        const formatPrice = (price) => {
            return price.toLocaleString('en-US', {
                style: 'currency', currency: 'TWD',
                minimumFractionDigits: 0,
                maximumFractionDigits: 0
            });
        };
        const isAnyPrice = computed(() => {
            return priceValue.value[0] === min && priceValue.value[1] === max;
        });



        const handleSliderChange = async (newVal) => {


           
                let [minVal, maxVal] = newVal;

             
                if (minVal > maxVal) {
                    minVal = maxVal; 
                }

            
                priceValue.value = [minVal, maxVal];

             
               
         

            const selectedCategoryIdsString = selectedCategories.value.map(category => category.categoryId).join(', ');
            const selectedCategoryNameString = selectedCategories.value.map(category => category.categoryName).join(', ');

            const selectedTagIdsString = selectedTags.value.map(tag => tag.tagId).join(', ');
            const selectedTagNameString = selectedTags.value.map(tag => tag.tagName).join(', ');


            fetchProductsBy(query.value, minVal, maxVal, selectedCategoryIdsString, selectedTagIdsString, selectedCategoryNameString, selectedTagNameString, typeBy.value, 1);



        };



        const dynamicTitle = computed(() => {

            if (typeBy.value === 'free') return '免費遊玩';
            if (typeBy.value === 'highest') return '暢銷商品';
            if (typeBy.value === 'Discount') { showDiscounted.value = true; return '特別優惠'; }
            if (typeBy.value === 'New') return '新推出';
            if (typeBy.value === 'Soon') return '即將發行';
            else return null;
        });




        const localUrl = "https://localhost:7168";


        const clearRoutingParameters = () => {
            const url = new URL(window.location);

            url.searchParams.delete('query');
            url.searchParams.delete('typeBy');
            url.searchParams.delete('categoryId');
            url.searchParams.delete('tagId');
            url.searchParams.delete('categoryName');
            url.searchParams.delete('tagName');


            window.history.replaceState({}, '', url);
        };


        const checkPrice = (minPrice, maxPrice) => {
            const url = new URL(window.location);
            url.searchParams.delete('minPrice');
            url.searchParams.delete('maxPrice');
            if (minPrice > min) url.searchParams.set('minPrice', minPrice);
            if (maxPrice < defaultMax) url.searchParams.set('maxPrice', maxPrice);
            window.history.pushState({}, '', url);
         
        }

        const fetchProducts = async (page = 1, pageSize = 10, minPrice = 0, maxPrice = 0) => {
            const endpoint = `/api/ProductSearchAPI/getProduct/page/${page}/pageSize/${pageSize}/min/${minPrice}/max/${maxPrice}`;
       
            clearRoutingParameters();
            return await handleFetch(endpoint);
        };


        const fetchProductsByQuery = async (query, minPrice = 0, maxPrice = 0) => {

            const endpoint = `/api/ProductSearchAPI/query/${encodeURIComponent(query)}/min/${minPrice}/max/${maxPrice}`;
            const url = new URL(window.location);
            url.searchParams.set('query', query);
      
            window.history.pushState({}, '', url);       
            return await handleFetch(endpoint);
        };
        const fetchProductsByCategoryOrTag = async (categoryId, tagId, categoryName, tagName, minPrice, maxPrice) => {


            const categoryIds = categoryId ? categoryId.split(/[, ]+/).join(',') : '0';
            const tagIds = tagId ? tagId.split(/[, ]+/).join(',') : '0';
            const categoryNames = categoryName ? categoryName.split(/[, ]+/).join(',') : 'none';
            const tagNames = tagName ? tagName.split(/[, ]+/).join(',') : 'none';

            const endpoint = `/api/ProductSearchAPI/CategoryOrTag/categoryIds/${categoryIds}/tagIds/${tagIds}/categoryNames/${encodeURIComponent(categoryNames)}/tagNames/${encodeURIComponent(tagNames)}/min/${minPrice}/max/${maxPrice}`;
            query.value = "";
            queryTag.value = "";
            typeBy.value = "";

            const url = new URL(window.location);
            url.searchParams.delete('query');
            url.searchParams.delete('typeBy');
          
          


            if (categoryNames === 'none') { url.searchParams.delete('categoryName'); }
            else { url.searchParams.set('categoryName', categoryNames); }

            if (tagNames === 'none') { url.searchParams.delete('tagName'); }
            else { url.searchParams.set('tagName', tagNames); }


            window.history.pushState({}, '', url);
     

            return await handleFetch(endpoint);
        };

        const fetchProductsByType = async (typeBy, minPrice, maxPrice) => {
            const endpoint = `/api/ProductSearchAPI/typeBy/${typeBy}/min/${minPrice}/max/${maxPrice}`;
         
            return await handleFetch(endpoint);
        };


     
        const handleFetch = async (endpoint) => {
            try {
                isLoading.value = true;
                const response = await axios.get(endpoint);
                return response.data;
            } catch (error) {
                return null;
            } finally {
                isLoading.value = false;
            }
        };

        const resetToDefault = async () => {
            products.value = [];   
            typeBy.value = '';
            showDiscounted.value = false;
            queryTag.value = '';
            query.value = '';
            currentPage.value = 1;
            selectedCategories.value = [];
            selectedTags.value = [];

            const responseData = await fetchProductsBy(null, priceValue.value[0], priceValue.value[1], null, null, null, null, null, 1);
        
        };

        const updateProducts = (fetchedProducts) => {
            products.value=fetchedProducts ;
        };

     

        const updateByAllProduct = (fetchedProducts) => {
            const existingProductIds = new Set(products.value.map(product => product.productID));
            fetchedProducts.forEach(product => {
                if (!existingProductIds.has(product.productID)) {
                    products.value.push(product);
                    existingProductIds.add(product.productID);
                }
            });
        };

        const updateProductsByRange = (fetchedProducts, min, max) => {

            products.value = products.value.filter(product => product.salsePrice >= min && product.salsePrice <= max);

            const existingProductIds = new Set(products.value.map(product => product.productID));

            fetchedProducts.forEach(product => {
                if (product.salsePrice >= min && product.salsePrice <= max) {

                    if (!existingProductIds.has(product.productID)) {
                        products.value.push(product);
                        existingProductIds.add(product.productID);
                    }
                }
            });
        };



        const checkString = (s) => {
            return s.trim() !== '';
        };

        const filterCount = ref(0);
        const fetchProductsBy = async (query = '', minPrice = null, maxPrice = null, categoryId = '', tagId = '', categoryName = '', tagName = '', typeBy = '', page = 1, pageSize = 10) => {
            let responseData;

            if (maxPrice === 10) maxPrice = defaultMax;

            else maxPrice = maxPrice * priceMultiplier;

            minPrice = minPrice * priceMultiplier;

     
            if (query && query !== '') {
              
                queryTag.value = query.replace(/,/g, ' ').trim();
                responseData = await fetchProductsByQuery(query, minPrice, maxPrice);
            }
            else if (typeBy && typeBy !== null && typeBy !== '' && typeBy !== undefined) {

             
                responseData = await fetchProductsByType(typeBy, minPrice, maxPrice);
            }
            else if (
                (categoryId && checkString(categoryId)) ||
                (tagId && checkString(tagId)) ||
                (categoryName && checkString(categoryName)) ||
                (tagName && checkString(tagName))) {
          
                responseData = await fetchProductsByCategoryOrTag(categoryId, tagId, categoryName, tagName, minPrice, maxPrice);

            }

            else {
             
                responseData = await fetchProducts(page, pageSize, minPrice, maxPrice);

            }

            const {
                tags: fetchedTags,
                products: fetchedProducts,
                categorys: fetchedCategorys,
                totalCount: fetchedTotalCount,
                filterCount: fetchedFilterCount,
                searchCategorys: fetchedSearchCategorys,
                searchTags: fetchedSearchTags,
            } = responseData;



            if (fetchedSearchCategorys && fetchedSearchCategorys.length > 0) {

                selectedCategories.value = fetchedSearchCategorys;
            }
            if (fetchedSearchTags && fetchedSearchTags.length > 0) {

                selectedTags.value = fetchedSearchTags;
            }
        

            filterCount.value = fetchedFilterCount;
            tags.value = fetchedTags;
            categorys.value = fetchedCategorys;
            totalCount.value = fetchedTotalCount;

            if (query || categoryId || tagId || categoryName || tagName || typeBy) {

                updateProducts(fetchedProducts);
                 
            }
            else if (minPrice > min || maxPrice < defaultMax) {

                updateProductsByRange(fetchedProducts, minPrice, maxPrice);
            }

            else {
              
                updateByAllProduct(fetchedProducts);
                // updateProducts
            };
        };

        const isTagChecked = (tagId, tagName) => {
            return selectedTags.value.includes(tagId);
        };
        const isCategoryChecked = (categoryId, categoryName) => {
            return selectedCategories.value.includes(categoryId);
        };



        const updateFiltersAndFetch = async () => {


            const selectedCategoryIdsString = selectedCategories.value.map(category => category.categoryId).join(', ');
            const selectedCategoryNameString = selectedCategories.value.map(category => category.categoryName).join(', ');


            const selectedTagIdsString = selectedTags.value.map(tag => tag.tagId).join(', ');
            const selectedTagNameString = selectedTags.value.map(tag => tag.tagName).join(', ');


            // 優先級分類與標籤 => 價錢
            await fetchProductsBy("", priceValue.value[0], priceValue.value[1], selectedCategoryIdsString, selectedTagIdsString, selectedCategoryNameString, selectedTagNameString, "", 1);
        };

        const toggleCategory = async (categoryId, categoryName) => {
            const objInd = selectedCategories.value.findIndex(category =>
                category.categoryId === categoryId && category.categoryName === categoryName
            );

            if (objInd !== -1) {
                
                selectedCategories.value.splice(objInd, 1);

            } else {

                selectedCategories.value.push({ categoryId, categoryName });

            }

            await updateFiltersAndFetch();
        };


        const toggleTag = async (tagId, tagName) => {

            const objInd = selectedTags.value.findIndex(tag =>
                tag.tagId === tagId && tag.tagName === tagName
            );


            if (objInd !== -1) {

                selectedTags.value.splice(objInd, 1);

            } else {

                selectedTags.value.push({ tagId, tagName });

            }
            await updateFiltersAndFetch();

        };


        const removeItem = async (itemType, index) => {
            let selectedItems;
            if (itemType === 'category') {
                selectedItems = selectedCategories.value;
            } else if (itemType === 'tag') {
                selectedItems = selectedTags.value;
            }


            if (index >= 0) {

                selectedItems.splice(index, 1);
            }

            await updateFiltersAndFetch();

        };

        const removeCategory = async (index) => {
            await removeItem('category', index);
        };

        const removeTag = async (index) => {
            await removeItem('tag', index);
        };

        const handleScroll = () => {
            const scrollY = window.scrollY;
            const windowHeight = window.innerHeight;
            const documentHeight = document.documentElement.scrollHeight;

            if (scrollY + windowHeight >= documentHeight - 200 && !isLoading.value && products.value.length < totalCount.value && priceValue.value[0] === 0 && priceValue.value[1] === 10) {
                currentPage.value++;
                fetchProductsBy('', priceValue.value[0], priceValue.value[1], null, '', null, '', '', currentPage.value);
            }

        };



        function throttle(fn, delay) {
            let lastCall = 0;
            return function (...args) {
                const now = Date.now();
                if (now - lastCall >= delay) {
                    lastCall = now;
                    fn.apply(this, args);
                }
            };
        }

        function debounce(fn, delay) {
            let timer;
            return function (...args) {
                clearTimeout(timer);
                timer = setTimeout(() => fn(...args), delay);
            };
        }


        const throttledHandleScroll = throttle(handleScroll, 200);


        onMounted(() => {
       
            fetchProductsBy(query.value, priceValue.value[0], priceValue.value[1], categoryId.value, tagId.value, categoryName.value, tagName.value, typeBy.value, 1);
            window.addEventListener('scroll', throttledHandleScroll);
        });

        onUnmounted(() => {

            window.removeEventListener('scroll', throttledHandleScroll);
        });





        const filteredProducts = computed(() => {
            return products.value.filter(product => {
                const hasDiscount = product.hasDiscount;
                const isFree = product.unitlPrice <= 0;
                return (!showDiscounted.value || hasDiscount) &&
                    (!hideFreeItems.value || !isFree);
            }).sort((a, b) => {
                if (sortOrder.value === 'startDate') {
                    return new Date(a.startdate) - new Date(b.startdate);
                } else if (sortOrder.value === 'lowestPrice') {
                    return a.salsePrice - b.salsePrice;
                } else if (sortOrder.value === 'highestPrice') {
                    return b.salsePrice - a.salsePrice;
                }
                return 0;
            });;
        });
        const handleSortChange = (event) => {
            sortOrder.value = event.target.value;
        };



        //==============================
        const totalProductsCount = computed(() => {
            return products.value.length;
        });

        const excludedProductsCount = computed(() => {
            return totalProductsCount.value - filteredProducts.value.length;
        });

        const productCount = computed(() => {
            return filteredProducts.value.length;
        });



        const handleClickOutside = (event) => {

            const menu = document.querySelector('.side-menu');
            const toggleButton = document.querySelector('.burger');
            if (!menu.contains(event.target) && !toggleButton.contains(event.target)) {
                menuVisible.value = false;
            }
        };

        const enter = (el, done) => {
            el.offsetWidth;

            el.style.transform = 'translateX(0)';

            done();
        };

        const leave = (el, done) => {

            el.style.transform = 'translateX(100%)';

            done();
        };

        onMounted(() => {
            document.addEventListener('click', handleClickOutside);
        });
        const toggleAccordion = (section) => {

            if (isDragAccordion.value) {
                section.preventDefault();
                return;
            }
            isAccordionOpen.value[section] = !isAccordionOpen.value[section];
        };


        const preventAccordionToggle = (event) => {
            event.stopPropagation();
            isDragAccordion.value = true;
        };

        const startDraggingRange = (event) => {
            isDragAccordion.value = true;
            event.stopPropagation()
        };

        const stopDraggingRange = (event) => {
            isDragAccordion.value = false;

        };
        onMounted(() => {
            const rangeInput = document.querySelector('input[type="range"]');
            if (rangeInput) {
                rangeInput.addEventListener('pointerdown', startDraggingRange);
                rangeInput.addEventListener('pointerup', stopDraggingRange);
            }
        });
        onBeforeUnmount(() => {
            const rangeInput = document.querySelector('input[type="range"]');
            if (rangeInput) {
                rangeInput.removeEventListener('pointerdown', startDraggingRange);
                rangeInput.removeEventListener('pointerup', stopDraggingRange);
            }
        });




        const handleHideFreeItemsChange = (event) => {

            hideFreeItems.value = event.target.checked;
        };

        const draggedIndex = ref(null);
        const dragOverIndex = ref(null);

        const isDragging = ref(false);

        const onDragStart = (index) => {
            draggedIndex.value = index;
            isDragging.value = true;
            document.querySelectorAll('li')[index].classList.add('dragging');
        };
        const resetDragState = () => {
            draggedIndex.value = null;
            dragOverIndex.value = null;
            isDragging.value = false;

        };

        const onDragOver = (index, event) => {
            event.preventDefault();
            event.stopPropagation();

            const target = event.currentTarget.getBoundingClientRect();
            const offset = event.clientY - target.top;
            const threshold = target.height / 2;

            if (offset > threshold) {
                dragOverIndex.value = index + 1;
            } else {
                dragOverIndex.value = index;
            }
        };

        const onDragEnter = (index, event) => {
            event.preventDefault();
            event.stopPropagation();
            dragOverIndex.value = index;
        };

        const onDragLeave = (event) => {
            event.preventDefault();
            event.stopPropagation();
            dragOverIndex.value = null;
        };

        const onDrop = (index, event) => {
            event.preventDefault();
            event.stopPropagation();

            if (draggedIndex.value !== null && draggedIndex.value >= 0 && draggedIndex.value < filteredProducts.value.length) {
                const draggedItem = filteredProducts.value[draggedIndex.value];

                if (draggedItem) {
                    filteredProducts.value.splice(draggedIndex.value, 1);
                    filteredProducts.value.splice(index, 0, draggedItem);
                } else {

                }
            } else {

            }
            resetDragState();
        };

        return {
            handleSliderChange,
            priceValue,
            priceRange, formattedPriceRange,
            isAnyPrice,
            productCount,
            totalProductsCount,
            filterCount,
            excludedProductsCount,
            selectedTags,
            selectedCategories,
            tags,
            categorys,
            products,
            isAccordionOpen,
            filteredProducts,
            searchQuery,
            isDragging,
            draggedIndex,
            dragOverIndex,
            queryTag,
            hideFreeItems,
            showDiscounted,
            menuVisible,
            isLoading,
            currentPage,
            totalCount,
            dynamicTitle,
            typeBy,
            categoryId,
            categoryName,
            tagId,
            tagName,
            enter,
            leave,
            toggleTag,
            toggleCategory,
            startDraggingRange,
            stopDraggingRange,
            toggleAccordion,
            preventAccordionToggle,
            onDragStart,
            onDragOver,
            resetDragState,
            onDrop,
            onDragEnter,
            onDragLeave,
            resetDragState,
            handleSortChange,
            handleHideFreeItemsChange,
            fetchProductsByCategoryOrTag,
            fetchProductsByQuery,
            fetchProductsBy,
            fetchProducts,
            updateProducts,
            isTagChecked,
            isCategoryChecked,
            fetchProductsByType,
            localUrl,
            resetToDefault,
            removeCategory,
            removeTag,
            updateByAllProduct,
            isZero
        };
    }
};
const app1 = Vue.createApp(App);
app1.use(ElementPlus).mount('#productSearch');




