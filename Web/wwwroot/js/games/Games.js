
var { createApp, ref, computed, onMounted, reactive } = Vue;

createApp({
    setup() {
        const user = reactive({
            userId: 0,
            userName: '',
            userImgUrl: '',
            gameCount: 0,
            games: []
        });
        const fetchUserData = async () => {
            try {
                const response = await axios.get('/api/MemberApi/GameLibray');
                console.log('API Response:', response.data);
                if (response.data) {
                    user.userId = response.data.userId;
                    user.userName = response.data.userName;
                    user.userImgUrl = response.data.userImgUrl;
                    user.gameCount = response.data.gameCount;
                    user.games = response.data.games;
                    user.games.forEach(game => {
                        if (!game.productName) {
                            console.error('Invalid game item:', game);
                        }
                    });
                }
            } catch (error) {
                console.error('Error fetching user data:', error);
            }
        };
        const searchQuery = ref('');

        const filteredGames = computed(() => {
            const query = searchQuery.value.toLowerCase();
            console.log(user.games)
            return user.games.filter(game => game.productName.toLowerCase().includes(query)).map(game => {
                if (!game.productName) {
                    console.error('Filtered game without productName:', game);
                }
                return game;
            });
        });

        onMounted(() => {
            fetchUserData();
        });

        const draggedIndex = ref(null);
        const dragOverIndex = ref(null);
        const isDragging = ref(false);

        const onDragStart = (index) => {
            console.log(index)
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

            if (draggedIndex.value !== null && draggedIndex.value >= 0 && draggedIndex.value < filteredGames.value.length) {
                const draggedItem = filteredGames.value[draggedIndex.value];

                if (draggedItem) {
                    filteredGames.value.splice(draggedIndex.value, 1);
                    filteredGames.value.splice(index, 0, draggedItem);
                    user.games = [...filteredGames.value];
                } else {
                    console.error('Dragged item is undefined');
                }
            } else {

                console.error('Invalid dragged index');
            }
            resetDragState();
        };

        return {
            onDragOver,
            onDragStart,
            onDrop,
            onDragEnter,
            onDragLeave,
            resetDragState,
            dragOverIndex,
            draggedIndex,
            searchQuery,
            filteredGames,
            isDragging,
            user
        };
    }
}).mount('#gameLibrary');





