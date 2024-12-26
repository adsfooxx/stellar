
var { createApp, ref, computed, onMounted, watch } = Vue;
const searchButton = createApp({
    setup() {
        const searchTerm = ref('');
        const filteredSuggestions = ref([]);
    
        function search() {
            const keywords = searchTerm.value
                .split(' ')
                .map(term => term.trim().toLowerCase())
                .filter(term => term !== '');
            const query = keywords.join(',');
         
            window.location.href = `/product/ProductSearchMin?query=${encodeURIComponent(query)}`;

        }

        const fetchSuggestions = _.debounce(async (query) => {

            if (searchTerm.value == false) {
                filteredSuggestions.value = [];
                return;
            }

            try {

                const response = await axios.get(`/api/ProductSearchAPI/Suggestions/${searchTerm.value}`);

                filteredSuggestions.value = response.data;


                initializeTooltips();
            } catch (error) {
                console.error('API 請求失敗:', error);
                filteredSuggestions.value = [];
            }
        }, 300); 

        const navigateToProduct = (productID) => {
            window.location.href = `/Product/ProductPage/${productID}`;
        };

        const initializeTooltips = () => {
            setTimeout(() => {
                const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
                tooltipTriggerList.map(function (tooltipTriggerEl) {
                    return new bootstrap.Tooltip(tooltipTriggerEl);
                });
            }, 0);
        };

        onMounted(() => {
           
            initializeTooltips();
        });

       
        watch(searchTerm, (newQuery) => {
            fetchSuggestions(newQuery);
        });

        watch(filteredSuggestions, () => {
            initializeTooltips();
        });


        return {
            searchTerm,
            search,
            filteredSuggestions,
            onInput: fetchSuggestions,
            navigateToProduct,
        };
    }
});
searchButton.mount('#SearchId');

