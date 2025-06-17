import { createApp, markRaw } from 'vue'
import App from './App.vue'
import router from './router'
import { createPinia } from 'pinia';

// Vuetify
import 'vuetify/styles'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'

const app = createApp(App);

const pinia = createPinia();

pinia.use(({ store }) => {
    store.router = markRaw(router);
});

//pinia.use(piniaPluginPersist);

const vuetify = createVuetify({
    components,
    directives,
})

app.use(pinia);
app.use(router);
app.use(vuetify);

app.mount('#app')
