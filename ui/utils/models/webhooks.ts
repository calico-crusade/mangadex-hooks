import { DbObject } from "./db.base";


export enum EmbedType {
    Rich = 0
}

export enum WebhookType {
    Disabled = 0,
    Discord = 1,
    Json = 2,
    Xml = 3,
    DiscordScript = 4
}

export interface DiscordField {
    name?: string;
    value?: string;
    inline: boolean;
}

export interface DiscordEmbed {
    type: EmbedType;
    description?: string;
    url?: string;
    title?: string;
    timestamp?: Date | string;
    color?: number;
    image?: string;
    thumbnail?: string;
    author?: {
        name?: string;
        url?: string;
        iconUrl?: string;
    };
    footer?: {
        text?: string;
        iconUrl?: string;
    };

    fields?: DiscordField[];
}

export interface DiscordWebhook {
    content: string;
    username?: string;
    avatarUrl?: string;
    tts: boolean;
    threadName?: string;
    threadId?: number;
    suppressEmbeds: boolean;
    embeds?: DiscordEmbed[];
}

export interface Webhook extends DbObject {
    name: string;
    ownerId: number;
    url: string;
    type: WebhookType;
    discordData?: DiscordWebhook;
    discordScript?: string;
}

export interface WebhookHistory extends DbObject {
    webhookId: number;
    results: string;
    code: number;
}

export interface WebhookHistoryWithRes {
    hook: Webhook;
    result: WebhookHistory;
}

export const HOOK_TYPES = [
    { text: 'Discord', value: WebhookType.Discord },
    { text: 'Discord Script', value: WebhookType.DiscordScript },
    { text: 'JSON', value: WebhookType.Json },
    { text: 'XML', value: WebhookType.Xml },
    { text: 'Disabled', value: WebhookType.Disabled }
];