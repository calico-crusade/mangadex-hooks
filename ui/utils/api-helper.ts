import { CollectionResult, FailureResult, SuccessResult } from "./models";
import { AsyncDataExecuteOptions } from "nuxt/dist/app/composables/asyncData";
import { FetchError } from "ofetch";

const STORAGE_TOKEN = 'Hooks-AuthToken';
const STORAGE_URL = 'Hooks-RedirectUrl';

export type Params = { [key: string]: any };

type HttpVerbs = "GET" | "POST" | "PUT" | "DELETE";

//A shim of Nuxt3's AsyncData<T, E> that doesn't type merge a promise (????)
interface _AsyncData<DataT, ErrorT> {
    /**
     * The raw JSON results of the response with no MD unwrapping.
     */
    data: Ref<DataT | undefined>;
    /**
     * A watchable boolean representing the state of the request
     */
    pending: Ref<boolean>;
    /**
     * Allows for retriggering the request with the same initial state to refresh the data
     */
    refresh: (opts?: AsyncDataExecuteOptions) => Promise<void>;
    /**
     * Executes the request
     */
    execute: (opts?: AsyncDataExecuteOptions) => Promise<void>;
    /**
     * Represents the possible error state of the response
     */
    error: Ref<ErrorT | undefined>;
}

export interface ErrorsAsync<T> extends _AsyncData<T, FetchError<any> | null> {
    apiError: Ref<FailureResult | undefined>;
}

export interface ResultsAsync<T> extends ErrorsAsync<SuccessResult<T>> {
    result: Ref<T | undefined>;
}

export interface CollectionAsync<T> extends ErrorsAsync<CollectionResult<T>> {
    results: Ref<T[] | undefined>;
    count: Ref<number>;
    pages: Ref<number>;
}

class ApiHelper {

    get token() { 
        if (this._token) return this._token;
        if (!process.client) return undefined;
        return localStorage.getItem(STORAGE_TOKEN) || undefined;
    }
    set token(token: string | undefined) { 
        this._token = token;
        if (!process.client) return;

        if (!token) localStorage.removeItem(STORAGE_TOKEN);
        else localStorage.setItem(STORAGE_TOKEN, token);
    }

    get redirect() { return localStorage.getItem(STORAGE_URL) || undefined; }
    set redirect(url: string | undefined) { 
        if (!url) localStorage.removeItem(STORAGE_URL);
        else localStorage.setItem(STORAGE_URL, url);
    }

    get apiUrl() { return this._apiUrl || useRuntimeConfig().public.apiUrl; }
    get proxyUrl() { return this._proxyUrl || useRuntimeConfig().public.proxyUrl; }

    constructor(
        private _token?: string,
        private _apiUrl?: string,
        private _proxyUrl?: string
    ) {}

    private headers() {
        if (!this.token) return undefined;
        return { 'Authorization': `Bearer ${this.token}` };
    }

    private wrapUrl(url: string) {
        if (url.toLowerCase().indexOf('https://') !== -1 || 
            url.toLowerCase().indexOf('http://') !== -1) return url;

        if (url.startsWith('/')) url = url.substring(1);
        if (url.endsWith('?')) url = url.substring(0, url.length - 1);

        return `${this.apiUrl}/${url}`;
    }

    private unwrap<T>(fetchData: ErrorsAsync<SuccessResult<T>>) : ResultsAsync<T> {
        const { data } = fetchData;
        const result = computed(() => {
            return data.value?.result === 'ok' && 'data' in data.value ? data.value.data : undefined;
        });
        return <ResultsAsync<T>>{ ...fetchData, result };
    }

    private unwrapMany<T>(fetchData: ErrorsAsync<CollectionResult<T>>) : CollectionAsync<T> {
        const { data } = fetchData;
        const results = computed(() => {
            return data.value?.result === 'ok' && 'data' in data.value ? data.value.data : [];
        });
        const count = computed(() => {
            return data.value?.result === 'ok' ? data.value.count : 0;
        });
        const pages = computed(() => {
            return data.value?.result === 'ok' ? data.value.pages : 0;
        })
        return <CollectionAsync<T>>{ ...fetchData, results, count, pages };
    }

    private unwrapErrors<T>(fetchData: _AsyncData<T, FetchError<any>>): ErrorsAsync<T> {
        const { error, data } = fetchData;
        const apiError = computed(() => {
            const target: FailureResult = data.value || error.value?.data;
            return target?.result === 'error' ? target : undefined;
        });

        return {...fetchData, apiError };
    }

    private async request<T>(route: string, method: HttpVerbs, body?: any, params?: Params, lazy?: boolean): Promise<_AsyncData<T, FetchError<any>>> {
        const url = this.wrapUrl(route);
        const options = {
            headers: this.headers(),
            params: params,
            method,
            body
        };

        return await <any>(lazy ? useLazyFetch<T>(url, options) : useFetch<T>(url, <any>options));
    }

    async get<T>(url: string, params?: Params, lazy: boolean = false) {
        const res = await this.request<T>(url, 'GET', null, params, lazy);
        return this.unwrapErrors(res);
    }

    async getOne<T>(url: string, params?: Params, lazy?: boolean) {
        const res = await this.get<SuccessResult<T>>(url, params, !!lazy);
        return this.unwrap(res);
    }

    async getMany<T>(url: string, params?: Params, lazy?: boolean) {
        const res = await this.get<CollectionResult<T>>(url, params, !!lazy);
        return this.unwrapMany(res);
    }

    async post<T>(url: string, body: any, params?: Params, lazy: boolean = false): Promise<ErrorsAsync<T>> {
        const res = await this.request<T>(url, 'POST', body, params, lazy);
        return this.unwrapErrors(res);
    }

    async postOne<T>(url: string, body: any, params?: Params, lazy: boolean = false) {
        const res = await this.post<SuccessResult<T>>(url, body, params, lazy);
        return this.unwrap(res);
    }

    async postMany<T>(url: string, body: any, params?: Params, lazy: boolean = false) {
        const res = await this.post<CollectionResult<T>>(url, body, params, lazy);
        return this.unwrapMany(res);
    }

    async put<T>(url: string, body: any, params?: Params, lazy: boolean = false) {
        const res = await this.request<T>(url, 'PUT', body, params, lazy);
        return this.unwrapErrors(res);
    }

    async putOne<T>(url: string, body: any, params?: Params, lazy: boolean = false) {
        const res = await this.put<SuccessResult<T>>(url, body, params, lazy);
        return this.unwrap(res);
    }

    async putMany<T>(url: string, body: any, params?: Params, lazy: boolean = false) {
        const res = await this.put<CollectionResult<T>>(url, body, params, lazy);
        return this.unwrapMany(res);
    }

    async delete<T>(url: string, params?: Params, lazy: boolean = false) {
        const res = await this.request<T>(url, 'DELETE', null, params, lazy);
        return this.unwrapErrors(res);
    }

    proxy(url: string, group: string = 'manga-page', referer?: string) {
        const path = encodeURIComponent(url);
        let uri = `${this.proxyUrl}/proxy?path=${path}&group=${group}`;

        if (referer) uri += `&referer=${encodeURIComponent(referer)}`;

        return uri;
    }
}

export const api = new ApiHelper();

export function isCodeSuccess(code: number) {
    return code >= 200 && code < 300;
}

export class Bytes {
    static bitLength(number: number) {
        return Math.floor(Math.log2(number)) + 1;
    }
      
    static byteLength(number: number) {
        return Math.ceil(Bytes.bitLength(number) / 8);
    }
    
    static to(number: number) {
        if (!Number.isSafeInteger(number)) {
            throw new Error("Number is out of range");
        }
    
        const size = number === 0 ? 0 : Bytes.byteLength(number);
        const bytes = new Uint8ClampedArray(size);
        let x = number;
        for (let i = (size - 1); i >= 0; i--) {
            const rightByte = x & 0xff;
            bytes[i] = rightByte;
            x = Math.floor(x / 0x100);
        }
    
        return Array.from(bytes);
    }
    
    static from(buffer: number[]) {
        const bytes = new Uint8ClampedArray(buffer);
        const size = bytes.byteLength;
        let x = 0;
        for (let i = 0; i < size; i++) {
            const byte = bytes[i];
            x *= 0x100;
            x += byte;
        }
        return x;
    }
}

export function debounce<T>(fun: (arg: T) => void, wait: number)  {
    let timer: NodeJS.Timer;
    return (arg: T) => {
        if (timer) clearTimeout(timer);
        timer = setTimeout(() => {
            fun(arg);
        }, wait);
    }
}

export function throttle(fun: Function, wait: number) {
    let throttled = false;
    return function(...args: any) {
        if (throttled) return;

        fun(...args);
        throttled = true;
        setTimeout(() => {
            throttled = false;
        }, wait);
    }
}