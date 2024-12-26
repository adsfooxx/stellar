var { createApp, ref, reactive, onMounted, computed } = Vue;
  
createApp({
    setup() {
        const account = ref('');
        const purchaseHistoryList = ref([]);
        const loading = ref(false);
        const order = ref({
            histroyDetailProducts: [] 
        });
      
        const showOrderDetail = ref(false);  
        const fetchPurchaseHistory = async () => {
            try {
                const response = await axios.get('/api/MemberApi/PurchaseHistory');
                account.value = response.data.userAccount; 
                purchaseHistoryList.value.push(...response.data.purchaseHistoryList);
            } catch (error) {
                console.error('Error fetching purchase history:', error);
            } finally {
                loading.value = false;
            }
        };
        const viewOrderDetail = async (id) => {
      
            loading.value = true; 
            showOrderDetail.value = true;
            try {
                const response = await axios.get(`/api/Memberapi/OrderDetail/${id}`);
                order.value = await response.data;    
                console.log(order.value)
             
            } catch (error) {
                console.error('無法獲取訂單詳情:', error);
            } finally {
                loading.value = false; // 結束加載
            }
        };
        const backToHistory = () => {
            showOrderDetail.value = false; // 返回到購買紀錄頁面
            console.log(showOrderDetail.value)
        };

        const searchQuery = ref(''); 

        const filteredGames = computed(() => {
            const query = searchQuery.value.trim().toLowerCase();

            if (!query) {
                return purchaseHistoryList.value;
            }

            return purchaseHistoryList.value.filter(order =>
                order.productList.some(product =>
                    product.productName.trim().toLowerCase().includes(query) 
                )
            );
        });
        const formatDate = (dateString) => {
            const options = { year: 'numeric', month: '2-digit', day: '2-digit' };
            return new Date(dateString).toLocaleDateString('zh-TW', options);
        };


        onMounted(() => {
            fetchPurchaseHistory();
        });

        return {
            order,
            loading,
            account,
            purchaseHistoryList,
            searchQuery,
            filteredGames,
            showOrderDetail,
            backToHistory,
            viewOrderDetail,
            formatDate
        };
    }
}).mount('#PurchaseHistory');


