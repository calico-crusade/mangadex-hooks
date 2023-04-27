<template>
<div class="flex fill-parent">
    <div class="center flex">
        <ProfileCard :profile="user"></ProfileCard>
    </div>
</div>
</template>

<script lang="ts" setup>
const user = authApi.currentUser;

useHead({ title: 'MD Hooks | Your account' });

onMounted(() => {
    if (!user.value) {
        navigateTo('/');
        return;
    }
})
</script>

<style lang="scss">

</style>