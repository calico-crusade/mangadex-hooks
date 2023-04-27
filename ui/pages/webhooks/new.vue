<template>
    <Loading v-if="pending" />
    <Error v-else-if="error || apiError" :message="apiError?.errors[0] ?? error?.message ?? 'Something went wrong!'" />
    <div v-else class="fill scroll-y flex">
        <div class="fill max-width center rounded bg-accent pad margin scroll-y">
            <WebhookEdit :hook="hook" @submitted="submit" />
        </div>
    </div>
</template>

<script setup lang="ts">
import { FetchError } from 'ofetch';
import { FailureResult, Webhook, WebhookType } from '~/utils/models';

useHead({
    title: 'MD Hooks | Create a new webhook'
})

const pending = ref(false);
const error = ref<FetchError | undefined>(undefined);
const apiError = ref<FailureResult | undefined>(undefined);

const currentUser = authApi.currentUser;

const hook = ref<Webhook>({
    name: 'My First Webhook',
    ownerId: currentUser.value?.profileId ?? 0,
    url: '',
    type: WebhookType.Json,
    discordData: undefined,
    id: -1,
    createdAt: new Date(),
    updatedAt: new Date()
});

const submit = async (hook: Webhook) => {
    pending.value = true;

    const { data, apiError: err } = await webhooksApi.post(JSON.parse(JSON.stringify(hook)));

    if (!data.value || err.value) {
        console.error('Error occurred while pushing webhook', { 
            data: {...data.value},
            error: {...err.value}
        });
        apiError.value = <any>{ ...err.value };
        pending.value = false;
        return;
    }

    await navigateTo(`/webhooks/${data.value.id}`);
    pending.value = false;
};
</script>