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
  SUPPLIERS: {
    GET_LIST: createEndpoint("/api/suppliers"),
    GET_BY_ID: createEndpoint("/api/suppliers/:id"),
    CREATE: createEndpoint("/api/suppliers/create"),
    UPDATE: createEndpoint("/api/suppliers/update"),
    DELETE: createEndpoint("/api/suppliers/delete"),
  },
};
