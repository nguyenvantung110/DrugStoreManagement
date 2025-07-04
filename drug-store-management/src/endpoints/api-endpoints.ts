type Params = Record<string, string | number>;

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
    GET_USER_INFO: createEndpoint("api/users/:id"),
    UPDATE_USER_INFO: createEndpoint("api/users/update"),
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
  }
};
