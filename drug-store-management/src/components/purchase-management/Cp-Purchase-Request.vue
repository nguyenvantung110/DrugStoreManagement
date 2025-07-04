<template>
  <div class="inventory-management pa-4 d-flex flex-xl-row ga-xl-4 h-100">
      <div class="left-bar d-flex flex-xl-column ga-xl-4">
          <calendar-component @update="getPurchaseList"/>
          <div class="inventory-list border-sm rounded-lg flex-xl-grow-1 pa-1">
              <v-data-table-virtual
                  :headers="headers"
                  :items="searchList"
                  height="420"
                  item-value="id"
                  fixed-header
              >
              <template #item="{ item, index }">
                  <tr @click="onRowClick(item, index)" style="cursor:pointer">
                      <td>{{ (item as any)?.createdByName }}</td>
                      <td>{{ formatDate((item as any)?.requestDate) }}</td>
                      <td>{{ (item as any)?.status }}
                        <v-icon color="primary">mdi-checkbox-marked-circle-outline</v-icon>
                        <v-icon color="primary">mdi-minus-circle</v-icon>
                      </td>
                  </tr>
              </template>
              </v-data-table-virtual>
          </div>
      </div>
      <div class="inventory-details border-sm rounded-lg flex-xl-grow-1 pa-2">
          <div class="details-header pa-2">
              <h2 class="text-h5">Chi tiết đơn đặt hàng</h2>
          </div>
          <div class="details-data">
              <v-data-table-virtual
                  :headers="detailsHeaders"
                  :items="detailsList"
                  height="740"
                  item-value="id"
                  fixed-header
              ></v-data-table-virtual>
          </div>
      </div>
  </div>
</template>
<script setup lang="ts">
import calendarComponent from '@/components/common/calendar-component.vue';
import { ref, onMounted } from 'vue';
import { usePurchaseRequestStore } from '@/composables/purchase/purchaseRequestStore';
import { format } from 'date-fns'
import { formatInTimeZone, zonedTimeToUtc } from 'date-fns-tz'

const purchaseRequestStore = usePurchaseRequestStore()

const dummyInventoryList = [
  { id: 1, name: 'Item 1', quantity: 10, createdDate: '2025-06-01 20:00:00' },
  { id: 2, name: 'Item 2', quantity: 5, createdDate: '2025-06-02 11:20:00' },
  { id: 3, name: 'Item 3', quantity: 20, createdDate: '2025-06-20 08:54:00' },
  { id: 4, name: 'Item 4', quantity: 15, createdDate: '2025-06-20 17:22:00' },
  { id: 5, name: 'Item 5', quantity: 8, createdDate: '2025-06-20 09:00:00' },
  { id: 6, name: 'Item 6', quantity: 12, createdDate: '2025-06-20 14:30:00' },
  { id: 7, name: 'Item 7', quantity: 18, createdDate: '2025-06-20 16:45:00' },
  { id: 8, name: 'Item 8', quantity: 25, createdDate: '2025-06-20 10:15:00' },
  { id: 9, name: 'Item 9', quantity: 30, createdDate: '2025-10-20 13:05:00' },
  { id: 10, name: 'Item 10', quantity: 22, createdDate: '2025-10-10 19:30:00' },
  { id: 11, name: 'Item 11', quantity: 17, createdDate: '2025-10-15 21:00:00' },
  { id: 12, name: 'Item 12', quantity: 9, createdDate: '2025-06-20 12:45:00' },
  { id: 13, name: 'Item 13', quantity: 14, createdDate: '2025-06-20 15:30:00' },
  { id: 14, name: 'Item 14', quantity: 6, createdDate: '2025-06-20 18:20:00' },
  { id: 15, name: 'Item 15', quantity: 11, createdDate: '2025-06-20 09:10:00' }

];

const headers = ref([
  { title: 'Người tạo', key: 'createdByName'},
  { title: 'Thời gian tạo', key: 'requestDate' },
  { title: 'Trạng thái', key: 'status'},
])

const detailsHeaders = ref([
{ title: 'ID', key: 'id'},
{ title: 'Tên thuốc', key: 'name' },
{ title: 'Ngày nhập', key: 'inputDate'},
{ title: 'Ngày hết hạn', key: 'expiredDate'},
{ title: 'Số lượng', key: 'quantity'},
])

const searchList = ref<any>([]);
const detailsList = ref<any>([]);

onMounted(async () => {
  await getPurchaseList()
})

const getPurchaseList = async(requestDate: any = new Date()) => {
  const convertDate = formatDateToYMDHMS(requestDate);

  await purchaseRequestStore.getPurchaseRequestByRequestDate(convertDate, (res) => {
    searchList.value = res?.data
  })
}

const formatDate = (val: any) => {
  const clientTimeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
  const formatted = formatInTimeZone(val, clientTimeZone, 'HH:mm:ss');
  return formatted;
}

// const selectDate = async(val: any) => {
//   const convertDate = formatDateToYMDHMS(val);
//   getPurchaseList();
// }

function onRowClick(item: any, index: number) {
// Xử lý khi click dòng
dummyDetails();
console.log(detailsList.value)
}

const dummyDetails = () => {
detailsList.value =
Array.from({ length: 1000 }, (_, i) => ({
  id: i + 1,
  name: `Tên thuốc ${i + 1}`,
  inputDate: `Ngày nhâp ${i + 1}`,
  expiredDate: `Ngày hết hạn ${i + 1}`,
  quantity: Math.floor(Math.random() * 100) + 1,
  supplier: `Nhà cung cấp ${i + 1}`,
}))
}

function formatDateToYMDHMS(date: Date): string {
const pad = (n: number) => n.toString().padStart(2, '0');
return [
  date.getFullYear(),
  pad(date.getMonth() + 1),
  pad(date.getDate())
].join('-') + ' ' +
[
  pad(date.getHours()),
  pad(date.getMinutes()),
  pad(date.getSeconds())
].join(':');
}
</script>
<style scoped>
  
</style>