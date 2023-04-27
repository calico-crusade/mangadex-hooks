<template>
<div class="search fill-parent flex row scroll-y">
    <div class="scroll-header">
        <div class="control no-top">
            <div class="group">
                <input class="fill" v-model="id" placeholder="User Id" />
                <button @click="doSearch()">
                    <Icon>search</Icon>
                </button>
            </div>
        </div>
    </div>
    <a class="user-list" v-if="!pending && user" @click="$emit('user', user)">
        <div class="image">
            <img src="https://mangadex.org/img/avatar.png" />
        </div>
        <div class="content">
            <div class="title">{{ user.attributes.username }}</div>
            <div class="id">ID: {{ user.id }}</div>
            <div class="count">Roles: {{ user.attributes.roles.join(', ') }}</div>
        </div>
    </a>
    <Loading v-if="pending" />
</div>
</template>

<script setup lang="ts">
import { MdUser } from '~/utils/models';
interface Emits { (e: 'user', list: MdUser): void; }
defineEmits<Emits>();
const id = ref('');
const pending = ref(false);
const user = ref<MdUser | undefined>(undefined);

const doSearch = async () => {
    pending.value = true;
    const { result } = await mdApi.user(id.value);
    user.value = result.value;
    pending.value = false;
};
</script>

<style lang="scss" scoped>
.user-list {
    margin-top: 5px;
    display: flex;
    flex-flow: row;
    background-color: var(--bg-color-accent);
    border-radius: 5px;
    max-height: 300px;
    padding: var(--margin);

    .image {
        max-width: 100px;
        margin-right: var(--margin);
        img {
            max-width: 100%;
            max-height: 100%;
            border-radius: 5px;
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