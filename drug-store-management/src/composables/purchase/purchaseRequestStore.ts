import { defineStore } from "pinia";
import { useApi } from "@/composables/common/api-instance";
import { END_POINTS } from "@/endpoints/api-endpoints";

export const usePurchaseRequestStore = defineStore("purchase-request-store", {
  state: () => ({ 
  }),
  actions: {
    async getPurchaseRequestByRequestDate(requestDate: any,success: any) {
      const { get } = useApi();
      await get(END_POINTS.PURCHASE_REQUEST.GET_PURCHASE_REQUEST_BY_REQUEST_DATE({
            pathParams: {
                requestDate: requestDate
            }
        }))
        .then((res) => {
          success(res);
        });
    },
  },
  persist: [
    {
      key: 'purchase-request-store',
      storage: sessionStorage,
    }
  ],
});
