<template>
    <Error v-if="error" :message="error.errors[0]" />
    <div v-else class="hook-history flex row fill scroll-y" @scroll="onScroll">
        <h2 class="center-horz max-width">Webhook History</h2>
        <NuxtLink :to="'/webhooks/' + result.hook.id" class="history max-width center-horz" :class="{ 'success': isSuc(result) }" v-for="result in data">
            <div class="grid by-2">
                <div class="title"><b>{{ result.hook.name }}</b>&nbsp;({{ result.hook.id }})</div>
                <div class="date"><Date :date="result.result.createdAt" /></div>
                <div class="title">Run #{{ result.result.id }}</div>
                <div class="date">Exit Code: {{ result.result.code }}</div>
            </div>
            <div class="result-set">
                <Markdown :content="result.result.results" />
            </div>
        </NuxtLink>
        <Loading v-if="pending" inline />
    </div>
</template>

<script setup lang="ts">
import { WebhookHistoryWithRes } from '~/utils/models';

const paginator = webhooksApi.historyByOwner(1, 20);
const { pending, data, error } = paginator;

const onScroll = (el: UIEvent) => { paginator.onScroll(<HTMLElement>el.target); }
const isSuc = (result: WebhookHistoryWithRes) => isCodeSuccess(result.result.code);
</script>

<style lang="scss" scoped>
.hook-history {
    h2 {
        margin: var(--margin) auto;
    }

    .history {
        .result-set {
            padding: var(--margin);
            margin-top: var(--margin);
            background-color: var(--bg-color);
            border-radius: var(--margin);
        }
    }
}

</style>