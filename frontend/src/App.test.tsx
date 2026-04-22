import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { render, screen, waitFor } from '@testing-library/react'
import { describe, expect, it, vi } from 'vitest'
import App from './App'
import * as api from './api/currencyApi'

vi.mock('./api/currencyApi')

describe('App', () => {
  it('renders latest rates and conversion panel', async () => {
    vi.mocked(api.getLatestRates).mockResolvedValue({
      date: '2020-01-01',
      baseCurrency: 'EUR',
      rates: { USD: 1.1, GBP: 0.9 },
    })

    vi.mocked(api.getHistoricalRates).mockResolvedValue({
      baseCurrency: 'EUR',
      startDate: '2020-01-01',
      endDate: '2020-01-10',
      page: 1,
      pageSize: 10,
      totalItems: 1,
      totalPages: 1,
      items: [{ date: '2020-01-01', rates: { USD: 1.1 } }],
    })

    vi.mocked(api.convertCurrency).mockResolvedValue({
      amount: 100,
      fromCurrency: 'EUR',
      toCurrency: 'USD',
      rate: 1.1,
      convertedAmount: 110,
      rateDate: '2020-01-01',
    })

    const queryClient = new QueryClient()

    render(
      <QueryClientProvider client={queryClient}>
        <App />
      </QueryClientProvider>,
    )

    expect(screen.getByText('Currency Converter Platform')).toBeInTheDocument()

    await waitFor(() => {
      expect(screen.getByText('USD')).toBeInTheDocument()
      expect(screen.getByText('2020-01-01')).toBeInTheDocument()
    })
  })
})
