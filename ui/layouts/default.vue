<template>
    <div class="app-fix flex">
        <NavBar />
        <main class="fill flex">
            <slot />
        </main>
    </div>
</template>

<style lang="scss" scoped>
    main {
        margin-right: 2px;
        overflow: hidden;
        max-width: 100%;
    }

    @media only screen and (max-width: 1050px) {
        .app-fix {
            flex-flow: column;
        }
    }
</style>