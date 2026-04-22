export interface LatestRatesResponse {
  date: string;
  baseCurrency: string;
  rates: Record<string, number>;
}

export interface ConversionResponse {
  amount: number;
  fromCurrency: string;
  toCurrency: string;
  rate: number;
  convertedAmount: number;
  rateDate: string;
}

export interface HistoricalRateItem {
  date: string;
  rates: Record<string, number>;
}

export interface HistoricalRatesResponse {
  baseCurrency: string;
  startDate: string;
  endDate: string;
  items: HistoricalRateItem[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}
