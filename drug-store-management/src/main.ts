import { createApp, markRaw } from 'vue'
import App from './App.vue'
import router from './router'
import { createPinia } from 'pinia';
import '@mdi/font/css/materialdesignicons.min.css'

// Vuetify
import 'vuetify/styles'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import { aliases, mdi } from 'vuetify/iconsets/mdi'
import VueApexCharts from "vue3-apexcharts";
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'

const app = createApp(App);

const pinia = createPinia();

pinia.use(({ store }) => {
    store.router = markRaw(router);
});

pinia.use(piniaPluginPersistedstate)

const vuetify = createVuetify({
    components,
    directives,
    icons: {
      defaultSet: 'mdi',
      aliases,
      sets: {
        mdi,
      },
    },
})

app.use(pinia);
app.use(router);
app.use(vuetify);
//app.use(VueApexCharts);
app.component("apexchart", VueApexCharts);

app.mount('#app')
