<template>
    <div class="webhook-edit flex row" v-if="hook">
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
            <div class="group">
                <input class="fill" :type="hideUrl ? 'password' : 'url'" :readonly="readonly" v-model="hook.url" placeholder="Web Hook URL" />
                <button @click="() => hideUrl = !hideUrl">
                    <Icon>{{ hideUrl ? 'visibility' : 'visibility_off' }}</Icon>
                </button>
            </div>
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

        <template v-else-if="hook.type === 4 && hook.discordScript">
            <div class="code-editor" v-if="false">
                <CodeEditor v-model="script" :typings="shim" />
            </div>
            <div class="control" v-else>
                <label>Discord Embed Script:</label>
                <textarea 
                    class="fake-editor" 
                    v-model="script" 
                    placeholder="The discord embed creation script" 
                    height="500px"></textarea>
                <p class="note">
                    Actual code editor 
                    <a href="https://github.com/e-chan1007/nuxt-monaco-editor/issues/27" target="_blank">
                        coming soon &trade;
                    </a>. 
                    You can get the type shims 
                    <a @click="() => save(shim || '')">here</a> 
                    for editing in VS Code for now..
                </p>
            </div>
        </template>

        <footer class="flex">
            <button class="icon-btn pad-left" @click="() => submit()">
                <Icon>save</Icon>
                <p>Save</p>
            </button>
            <WebhookRunLocally 
                v-model="hook.discordScript" 
                v-if="hook.type === 4 && hook.discordScript"
            />
            <button v-if="!isNew" class="icon-btn" @click="() => test()">
                <Icon>send</Icon>
                <p>Execute on Discord</p>
            </button>
        </footer>
    </div>

    <PopupInput 
        v-model="chapterId" 
        :prompt="promptChapKey" 
        title="What chapter ID should be used for testing?" 
        label=""
        placeholder="MangaDex Chapter ID"
        @confirmed="(val) => $emit('tested', val)" 
    />
</template>

<script setup lang="ts">
import { HOOK_TYPES, Webhook, WebhookType } from '~/utils/models';

interface Props {
    hook?: Webhook;
}

interface Emits {
    (e: 'submitted', hook: Webhook): void;
    (e: 'tested', chapterId: string | undefined): void;
}

const hookTypes = HOOK_TYPES;
const { hook } = defineProps<Props>();
const emit = defineEmits<Emits>();
const currentUser = authApi.currentUser;
const isNew = computed(() => (hook?.id ?? 0) <= 0);
const readonly = computed(() => hook && !isNew.value && hook.ownerId !== currentUser.value?.profileId);

const promptChapKey = ref(0);
const chapterId = ref<string | undefined>(undefined);
const hideUrl = ref(true);

const { data } = await webhooksApi.shim();
const shim = computed(() => data.value?.data.shim);
const defa = computed(() => data.value?.data.default);

const script = computed({
    get() { return hook?.discordScript || ''; },
    set(value: string) { if (hook) hook.discordScript = value; }
});

const typeChanged = () => {
    if (!hook) return;

    if (hook.type === WebhookType.Discord && !hook.discordData) {
        hook.discordData = {
            tts: false,
            suppressEmbeds: false,
            content: 'Hey everyone! Checkout this new chapter that just dropped!'
        };
    }

    if (hook.type === WebhookType.DiscordScript && !hook.discordScript) {
        hook.discordScript = defa.value;
    }
}

const test = async () => {
    if (!hook) return;
    promptChapKey.value++;
}

const submit = async () => {
    if (!hook) return;
    emit('submitted', hook);
}

const save = (data: string) => {
    const link = document.createElement('a');
    const file = new Blob([data], { type: 'text/plain' });
    link.href = URL.createObjectURL(file);
    link.download = 'discord-script.d.ts';
    link.click();
    URL.revokeObjectURL(link.href);
};

</script>

<style lang="scss">
.webhook-edit {
    flex: 1;

    header {
        flex-flow: row wrap;
        margin-top: var(--margin);
        h2 { white-space: pre; }
    }

    footer {
        margin-top: auto;
        flex-flow: row wrap;
        button {
            margin-top: var(--margin);
            margin-right: var(--margin);
        }
    }

    .code-editor {
        margin-top: 10px;
        position: relative;
        max-height: min(90vh, 500px);
        height: 100%;
    }

    .fake-editor {
        height: 500px;
        font-family: consolas;
    }
}

@media only screen and (max-width: 620px) {
    .webhook-edit {
        footer {
            flex-flow: column;
            button {
                flex: 1;
                margin: 5px;
            }
        }
    }
}
</style>