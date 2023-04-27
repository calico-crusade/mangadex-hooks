<template>
    <Loading v-if="pending" />
    <Error v-else-if="error" :message="error?.errors[0] ?? 'Something went wrong!'" />
    <div v-else class="hooks flex row fill scroll-y" @scroll="(el: UIEvent) => paginator.onScroll(<any>el.target)">
        <h1 class="max-width center-horz">Your Webhooks:</h1>
        <NuxtLink :to="`/webhooks/${hook.id}`" class="hook grid by-2 max-width center-horz" v-for="hook in results.data">
            <div class="title">{{ hook.name }} (#{{ hook.id }})</div>
            <div class="date">
                Last Updated at <Date :date="hook.updatedAt" />
            </div>
            <div class="type">
                Type: {{ getTypeText(hook.type)?.text ?? 'Unknown' }}
            </div>
        </NuxtLink>
    </div>
</template>

<script setup lang="ts">
import { HOOK_TYPES, WebhookType } from '~/utils/models';

useHead({ title: 'MD Hooks | Your webhooks' })

const paginator = webhooksApi.paginate();
const { pending, results, error } = paginator;

const getTypeText = (type: WebhookType) => {
    return HOOK_TYPES.find(t => t.value === type);
}
</script>

<style lang="scss" scoped>
.hooks {
    h1 {
        margin: var(--margin) auto;
    }

    .hook {
        padding: var(--margin);
        border-radius: 10px;
        background-color: var(--bg-color-accent);
        margin-bottom: var(--margin);

        .date {
            text-align: right;
        }
    }
}
</style>