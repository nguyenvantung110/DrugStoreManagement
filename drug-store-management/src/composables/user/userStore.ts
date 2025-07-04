import { defineStore } from "pinia";
import { useApi } from "@/composables/common/api-instance";
import { END_POINTS } from "@/endpoints/api-endpoints";

export const useUserStore = defineStore("user-store", {
  state: () => ({
    userInfo: {},
  }),
  actions: {
    async getUserInfo(userId: string, success: any) {
      const { get } = useApi();

      await get(END_POINTS.USER.GET_USER_INFO({pathParams: { id: userId }}))
      .then((res) => {
          success(res);
          this.userInfo = res.data as any;
      });
    },
    async updateUserInfo(userInfo: any) {
        const { post } = useApi();
  
        await post(END_POINTS.USER.UPDATE_USER_INFO(), userInfo);
    },
  },
  getters: {
    getterUserInfo(state) {
      return state.userInfo;
    },
  },
  persist: [
    {
      key: 'user-store',
      storage: sessionStorage,
    }
  ],
});
