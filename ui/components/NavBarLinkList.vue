<template>
<NuxtLink to="/" active-class="active">
    <Icon>home</Icon>
    <p>Home</p>
</NuxtLink>

<NuxtLink to="/webhooks" active-class="active">
    <Icon>webhook</Icon>
    <p>Web Hooks</p>
</NuxtLink>

<NuxtLink class="sub" to="/webhooks/new" active-class="active">
    <Icon>add</Icon>
    <p>New</p>
</NuxtLink>

<NuxtLink class="sub" to="/webhooks/results" active-class="active">
    <Icon>history</Icon>
    <p>History</p>
</NuxtLink>

<div class="fill"></div>

<button v-if="!user" @click="() => login()">
    <Icon>login</Icon>
    <p>Login</p>
</button>

<template v-else>
    <NuxtLink to="/account" active-class="active">
        <img class="avatar" :src="user.avatar" />
        <p>{{ user.nickname }}</p>
    </NuxtLink>
    <button @click="() => logout()">
        <Icon>logout</Icon>
        <p>Logout</p>
    </button>
</template>

<a href="https://cba.index-0.com" target="_blank">
    <Icon>live_tv</Icon>
    <p>Anime</p>
    <Icon>open_in_new</Icon>
</a>

<a href="https://discord.gg/RV9MvvYXsp" target="_blank">
    <img src="~/assets/discord-icon.png" alt="Discord Logo" />
    <p>Discord Server</p>
    <Icon>open_in_new</Icon>
</a>

<a href="https://github.com/calico-crusade/mangadex-hooks" target="_blank">
    <svg height="24" width="24" aria-hidden="true" viewBox="0 0 16 16" version="1.1" data-view-component="true" class="octicon octicon-mark-github v-align-middle">
        <path fill-rule="evenodd" d="M8 0C3.58 0 0 3.58 0 8c0 3.54 2.29 6.53 5.47 7.59.4.07.55-.17.55-.38 0-.19-.01-.82-.01-1.49-2.01.37-2.53-.49-2.69-.94-.09-.23-.48-.94-.82-1.13-.28-.15-.68-.52-.01-.53.63-.01 1.08.58 1.23.82.72 1.21 1.87.87 2.33.66.07-.52.28-.87.51-1.07-1.78-.2-3.64-.89-3.64-3.95 0-.87.31-1.59.82-2.15-.08-.2-.36-1.02.08-2.12 0 0 .67-.21 2.2.82.64-.18 1.32-.27 2-.27.68 0 1.36.09 2 .27 1.53-1.04 2.2-.82 2.2-.82.44 1.1.16 1.92.08 2.12.51.56.82 1.27.82 2.15 0 3.07-1.87 3.75-3.65 3.95.29.25.54.73.54 1.48 0 1.07-.01 1.93-.01 2.2 0 .21.15.46.55.38A8.013 8.013 0 0016 8c0-4.42-3.58-8-8-8z"></path>
    </svg>
    <p>Source Code</p>
    <Icon>open_in_new</Icon>
</a>
</template>

<script setup lang="ts">
const user = authApi.currentUser;
const login = async () => {
    const url = await authApi.loginUrl();
    if (!url) return;

    window.location.href = url;
};
const logout = () => authApi.logout();

onMounted(() => nextTick(async () => {
    await authApi.bump();
}))
</script>

<style lang="scss" scoped>
a, button {
    display: flex;
    flex-flow: row;
    border-left: 3px solid transparent;
    border-right: 3px solid transparent;
    padding: 5px;
    text-align: left;
    font-size: 16px;

    p { 
        margin: auto 5px; 
        flex: 1; 
        white-space: pre;
        display: block;
    }
    
    span:last-child { display: inline-block; }

    svg { path { color: #fff; fill: currentColor; } }

    img { 
        width: 24px;
        &.avatar { border-radius: 50%; }
    }
    &.sub { margin-left: 20px; }
    &.active { border-left-color: var(--color-primary); }
    &:hover, &.active { background-color: var(--bg-color-accent); }
}
</style>