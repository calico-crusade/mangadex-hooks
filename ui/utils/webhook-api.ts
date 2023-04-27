import { ErrorsAsync, ResultsAsync } from "./api-helper";
import { DiscordWebhook, MessageResult, SuccessResult, UpsertResult, Webhook, WebhookHistory, WebhookHistoryWithRes } from "./models";

class WebhookApi {

    paginate(page: number = 1, size: number = 100) {
        return new PaginationHelper<Webhook>('webhook', { size }, page);
    }

    get(id: number) {
        return api.getOne<Webhook>(`webhook/${id}`, undefined, true);
    }

    post(hook: Webhook) {
        return api.post<UpsertResult>(`webhook`, hook);
    }

    test(script: string, chapterId?: string): Promise<ResultsAsync<DiscordWebhook>>;
    test(id: number, chapterId?: string): Promise<ErrorsAsync<MessageResult>>;
    test(item: number | string, chapterId?: string) {
        if (typeof item === 'string') {
            return api.postOne<DiscordWebhook>(`webhook/test`, {
                script: item,
                chapterId
            });
        }

        return api.get<MessageResult>(`webhook/${item}/test`, { chapterId });
    }

    shim() {
        return api.get<SuccessResult<{ shim: string, default: string }>>(`webhook/shim`, { wrap: true }, true);
    }

    history(id: number, page: number = 1, size: number = 100) {
        return new PaginationHelper<WebhookHistory>(`webhook/${id}/history`, { size }, page);
    }

    historyByOwner(page: number = 1, size: number = 100) {
        return new PaginationHelper<WebhookHistoryWithRes>(`webhook/history`, { size }, page);
    }
}

export const webhooksApi = new WebhookApi();