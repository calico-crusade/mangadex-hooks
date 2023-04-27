// import MonacoEditorNlsPlugin, {
//     esbuildPluginMonacoEditorNls,
//     Languages,
// } from 'vite-plugin-monaco-editor-nls';

// https://nuxt.com/docs/api/configuration/nuxt-config
if (process.env.ENV_PROFILE) {
    require("dotenv").config({
      debug: true,
      path: `./.envs/${process.env.ENV_PROFILE}`,
    });
  }

export default defineNuxtConfig({
    app: {
        head: {
            link: [
                { rel: 'preconnect', href: 'https://fonts.gstatic.com' },
                { rel: 'stylesheet', href: 'https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500&amp;display=swap' },
                { rel: 'stylesheet', href: 'https://fonts.googleapis.com/css2?family=Kolker+Brush&display=swap' },
                { rel: 'stylesheet', href: 'https://fonts.googleapis.com/icon?family=Material+Icons' },
                { rel: 'stylesheet', href: 'https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:opsz,wght,FILL,GRAD@20..48,100..700,0..1,-50..200' },
            ],
            noscript: [
                { children: 'JavaScript is required' }
            ]
        },
        pageTransition: { name: 'page', mode: 'out-in' }
    },
    css: [
        '@/node_modules/highlight.js/styles/vs2015.css',
        '@/styles/styles.scss',
    ],
    runtimeConfig: {
        public: {
            apiUrl: process.env.API_URL ?? 'https://hooks-api.index-0.com',
            proxyUrl: process.env.PROXY_URL ?? 'https://cba-proxy.index-0.com'
        }
    },
    components: [
        '~/components/general',
        '~/components/webhooks',
        '~/components/popups',
        '~/components/search',
        '~/components'
    ],
    routeRules: {
        '/webhooks/**': { ssr: false }
    },
    modules: [
        // 'nuxt-monaco-editor'
    ]
});
