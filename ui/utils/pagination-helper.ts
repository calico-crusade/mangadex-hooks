import { Params } from "./api-helper";
import { CollectionResult, FailureResult } from "./models";

export class PaginationHelper<T> {

    results = ref<CollectionResult<T>>({ pages: 0, count: 0, data: [], result: 'ok', code: 200 });
    pending = ref(false);
    error = ref<FailureResult | undefined>(undefined);

    get page() { return this._page ?? 1; }
    set page(value: number) {
        this._page = value;
        this.fetch();
    }

    constructor(
        private _url: string,
        private _params?: Params,
        private _page?: number,
        public infinite: boolean = false
    ) { this.fetch(true); }

    private async fetch(reset: boolean = false) {
        if (this.pending.value) return;

        if (reset || this.infinite) {
            this.results.value.data = [];
            this.results.value.count = 0;
            this.results.value.pages = 0;
        }

        this.pending.value = true;
        const { data: results, apiError, error } = await api.getMany<T>(this._url, {
            ...this._params,
            page: this.page
        });

        this.pending.value = false;
        this.error.value = apiError.value;

        if (error.value && !apiError.value) {
            this.error.value = {
                code: 500,
                result: 'error',
                errors: [ error.value.message ] 
            };
        }

        if (!results.value) return;

        const unbound = {...results.value};
        const current = <T[]>[...this.results.value.data];
        this.results.value.pages = unbound?.pages;
        this.results.value.count = unbound?.count;
        this.results.value.data = <any>[...current, ...unbound?.data];
    }

    next() { this.page++; }
    prev() { this.page--; }
    set(page: number) { this.page = page; }

    onScroll(element: HTMLElement) {
        if (!element) return;

        const curRes = this.results.value;
        if (!curRes || curRes.pages <= this.page || this.pending.value) return;

        const bottom = element.scrollTop + element.clientHeight >= element.scrollHeight;
        if (!bottom) return;
        
        this.page++;
    }
}