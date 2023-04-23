<template>
    <div class="card profile-card flex">
        <img :src="profile?.avatar" />
        <div class="content flex fill row">
            <h2>{{ profile?.nickname }}</h2>
            <div class="tags in-line" v-if="!!profile?.roles?.length">
                <span>Roles: </span>
                <span v-for="role in profile?.roles">{{ role }}</span>
            </div>
            <div class="tags in-line">
                <span>Login Provider: </span>
                <a v-if="profile?.provider === 'Discord'" :href="`https://discord.com/users/${profile?.providerId}`" target="_blank">
                    Discord
                </a>
                <span v-else>
                    {{ profile?.provider }}: {{ profile?.providerId }}
                </span>
            </div>
            <div class="tags in-line">
                <span>Profile Id: </span>
                <span class="no-bg">{{ profile?.profileId }}</span>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
    import { UserResult } from '~/utils/models';
    const { profile } = defineProps<{ profile?: UserResult }>();

    useHead({ title: 'Your account!' });
</script>

<style lang="scss" scoped>
    .profile-card {
        padding: 1rem;
        border-radius: 10px;
        background-color: var(--bg-color-accent);
        min-width: min(800px, 95vw);

        img {
            border-radius: 50%;
            max-height: 128px;
            max-width: 128px;
        }

        .content {
            margin-left: 1rem;
        }
    }
</style>