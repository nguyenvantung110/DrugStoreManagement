import { defineStore } from "pinia";
import { useApi } from "@/composables/common/api-instance";
import { END_POINTS } from "@/endpoints/api-endpoints";

export const useSupplierStore = defineStore("supplier-store", {
  state: () => ({ 
  }),
  actions: {
    async getSupplierList(success: any) {
      const { get } = useApi();
      await get(END_POINTS.SUPPLIERS.GET_LIST())
        .then((res) => {
          success(res);
        });
    },
    async getSupplierById(supplierId: string, success: any) {
      const { get } = useApi();
      await get(END_POINTS.SUPPLIERS.GET_BY_ID({
        pathParams: { id: supplierId },
      }))
        .then((res) => {
          success(res);
        });
    },
    async createSupplier(supplierInfo: any) {
      const { post } = useApi();
      await post(END_POINTS.SUPPLIERS.CREATE(), supplierInfo)
    },
    async updateSupplier(supplierInfo: any) {
      const { post } = useApi();
      await post(END_POINTS.SUPPLIERS.UPDATE(), supplierInfo)
    },
    async deleteSupplier(supplierId: string) {
      const { post } = useApi();
      await post(END_POINTS.SUPPLIERS.DELETE(), supplierId)
    },
  },
  persist: [
    {
      key: 'supplier-store',
      storage: sessionStorage,
    }
  ],
});
