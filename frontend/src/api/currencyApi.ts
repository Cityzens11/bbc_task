import axios, { AxiosError } from "axios";
import { API_BASE_URL } from "../config";
import type {
  ConversionResponse,
  HistoricalRatesResponse,
  LatestRatesResponse,
} from "../types";

const api = axios.create({
  baseURL: `${API_BASE_URL}/api/v1`,
  timeout: 15000,
});

let accessToken: string | null = null;

async function ensureToken(): Promise<string> {
  if (accessToken) {
    return accessToken;
  }

  const tokenResponse = await axios.post<{ accessToken: string }>(
    `${API_BASE_URL}/api/v1/auth/token`,
    {
      clientId: "frontend-client",
      roles: ["RatesReader", "Converter"],
    },
  );

  accessToken = tokenResponse.data.accessToken;
  return accessToken;
}

async function authorizedGet<T>(url: string, params?: Record<string, unknown>) {
  const token = await ensureToken();
  return api.get<T>(url, {
    params,
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
}

export async function getLatestRates(baseCurrency: string) {
  const response = await authorizedGet<LatestRatesResponse>("/rates/latest", {
    baseCurrency,
  });

  return response.data;
}

export async function convertCurrency(
  amount: number,
  fromCurrency: string,
  toCurrency: string,
) {
  const response = await authorizedGet<ConversionResponse>("/rates/convert", {
    amount,
    fromCurrency,
    toCurrency,
  });

  return response.data;
}

export async function getHistoricalRates(
  baseCurrency: string,
  startDate: string,
  endDate: string,
  page: number,
  pageSize: number,
) {
  const response = await authorizedGet<HistoricalRatesResponse>(
    "/rates/historical",
    {
      baseCurrency,
      startDate,
      endDate,
      page,
      pageSize,
    },
  );

  return response.data;
}

export function getApiErrorMessage(error: unknown): string {
  if (error instanceof AxiosError) {
    const detail = error.response?.data?.detail;
    if (typeof detail === "string") {
      return detail;
    }

    return error.message;
  }

  return "Something went wrong while contacting the API.";
}
