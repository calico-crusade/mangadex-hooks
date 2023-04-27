<template>
<Error v-if="error" :message="error.errors[0]" />
<div v-else class="flex row fill-parent overflow">
    <div class="flex row fill scroll-y" @scroll="onScroll">
        <header class="scroll-header flex center-items">
            <h3 class="fill">Subscriptions</h3>
            <button @click="newWatch">
                <Icon>add</Icon>
            </button>
        </header>
        <div class="watcher" v-for="watcher in data" :class="getClass(watcher.type)" >
            <a class="image" :href="getHref(watcher)" target="_blank">
                <img :src="proxy(watcher.resourceImage)" />
            </a>
            <a class="data" :href="getHref(watcher)" target="_blank">
                <div class="title">
                    <div class="type">{{ getClass(watcher.type) }}</div>
                    <div class="text">{{ watcher.resourceName }}</div>
                    <div class="date">Created: <Date :date="watcher.updatedAt" /></div>
                </div>
                <div class="more">
                    <div class="id"><b>ID:</b>&nbsp;{{ watcher.itemId }}</div>
                    <div class="count" v-if="watcher.cacheItems.length > 0">Title Count: {{ watcher.cacheItems.length }}</div>
                </div>
            </a>
            <button @click="() => deleteItem(watcher)">
                <Icon>delete</Icon>
            </button>
        </div>
        <Loading v-if="pending" inline />
    </div>
</div>
<Search 
    v-model="searchOpen" 
    @manga="addManga" 
    @group="addGroup"
    @user="addUser"
    @list="addList"
    title="What to watch" />
</template>

<script setup lang="ts">
import { MdManga, ResourceType, Watcher, MdResult, MdGroup, MdCustomList, MdUser } from '~/utils/models';
const props = defineProps<{ id: number }>();
const paginator = watcherApi.get(props.id);
const { data, error, pending } = paginator;
const searchOpen = ref(false);

const onScroll = (el: UIEvent) => { paginator.onScroll(<HTMLElement>el.target); }
const newWatch = async () => { searchOpen.value = true; }
const getClass = (type: ResourceType) => {
    switch(type) {
        case ResourceType.Manga: return 'manga';
        case ResourceType.User: return 'user';
        case ResourceType.Group: return 'group';
        case ResourceType.CustomList: return 'list';
    }
    return 'unknown';
}
const deleteItem = async (watcher: Watcher) => {
    const { data } = await watcherApi.delete(watcher.webhookId, watcher.id);
    if (data.value?.result === 'ok') paginator.refresh();
}
const getHref = (watcher: Watcher) => {
    const route = 'https://mangadex.org';
    switch(watcher.type) {
        case ResourceType.Manga: return `${route}/title/${watcher.itemId}`;
        case ResourceType.Group: return `${route}/group/${watcher.itemId}`;
        case ResourceType.User: return `${route}/user/${watcher.itemId}`;
        case ResourceType.CustomList: return `${route}/list/${watcher.itemId}`;
    }
    return route;
}
const proxy = (url: string) => api.proxy(url, 'manga-covers');

const add = async(id: string, type: ResourceType) => {
    const { data } = await watcherApi.post(props.id, id, type);
    if (data.value?.result === 'ok') paginator.refresh();
} 

const addManga = (item: MdManga) => add(item.id, ResourceType.Manga);
const addGroup = (item: MdGroup) => add(item.id, ResourceType.Group);
const addList = (item: MdCustomList) => add(item.id, ResourceType.CustomList);
const addUser = (item: MdUser) => add(item.id, ResourceType.User);
</script>

<style lang="scss" scoped>
.scroll-header {
    padding: var(--margin);

    border-radius: var(--margin);
    background-color: var(--bg-color);
    z-index: 1;
}

.watcher {
    display: flex;
    flex-flow: row;
    margin-top: var(--margin);
    padding: var(--margin);
    background-color: var(--bg-color-accent);
    border-radius: var(--margin);

    .image {
        max-width: 65px;
        margin-right: var(--margin);
        img {
            max-width: 100%;
            max-height: 100%;
        }
    }
    .data {
        flex: 1;
        margin: auto 5px;
        .title, .more {
            display: flex;
            align-items: center;

            .type {
                padding: 3px 5px;
                margin-right: 5px;
                background-color: var(--color-orange);
                border-radius: 5px;
                text-transform: capitalize;
            }

            .text { 
                flex: 1;
                font-weight: bolder;
            }
            .id {
                flex: 1;
                color: var(--color-muted);
            }
        }
    }

    button { padding: var(--margin); }
    &.manga, &.list {
        .image {
            max-width: 150px;
            img { border-radius: 10px; }
        }

        .data {
            margin: 0;
            .title, .more {
                flex-flow: column;
                align-items: normal;
                margin: 0;

                .text, .id, .date, .count { margin-top: var(--margin); }
            }
        }
    }
    &.user, &.group {
        .image {
            img { border-radius: 50%;} 
        }

        .data {
            .title {
                margin-bottom: var(--margin);
                .type {
                    background-color: var(--color-primary);
                }
            }
        }
    }

    &.group .data .title .type {
        background-color: var(--color-secondary);
    }

    &.list .data .title .type {
        background-color: var(--color-muted);
    }
}
</style>