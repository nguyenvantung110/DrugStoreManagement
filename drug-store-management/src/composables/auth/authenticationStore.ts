import { defineStore } from "pinia";
import { useApi } from "@/composables/common/api-instance";
import { END_POINTS } from "@/endpoints/api-endpoints";

export const useAuthenticationStore = defineStore("authentication-store", {
  state: () => ({
    accessToken: "" as string,
    authenticated: false,
    userId: "" as string,
  }),
  actions: {
    // Log in
    async login(req: any, success: any) {
      const { post } = useApi();
      try {
        await post(END_POINTS.AUTH.LOGIN(),req)
        .then((res) => {
            this.authenticated = true;
            success(res);
            this.accessToken = (res.data as any)?.token;
            this.userId = (res.data as any)?.userId;
        });
      } catch (e) {
        if (success) success(null);
      }
    },
    // Setters
    setToken(token: string) {
      this.accessToken = token;
    }
  },
  getters: {
    getAccessToken(state) {
      return state.accessToken;
    },
    isAuthenticated(state) {
      return state.authenticated;
    },
    getterUserId(state) {
      return state.userId;
    }
  },
  persist: [
    {
      key: 'authentication-store',
      storage: sessionStorage,
    }
  ],
});
