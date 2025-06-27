import { defineStore } from "pinia";
import { useApi } from "@/composables/common/api-instance";
import { END_POINTS } from "@/endpoints/api-endpoints";

export const useAuthenticationStore = defineStore("authentication-store", {
  state: () => ({
    homeAccountId: "" as string,
    accessToken: "" as string,
    authenticated: false,
    userInfo: null,
  }),
  actions: {
    // Log in
    async login(req: any, success: any) {
      const { post } = useApi();

      await post(END_POINTS.AUTH.LOGIN(),req)
      .then((res) => {
          this.authenticated = true;
          success(res);
          this.accessToken = (res.data as any)?.token;
      });
    },
    // Setters
    setToken(token: string) {
      this.accessToken = token;
    },
  },
  getters: {
    getAccessToken(state) {
      return state.accessToken;
    },
    isAuthenticated(state) {
      return state.authenticated;
    },
    getUserInfo(state) {
      return state.userInfo;
    },
  },
  persist: [
    {
      key: 'authentication-store',
      storage: sessionStorage,
    }
  ],
});
