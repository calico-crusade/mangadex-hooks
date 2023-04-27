<template>
    <button class="icon-btn" @click="() => test()">
        <Icon>construction</Icon>
        <p>Execute Locally</p>
    </button>

    <PopupInput
        v-model="chapId"
        :prompt="chapPrompt"
        title="What Chapter ID should be used for testing?"
        label=""
        placeholder="MangaDex Chapter ID (optional)"
        @confirmed="(val) => run(val)"
    />

    <PopupLoading v-model="loading" />

    <Popup v-model="errorOpen" title="An Error Occurred">
        <Error :message="error?.errors.join('\n')" />
    </Popup>

    <Popup v-model="hookOpen" title="The results of the webhook" max-size="700px">
        <div class="discord-fake" v-if="hook">
            <div class="avatar">
                <img :src="hook.avatarUrl ?? '~/assets/discord-icon.png'" />
            </div>
            <div class="discord-message">
                <div class="username">
                    <div class="title">{{ hook.username ?? 'Discord Bot' }}</div>
                    <div class="tag">BOT</div>
                    <div class="date">Today at 10:00 PM</div>
                </div>
                <div class="message">
                    <div class="message-data" v-if="hook.content">
                        <Markdown :content="hook.content" />
                    </div>
                    <div class="embed" v-for="embed in hook.embeds" v-if="!hook.suppressEmbeds" :style="{'border-color': color(embed.color)}">
                        <div class="embed-content">
                            <div class="author" v-if="embed.author && (embed.author.iconUrl || embed.author.name)">
                                <div class="author-image">
                                    <img :src="embed.author.iconUrl" v-if="embed.author" />
                                </div>
                                <a :href="embed.author.url" target="_blank" v-if="embed.author.name">
                                    {{ embed.author.name }}
                                </a>
                            </div>
                            <a class="title" :href="embed.url" target="_blank" v-if="embed.title">
                                {{ embed.title }}
                            </a>
                            <div class="description" v-if="embed.description">
                                <Markdown :content="embed.description" />
                            </div>
                            <div class="fields">
                                <div class="field" v-for="field in embed.fields" :class="{ 'not-inline': !field.inline }">
                                    <div class="key"><Markdown :content="field.name" /></div>
                                    <div class="value"><Markdown :content="field.value" /></div>
                                </div>
                            </div>
                            <div class="image" v-if="embed.image">
                                <img :src="embed.image" />
                            </div>
                            <div class="author">
                                <div class="author-image" v-if="embed.footer?.iconUrl">
                                    <img :src="embed.footer?.iconUrl" />
                                </div>
                                <div class="name" v-if="embed.footer?.text">{{ embed.footer?.text }}</div>
                                <div class="date" v-if="embed.timestamp"><Date :date="embed.timestamp" format="YYYY/MM/DD HH:mm" /></div>
                            </div>
                        </div>
                        <div class="embed-thumbnail" v-if="embed.thumbnail">
                            <img :src="embed.thumbnail" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </Popup>
</template>

<script setup lang="ts">
import { DiscordWebhook, FailureResult } from '~/utils/models';
interface Emits {
    (e: 'update:modelValue', value: string | undefined): void;
}
interface Props {
    modelValue: string | undefined;
}
const props = defineProps<Props>();
const emits = defineEmits<Emits>();

const script = computed({
    get() { return props.modelValue; },
    set(value: string | undefined) { emits('update:modelValue', value); }
});

const chapId = ref('');
const chapPrompt = ref(0);
const loading = ref(false);
const hook = ref<DiscordWebhook | undefined>(undefined);
const error = ref<FailureResult | undefined>(undefined);
const hookOpen = computed({
    get() { return !!hook.value; },
    set(value: boolean){ if (!value) hook.value = undefined; }
});
const errorOpen = computed({
    get() { return !!error.value; },
    set(value: boolean) { if (!value) error.value = undefined; }
});

const test = () => {
    if (!script) return;
    chapPrompt.value++;
}

const run = async (id: string | undefined) => {
    if (!script.value) return;
    
    const _id = id || undefined;
    loading.value = true;

    const { result, apiError } = await webhooksApi.test(script.value, _id);

    error.value = apiError.value;
    hook.value = result.value;
    loading.value = false;
}

const color = (color?: number) => {
    if (!color) return `rgb(0, 0, 0)`;

    const [r, g, b] = Bytes.to(color);
    return `rgb(${r}, ${g}, ${b})`;
}
</script>

<style lang="scss">
$discord-secondary: rgb(88, 101, 242);
$discord-muted: rgb(133, 155, 164);
$discord-margin: 5px;
$discord-embed-bg: rgb(43, 45, 49);
$discord-link: rgb(14, 168, 252);
.discord-fake {
    display: flex;
    flex-flow: row;
    max-width: min(100%, 700px);
    a {
        color: $discord-link;
        text-decoration: none;
    }
    .avatar {
        img {
            margin: var(--margin);
            margin-top: 5px;
            width: 45px;
            height: 45px;
            border-radius: 50%;
        }
    }
    .discord-message {
        flex: 1;
        display: flex;
        flex-flow: column;
        overflow: hidden;
        .username {
            display: flex;
            flex-flow: row;
            align-items: center;
            .title {
                font-weight: bold;
                color: var(--color);
                margin: $discord-margin;
                margin-left: 0;
            }
            .tag {
                font-size: 14px;
                padding: 3px 5px;
                background-color: $discord-secondary;
                border-radius: 3px;
                margin-right: $discord-margin;
            }

            .date {
                color: var(--color-muted);
                font-size: 14px;
            }
        }
        .message {
            display: flex;
            flex-flow: column;
            overflow: hidden;
            .embed {
                display: flex;
                flex-flow: row;
                background-color: $discord-embed-bg;
                padding: var(--margin);
                border-left: 3px solid #{$discord-secondary};
                border-radius: 5px;
                margin-top: $discord-margin;
                margin-right: var(--margin);
                overflow: hidden;
                .embed-content {
                    flex: 1;
                    display: flex;
                    flex-flow: column;
                    overflow: hidden;
                    .author {
                        display: flex;
                        flex-flow: row;
                        align-items: center;
                        margin-bottom: var(--margin);
                        .author-image {
                            width: 30px;
                            height: 30px;
                            border-radius: 50%;
                            overflow: hidden;
                            display: flex;
                            margin-right: 10px;
                            img {
                                margin: auto;
                                max-height: 30px;
                                max-width: 30px;
                            }
                        }
                        a { 
                            font-weight: bold; 
                            color: var(--color); 
                        }
                        .name:not(:last-child) {
                            &::after {
                                margin: 0 5px;
                                content: '•';
                            }
                        }
                    }
                    .title {
                        font-weight: bold;
                        margin-bottom: var(--margin);
                    }
                    .description {
                        margin-bottom: var(--margin);
                    }
                    .fields {
                        display: grid;
                        grid-template-columns: repeat(3, minmax(0, 1fr));
                        gap: var(--margin);
                        margin-bottom: var(--margin);

                        .field {
                            .key {
                                font-weight: bold;
                                margin-bottom: 5px;
                            }
                            &.not-inline {
                                grid-column: 1 / -1;
                            }
                        }
                    }
                    .image {
                        margin-bottom: var(--margin);
                        display: flex;
                        img {
                            max-width: 100%;
                            max-height: 350px;
                            border-radius: 5px;
                        }
                    }
                    :last-child {
                        margin-bottom: 0;
                    }
                }
                .embed-thumbnail {
                    display: flex;
                    margin-left: var(--margin);

                    img {
                        max-width: 80px;
                        max-height: 80px;
                        border-radius: 5px;
                    }
                }
            }
        }
    }
}

@media only screen and (max-width: 620px) {
    .field {
        grid-column: 1 / -1;
    }
}
</style>