<template>
<div class="search fill-parent flex row scroll-y" @scroll="onScroll">
    <div class="scroll-header">
        <div class="control no-top">
            <div class="group">
                <input class="fill" v-model="search" placeholder="Search for a scanlation group" @keyup.prevent="doSearch(search)" />
                <button @click="doSearchAct(search)">
                    <Icon>search</Icon>
                </button>
            </div>
        </div>
    </div>
    <a class="group-item" v-for="g in groups" @click="$emit('group', g)">
        <div class="image">
            <img src="https://mangadex.org/img/avatar.png" />
        </div>
        <div class="content">
            <div class="title">{{ g.attributes.name }}</div>
            <div class="id">ID: {{ g.id }}</div>
        </div>
    </a>
    <Loading v-if="pending" inline />
</div>
</template>

<script setup lang="ts">
import { MdGroup } from '~/utils/models';

interface Emits { (e: 'group', group: MdGroup): void; }
defineEmits<Emits>();

const search = ref('');
const params = { search: '', size: 20 };
const paginator = new PaginationHelper<MdGroup>(`md/group`, params, 1, () => !!params.search);
const { data: groups, pending } = paginator;

const doSearchAct = async (input: string) => {
    if (!input || input.length < 3) return;
    params.search = search.value;
    paginator.refresh();
}

const doSearch = debounce(doSearchAct, 500);
const onScroll = (el: UIEvent) => paginator?.onScroll(<any>el.target);
</script>

<style lang="scss">
.group-item {
    margin-top: 5px;
    display: flex;
    flex-flow: row;
    background-color: var(--bg-color-accent);
    border-radius: 5px;
    max-height: 300px;
    padding: var(--margin);

    .image {
        max-width: 45px;
        margin-right: var(--margin);
        img {
            max-width: 100%;
            max-height: 100%;
            border-radius: 50%;
        }
    }

    .content {
        flex: 1;
        margin: auto 5px;

        .title { font-weight: bold; }
        .id { color: var(--color-muted); }
    }
}
</style>