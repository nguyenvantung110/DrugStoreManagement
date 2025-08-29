type Params = Record<string, string | number | null | undefined>;

interface EndpointParams {
  pathParams?: Params;
  queryParams?: Params;
}

const createEndpoint =
  (template: string) =>
  ({ pathParams = {}, queryParams = {} }: EndpointParams = {}) => {
    let url = Object.entries(pathParams).reduce(
      (url, [key, value]) =>
        url.replace(`:${key}`, encodeURIComponent(String(value))),
      template,
    );

    const queryString =
      queryParams !== undefined
        ? Object.entries(queryParams)
            .map(
              ([key, value]) =>
                `${encodeURIComponent(key)}=${encodeURIComponent(
                  String(value),
                )}`,
            )
            .join("&")
        : null;

    if (queryString) {
      url += `?${queryString}`;
    }

    return url;
  };

export const END_POINTS = {
  AUTH: {
    LOGIN: createEndpoint("/api/auth/login"),
  },
  USER: {
    GET_USER_LIST: createEndpoint("/api/users"),
    GET_USER_INFO: createEndpoint("api/users/:id"),
    UPDATE_USER_INFO: createEndpoint("api/users/update"),
    CREATE_USER: createEndpoint("api/users/create"),
    DELETE_USER: createEndpoint("api/users/delete/:id"),
  },
  SUPPLIERS: {
    GET_LIST: createEndpoint("/api/suppliers"),
    GET_BY_ID: createEndpoint("/api/suppliers/:id"),
    CREATE: createEndpoint("/api/suppliers/create"),
    UPDATE: createEndpoint("/api/suppliers/update"),
    DELETE: createEndpoint("/api/suppliers/delete"),
  },
  PURCHASE_REQUEST: {
    GET_PURCHASE_REQUEST_BY_REQUEST_DATE: createEndpoint("/api/purchase-request/get-by-created-date/:requestDate"),
  },
  PRODUCT: {
    GET_BASIC_INFO_LIST: createEndpoint("/api/products/get-basic-info-list"),
  },
  PRESCRIPTION: {
    GET_PRESCRIPTION_TEMPLATES: createEndpoint("/api/prescriptions/get-templates"),
    CREATE_PRESCRIPTION_TEMPLATE: createEndpoint("/api/prescriptions/create-template"),
    UPDATE_PRESCRIPTION_TEMPLATE: createEndpoint("/api/prescriptions/update-template"),
    DELETE_PRESCRIPTION_TEMPLATE: createEndpoint("/api/prescriptions/delete-template/:id"),
  },
  CATEGORIES: {
    GET_ALL_CATEGORIES: createEndpoint("/api/categories/get-all"),
    GET_CATEGORY_BY_TYPE: createEndpoint("/api/categories/get-by-type"),
    CREATE_CATEGORY: createEndpoint("/api/categories/create"),
    UPDATE_CATEGORY: createEndpoint("/api/categories/update"),
    DELETE_CATEGORY: createEndpoint("/api/categories/delete/:id"),
  },
  SALE_ORDERS: {
    CREATE_ORDER: createEndpoint("/api/sales-orders/create-order"),
  }
};
