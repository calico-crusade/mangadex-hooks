<template>
<Error v-if="error" :message="error.errors[0]" />
<div v-else class="flex row fill-parent overflow">
    <div class="flex row fill scroll-y" @scroll="onScroll">
        <div class="history" v-for="history in data" :class="{ 'success': isSuccess(history) }">
            <div class="grid by-2">
                <div class="title">Run #{{ history.id }} - Exited with code: {{ history.code }}</div>
                <div class="date"><Date :date="history.createdAt" /></div>
            </div>
            <Markdown :content="history.results" />
        </div>
        <Loading v-if="pending" inline />
    </div>
</div>
</template>

<script setup lang="ts">
import { WebhookHistory } from '~/utils/models';

const { id, reset } = defineProps<{ id: number, reset?: number }>();

const paginator = webhooksApi.history(id);
const { pending, data, error } = paginator;

const onScroll = (el: UIEvent) => {
    paginator.onScroll(<HTMLElement>el.target);
}

const isSuccess = (item: WebhookHistory) => {
    return isCodeSuccess(item.code);
}

watch(() => reset, () => paginator.refresh());
</script>

<style lang="scss">
.history {
    padding: var(--margin);
    border-radius: 10px;
    background-color: var(--bg-color-accent);
    margin-bottom: var(--margin);
    border: 1px solid var(--color-warning);

    .date {
        text-align: right;
    }

    &.success {
        border-color: transparent;
    }
}
</style>