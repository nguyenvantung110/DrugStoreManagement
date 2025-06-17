import { defineStore } from "pinia";

export const useAuthenticationStore = defineStore("authentication", {
  state: () => ({
    homeAccountId: "" as string,
    accessToken: "" as string,
    authenticated: false,
    userInfo: null,
  }),
//   persist: {
//     enabled: true,
//     strategies: [
//       {
//         key: "authentication",
//         storage: sessionStorage,
//       },
//     ],
//   },
  actions: {
    // Log in
    async signIn() {
      //sigin process
      this.authenticated = true;
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
});
