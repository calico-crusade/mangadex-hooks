<template>
    <div  class="hook max-width center-horz fill">
        <Loading v-if="pending" />
        <Error v-else-if="error || apiError" :message="apiError?.errors[0] ?? error?.message ?? 'Something went wrong!'" />
        <Tabs v-else>
            <Tab title="Settings" icon="settings" scrollable keep-alive class-name="flex row">
                <WebhookEdit :hook="hook" @tested="test" @submitted="submit" />
            </Tab>
            <Tab title="Subscriptions" icon="notifications_active" keep-alive>
                <Watchers :id="id" />
            </Tab>
            <Tab title="Run History" icon="history" keep-alive>
                <WebhookHistoryScroll :id="id" :reset="resetSwitch" />
            </Tab>
        </Tabs>
        <PopupAlert :content="alertMessage" :title="alertTitle" :reset-switch="alertMessage" is-markdown></PopupAlert>
    </div>
</template>

<script setup lang="ts">
import { MdManga, Webhook } from '~/utils/models';

const id = +(useRoute().params.id as string);
const resetSwitch = ref(0);
const alertMessage = ref('');
const alertTitle = ref('');

const {
    pending,
    apiError,
    result: hook,
    error,
    refresh,
} = await webhooksApi.get(id);

useHead({ title: 'MD Hooks | Edit ' + (hook.value?.name || 'a webhook') });

const test = async (chapId?: string) => {
    pending.value = true;

    const { data, apiError: err } = await webhooksApi.test(id, chapId);

    if (!data.value || err.value) {
        console.error('Error occurred while testing webhook', { 
            data: {...data.value},
            error: {...err.value}
        });
        data.value ??= {
            code: 500,
            result: 'error',
            message: 'An API error occurred: ' + err.value
        }
    }

    alertTitle.value = isCodeSuccess(data.value.code) ? 'Webhook Executed Successfully' : 'Webhook failed!';
    alertMessage.value = data.value.message ?? (isCodeSuccess(data.value.code) ? 'Ok!' : 'An unknown error occurred!');
    resetSwitch.value++;
    pending.value = false;
}

const submit = async (hook: Webhook) => {
    pending.value = true;

    const { data, apiError: err } = await webhooksApi.post(JSON.parse(JSON.stringify(hook)));

    if (!data.value || err.value) {
        console.error('Error occurred while pushing webhook', { 
            data: {...data.value},
            error: {...err.value}
        });

        alertTitle.value = 'An error occurred while saving the webhook';
        alertMessage.value = err.value?.errors.join('\n') ?? 'An unknown error occurred!';
        pending.value = false;
        return;
    }

    await refresh();
}

const man = (manga: MdManga) => {
    console.log('Manga Found', { manga });
}
</script>

<style lang="scss" scoped>

</style>