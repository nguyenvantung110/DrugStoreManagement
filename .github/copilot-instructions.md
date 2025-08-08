# Hướng dẫn Copilot cho API Quản lý Nhà thuốc

## Tổng quan dự án
Đây là API backend cho hệ thống quản lý nhà thuốc, xử lý các nghiệp vụ như quản lý kho thuốc, xử lý đơn thuốc, theo dõi bán hàng và quản lý khách hàng.

## Công nghệ sử dụng
- **Backend Framework**: Node.js với Express.js / ASP.NET Core
- **Cơ sở dữ liệu**: PostgreSQL/MySQL
- **Xác thực**: JWT tokens
- **Tài liệu API**: Swagger/OpenAPI
- **Kiểm thử**: Jest/Mocha

## Cấu trúc dự án
```
drug-store-api/
├── controllers/     # Xử lý các endpoint API
├── models/         # Mô hình dữ liệu và schema
├── services/       # Logic nghiệp vụ
├── middleware/     # Middleware tùy chỉnh
├── config/         # File cấu hình
├── utils/          # Hàm tiện ích
├── tests/          # Kiểm thử unit và tích hợp
├── docs/           # Tài liệu API
└── database/       # Migration và seed database
```

## Tính năng chính cần triển khai
1. **Quản lý kho thuốc**
   - Danh mục thuốc với theo dõi hạn sử dụng
   - Giám sát mức tồn kho
   - Quản lý nhà cung cấp
   - Theo dõi lô hàng

2. **Xử lý đơn thuốc**
   - Xác thực đơn thuốc
   - Kiểm tra thông tin bác sĩ
   - Theo dõi lịch sử bệnh nhân
   - Tính toán liều lượng

3. **Bán hàng & Thanh toán**
   - Hệ thống bán hàng
   - Tạo hóa đơn
   - Xử lý thanh toán
   - Quản lý giảm giá

4. **Quản lý người dùng**
   - Phân quyền theo vai trò (Admin, Dược sĩ, Thu ngân)
   - Hồ sơ khách hàng
   - Quản lý nhân viên

## Tiêu chuẩn viết mã
- Sử dụng quy ước đặt tên nhất quán (camelCase cho biến, PascalCase cho class)
- Xử lý lỗi toàn diện
- Viết commit message có ý nghĩa
- Thêm documentation cho tất cả method public
- Tuân theo nguyên tắc RESTful API
- Validate tất cả input
- Sử dụng biến môi trường cho cấu hình

## Hướng dẫn bảo mật
- Triển khai xác thực và phân quyền đúng cách
- Validate và sanitize tất cả input
- Sử dụng HTTPS cho mọi giao tiếp
- Triển khai rate limiting
- Ghi log các sự kiện bảo mật
- Xử lý dữ liệu nhạy cảm (đơn thuốc, thông tin cá nhân) cẩn thận

## Mẫu thiết kế API
- Sử dụng format response nhất quán
- Triển khai HTTP status code đúng cách
- Phiên bản hóa API (v1, v2, v.v.)
- Pagination cho các endpoint danh sách
- Response lỗi phù hợp

## Yêu cầu kiểm thử
- Viết unit test cho tất cả logic nghiệp vụ
- Bao gồm integration test cho API endpoint
- Mock các dependency bên ngoài
- Đạt tối thiểu 80% code coverage
- Kiểm thử các tình huống lỗi và edge case

## Cân nhắc Database
- Sử dụng transaction cho các thao tác multi-table
- Triển khai indexing phù hợp cho hiệu suất
- Xử lý migration cẩn thận
- Backup dữ liệu quan trọng thường xuyên
- Tuân thủ GDPR cho dữ liệu cá nhân


# Hướng dẫn Copilot cho Frontend Quản lý Nhà thuốc

## Tổng quan dự án
Đây là ứng dụng frontend cho hệ thống quản lý nhà thuốc sử dụng Vue 3 + TypeScript, quản lý trạng thái với Pinia và giao diện với Vuetify. Cung cấp giao diện thân thiện cho dược sĩ, quản trị viên và nhân viên quản lý các hoạt động nhà thuốc.

## Công nghệ sử dụng
- **Frontend Framework**: Vue 3 với Composition API
- **Ngôn ngữ**: TypeScript
- **Quản lý trạng thái**: Pinia (API setup-style)
- **UI Framework**: Vuetify
- **Styling**: Tailwind CSS + Scoped CSS
- **Build Tool**: Vite
- **Kiểm thử**: Jest (unit), Cypress (E2E)
- **Biểu đồ**: Chart.js cho phân tích

## Cấu trúc dự án
```
drug-store-management/
├── src/
│   ├── components/        # Component UI có thể tái sử dụng
│   │   ├── common/       # Component dùng chung
│   │   ├── sales/        # Component bán hàng
│   │   ├── inventory/    # Component quản lý kho
│   │   └── suppliers/    # Component nhà cung cấp
│   ├── views/            # Các trang chính
│   ├── composables/      # Logic và store Pinia
│   ├── models/           # TypeScript interfaces
│   ├── constants/        # API endpoints và constants
│   ├── styles/           # CSS toàn cục và themes
│   └── router/           # Vue Router config
├── tests/
│   ├── unit/            # Unit tests
│   └── e2e/             # Cypress E2E tests
└── public/              # Tài nguyên tĩnh
```

## Quy trình làm việc chính
- **Cài đặt:** `npm install`
- **Chạy dev:** `npm run serve` (hot reload)
- **Build:** `npm run build`
- **Unit test:** `npm run test:unit`
- **E2E test:** `npm run test:e2e`
- **Lint:** `npm run lint`

## Modules chính cần triển khai
1. **Dashboard**
   - Tổng quan bán hàng và phân tích
   - Cảnh báo tồn kho và thông báo
   - Truy cập nhanh các tác vụ thường dùng
   - Cập nhật real-time

2. **Quản lý kho thuốc**
   - Danh mục thuốc với tìm kiếm và lọc
   - Giao diện quản lý tồn kho
   - Theo dõi hạn sử dụng
   - Quản lý nhà cung cấp

3. **Xử lý đơn thuốc**
   - Nhập và validate đơn thuốc
   - Tra cứu bệnh nhân và lịch sử
   - Cảnh báo tương tác thuốc
   - In đơn thuốc

4. **Giao diện bán hàng**
   - Hệ thống POS
   - Tích hợp quét mã vạch
   - Tạo hóa đơn
   - Xử lý thanh toán

5. **Báo cáo & Phân tích**
   - Báo cáo bán hàng và xu hướng
   - Báo cáo tồn kho
   - Tổng kết tài chính
   - Tính năng xuất file

## Quy ước & Tiêu chuẩn mã nguồn
- **Pinia Store**: Luôn sử dụng API setup-style
```typescript
export const useSalesStore = defineStore('sales-store', () => {
  // state
  const sales = ref<Sale[]>([])
  
  // actions
  const fetchSales = async () => { ... }
  
  return { sales, fetchSales }
})
```

- **Component Structure**: Nhóm theo domain, component chung ở `common/`
- **TypeScript**: Tất cả code dùng TypeScript, định nghĩa interface ở `src/models/`
- **Styling**: Ưu tiên Tailwind utility classes và Vuetify components
- **API Integration**: Endpoint ở `src/constants/endpoints/api-endpoints.ts`

## Hướng dẫn UI/UX
- Tuân theo nguyên tắc Material Design của Vuetify
- Đảm bảo responsive design cho tablet và desktop
- Triển khai tiêu chuẩn accessibility (WCAG 2.1)
- Sử dụng color scheme và typography nhất quán
- Cung cấp navigation rõ ràng và breadcrumbs
- Bao gồm loading states và error handling
- Phím tắt cho các thao tác thường dùng

## Tiêu chuẩn Component
- Tạo component có thể tái sử dụng, modular
- Sử dụng TypeScript interfaces cho props
- Tuân theo nguyên tắc single responsibility
- Quy ước đặt tên nhất quán (PascalCase cho component)
- Bao gồm documentation cho component

## Quản lý State với Pinia
- Giữ global state tối thiểu
- Sử dụng local component state khi có thể
- Triển khai data flow patterns đúng cách
- Xử lý loading và error states nhất quán
- Cache dữ liệu truy cập thường xuyên
- Triển khai optimistic updates phù hợp

## Tích hợp API
- Sử dụng service layer nhất quán cho API calls
- Triển khai error handling phù hợp
- Thêm request/response interceptors
- Xử lý authentication tokens
- Triển khai retry logic cho failed requests
- Sử dụng loading indicators phù hợp

## Tối ưu hiệu suất
- Triển khai code splitting và lazy loading
- Tối ưu images và assets
- Sử dụng caching strategies phù hợp
- Giảm thiểu bundle size
- Triển khai virtual scrolling cho danh sách lớn
- Debounce cho search inputs

## Cân nhắc bảo mật
- Validate tất cả user inputs
- Sanitize dữ liệu trước khi hiển thị
- Triển khai kiểm tra authentication đúng cách
- Sử dụng HTTPS cho tất cả API calls
- Xử lý dữ liệu nhạy cảm an toàn
- Triển khai session timeout

## Chiến lược kiểm thử
- Viết unit tests cho components và utilities
- Bao gồm integration tests cho user flows
- Kiểm thử accessibility features
- Mock API calls trong tests
- Kiểm thử error scenarios
- Duy trì test coverage trên 80%

## Ví dụ tham khảo
- Xem `src/components/sales/Cp-Sales.vue` cho ví dụ component
- Xem `src/composables/sales/salesStore.ts` cho ví dụ Pinia store
- Kiểm tra `src/constants/endpoints/api-endpoints.ts` cho API integration

## Lưu ý quan trọng
- Luôn sử dụng setup-style API của Pinia (không dùng options-style)
- Định nghĩa TypeScript interface cho mọi model dữ liệu
- Ưu tiên composable cho logic và state dùng chung
- Sử dụng cấu trúc thư mục theo tính năng để dễ mở rộng