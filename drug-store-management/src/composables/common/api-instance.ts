import axios, { AxiosError, AxiosRequestConfig, AxiosResponse } from "axios";
import { useRouter } from "vue-router";
import { useLoadingStore } from "./loadingStore";
import { useAuthenticationStore } from "../auth/authenticationStore";

interface ApiResponse<T = any> {
  result?: number;
  data?: T;
  errorMessage?: string;
  errorCode?: string;
}

export function useApi() {
  const router = useRouter();
  const authStore = useAuthenticationStore();
  const loadingStore = useLoadingStore();
//process.env.VITE_API_URL
  const api = axios.create({
    baseURL: "http://localhost:5206",
    withCredentials: true,
    timeout: 5000,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${authStore.getAccessToken}`,
    },
  });

  api.interceptors.response.use(
    (response: any) => response,
    async (error: AxiosError) => {
      if (error.response && error.response.status === 401) {
        window.location.href = "/login";
      } else {
        handleApiError(error);
      }
      return Promise.reject(error);
    },
  );

  const handleApiError = (error: AxiosError) => {
    console.error("API Error:", error);
    router.push("/error").catch(() => {
      window.location.href = "/error";
    });
  };

  const handleResponse = async (response: AxiosResponse<ApiResponse>) => {
    if (response.data && response.data.result === 2) {
      router.push("/error").catch(() => {
        window.location.href = "/error";
      });
    }

    return response;
  };

  const request = async <T>(
    config: AxiosRequestConfig,
  ): Promise<AxiosResponse<ApiResponse<T>>> => {
    loadingStore.show();

    try {
      const response = await api.request<ApiResponse<T>>(config);
      return await handleResponse(response);
    }
    catch (err) {
      throw err;
    } 
    finally {
      loadingStore.hide();
    }
  };

  const get = <T>(url: string, config?: AxiosRequestConfig) =>
    request<T>({ ...config, method: "GET", url });

  const post = <T>(url: string, data?: any, config?: AxiosRequestConfig) =>
    request<T>({ ...config, method: "POST", url, data });

  // Add put,delete,... method

  return {
    get,
    post,
  };
}
