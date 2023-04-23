<template>
    <Loading v-if="pending" />
    <Error v-else-if="error || apiError" :message="apiError?.errors[0] ?? error?.message ?? 'Something went wrong!'" />
    <div v-else class="hook max-width center-horz fill">
        <header v-if="isNew">
            <h2>Create a new Web Hook</h2>
        </header>
        <header v-else class="flex center-items">
            <h2 class="fill">Edit: {{ hook.name }}</h2>
            <p>Last Updated at <Date :date="hook.updatedAt" /></p>
        </header>

        <div class="control" v-if="isNew">
            <label>Name</label>
            <input type="text" :readonly="readonly" v-model="hook.name" placeholder="Web Hook Name" />
        </div>
        <div class="control">
            <label>Url</label>
            <input type="url" :readonly="readonly" v-model="hook.url" placeholder="Web Hook URL" />
        </div>
        <div class="control">
            <label>Type</label>
            <select v-model="hook.type" :disabled="readonly" @change="() => typeChanged()">
                <option v-for="hookType in hookTypes" :value="hookType.value">
                    {{ hookType.text }}
                </option>
            </select>
        </div>

        <template v-if="hook.type === 1 && hook.discordData">
            <div class="control">
                <label>Discord Message: </label>
                <textarea type="text" v-model="hook.discordData.content" placeholder="The contents of the discord message"></textarea>
                <p class="note">Custom Embeds coming soon &trade;</p>
            </div>
            <div class="control">
                <label>Webhook Username: </label>
                <input type="text" v-model="hook.discordData.username" placeholder="Webhook Username" />
                <p class="note">This changes the username the webhook is posted with</p>
            </div>
            <div class="control">
                <label>Webhook Avatar URL: </label>
                <input type="url" v-model="hook.discordData.avatarUrl" placeholder="Webhook Avatar URL" />
                <p class="note">This changes the image/avatar the webhook is posted with</p>
            </div>
            <div class="control checkbox">
                <CheckBox v-model="hook.discordData.suppressEmbeds">Suppress Embeds</CheckBox>
                <p class="note">Whether or not to allow embeds to appear.</p>
            </div>
            <div class="control checkbox">
                <CheckBox v-model="hook.discordData.tts">Text To Speech</CheckBox>
                <p class="note">Whether or not to read this embed out loud.</p>
            </div>
        </template>

        <footer class="flex">
            <button class="icon-btn pad-left" @click="() => submit()">
                <Icon>save</Icon>
                <p>Save</p>
            </button>
            <button v-if="!isNew" class="icon-btn" @click="() => test()">
                <Icon>send</Icon>
                <p>Test Web Hook</p>
            </button>
        </footer>
    </div>
</template>

<script setup lang="ts">
    import { FetchError } from 'ofetch';
    import { FailureResult, HOOK_TYPES, Webhook, WebhookType } from '~/utils/models';

    const currentUser = authApi.currentUser;
    const hookTypes = HOOK_TYPES;

    const _id = useRoute().params.id as string;
    const id = isNaN(+_id) ? -1 : +_id;

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

    const error = ref<FetchError | undefined>(undefined);
    const apiError = ref<FailureResult | undefined>(undefined);
    const pending = ref(false);
    const isNew = computed(() => id <= 0);
    const readonly = computed(() => !isNew.value && hook.value.ownerId !== currentUser.value?.profileId);
    let refresh: (() => Promise<void>) | undefined = undefined;

    if (!isNew.value) {
        pending.value = true;
        const { result, error: err, apiError: apiErr, pending: pend, refresh: ref } = await webhooksApi.get(id);

        refresh = ref;
        pending.value = pend.value;
        error.value = err.value || undefined;
        apiError.value = apiErr.value;

        if (result.value) hook.value = result.value;
    }

    const typeChanged = () => {
        if (hook.value.type === WebhookType.Discord && !hook.value.discordData) {
            hook.value.discordData = {
                tts: false,
                suppressEmbeds: false,
                content: 'Hey everyone! Checkout this new chapter that just dropped!'
            };
        }
    }

    const test = async () => {
        pending.value = true;

        const { data, apiError: err } = await webhooksApi.test(id);

        if (!data.value || err.value) {
            console.error('Error occurred while testing webhook', { 
                data: {...data.value},
                error: {...err.value}
            });
            apiError.value = <any>{ ...err.value };
            pending.value = false;
            return;
        }

        pending.value = false;
    }

    const submit = async () => {
        pending.value = true;

        const { data, apiError: err } = await webhooksApi.post(JSON.parse(JSON.stringify(hook.value)));

        if (!data.value || err.value) {
            console.error('Error occurred while pushing webhook', { 
                data: {...data.value},
                error: {...err.value}
            });
            apiError.value = <any>{ ...err.value };
            pending.value = false;
            return;
        }

        if (id === data.value.id && refresh) {
            await refresh();
            pending.value = false;
            return;
        }

        await navigateTo(`/webhooks/${data.value.id}`);
        pending.value = false;
    }
</script>

<style lang="scss" scoped>
    .hook {
        overflow-y: auto;
        padding: 0 10px;

        header {
            margin-top: var(--margin);
        }

        footer {
            margin-top: var(--margin);
            button {
                margin-right: var(--margin);
            }
        }
    }
</style>