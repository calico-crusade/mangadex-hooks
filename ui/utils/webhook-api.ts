import { ApiResult, UpsertResult, Webhook } from "./models";

class WebhookApi {

    paginate(page: number = 1, size: number = 100) {
        return new PaginationHelper<Webhook>('webhook', { size }, page);
    }

    get(id: number) {
        return api.getOne<Webhook>(`webhook/${id}`);
    }

    post(hook: Webhook) {
        return api.post<UpsertResult>(`webhook`, hook);
    }

    test(id: number, chapterId?: string) {
        return api.get<ApiResult>(`webhook/${id}/test`, { chapterId });
    }
}

export const webhooksApi = new WebhookApi();