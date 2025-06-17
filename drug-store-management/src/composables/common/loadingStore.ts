import { defineStore } from "pinia";

export const useLoadingStore = defineStore("loadingStore", {
  state: () => ({ isShow: false }),
  actions: {
    show() {
      //show loading animation
      this.isShow = true;
    },
    hide() {
      //hide loading animation
      this.isShow = false;
    },
  },
});
