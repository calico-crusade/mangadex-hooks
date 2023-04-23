import { LoginResult, UserResult } from "./models";

class AuthApi {

    get currentUser() { return useState<UserResult | undefined>('login-user', () => undefined); }
    get failureReason() { return useState<string | undefined>('login-failure', () => undefined); }

    async bump() {
        if (!process.client) return this.failureReason.value = 'Login is only client side.';

        if(this.currentUser.value) return undefined;

        if (!api.token) {
            this.currentUser.value = undefined;
            return this.failureReason.value = 'User is not logged in.';
        }

        const { result, error } = await this.me();
        if (error.value || !result.value) {
            this.currentUser.value = undefined;
            return this.failureReason.value = 'Couldn\'t fetch user profile.';
        }

        this.currentUser.value = {...result.value};
        return this.failureReason.value = undefined;
    }

    async resolve(code?: string) {
        if (!code) {
            return this.failureReason.value = 'Invalid login code.';
        }

        if (!process.client) {
            return this.failureReason.value = 'Login is only client side.';
        }

        this.failureReason.value = undefined;
        const { result, error, apiError } = await this.code(code);
        if (error.value || apiError.value || !result.value) {
            api.token = undefined;
            this.currentUser.value = undefined;
            console.error('Error occurred during login', { error: error.value, results: result.value });
            const errorMessage = apiError.value?.errors[0] || error.value?.message || '';
            return this.failureReason.value = 'An error occurred during login! ' + errorMessage;
        }

        api.token = result.value.token;
        console.log('Token set to ', { token: api.token });
        return await this.bump();
    }

    async loginUrl() {
        api.redirect = useRouter().currentRoute.value.fullPath;
        const returnUrl = window.location.protocol + '//' + window.location.host + '/auth';

        const { result, error, apiError, data } = await this.url(returnUrl);
        const url = result.value;
        if (!url) {
            console.error('Couldnt find url to redirect to.', { url, error, apiError, data });
            return undefined;
        }

        return url;
    }

    logout() {
        api.token = undefined;
        this.currentUser.value = undefined;
        this.failureReason.value = undefined;
    }

    private url(redirect?: string) { return api.getOne<string>(`auth/url`, { redirect }); }

    private code(code: string) { return api.getOne<LoginResult>(`auth/${code}`); }

    private me() { return api.getOne<UserResult>(`auth`); }
}

export const authApi = new AuthApi();