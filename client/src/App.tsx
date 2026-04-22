import { useMemo, useState } from 'react'
import { useMutation, useQuery } from '@tanstack/react-query'
import {
  convertCurrency,
  getHistoricalRates,
  getLatestRates,
} from './api/currencyApi'
import { ConversionCard } from './components/ConversionCard'
import { HistoricalRatesSection } from './components/HistoricalRatesSection'
import { LatestRatesCard } from './components/LatestRatesCard'

const EXCLUDED = new Set(['TRY', 'PLN', 'THB', 'MXN'])

function App() {
  const [latestBase, setLatestBase] = useState('EUR')
  const [amount, setAmount] = useState(100)
  const [fromCurrency, setFromCurrency] = useState('EUR')
  const [toCurrency, setToCurrency] = useState('USD')
  const [historyBase, setHistoryBase] = useState('EUR')
  const [startDate, setStartDate] = useState('2020-01-01')
  const [endDate, setEndDate] = useState('2020-01-10')
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(10)

  const latestQuery = useQuery({
    queryKey: ['latest', latestBase],
    queryFn: () => getLatestRates(latestBase),
    staleTime: 60_000,
  })

  const convertMutation = useMutation({
    mutationFn: () => convertCurrency(amount, fromCurrency, toCurrency),
  })

  const historicalQuery = useQuery({
    queryKey: ['historical', historyBase, startDate, endDate, page, pageSize],
    queryFn: () =>
      getHistoricalRates(historyBase, startDate, endDate, page, pageSize),
    staleTime: 60_000,
  })

  const latestRows = useMemo(() => {
    const entries = Object.entries(latestQuery.data?.rates ?? {})
    return entries.sort(([a], [b]) => a.localeCompare(b)).slice(0, 25)
  }, [latestQuery.data])

  const isExcludedInput = EXCLUDED.has(fromCurrency) || EXCLUDED.has(toCurrency)

  return (
    <main className="shell">
      <header className="hero">
        <p className="tag">Currency Converter Platform</p>
      </header>

      <section className="grid">
        <ConversionCard
          amount={amount}
          fromCurrency={fromCurrency}
          toCurrency={toCurrency}
          isExcludedInput={isExcludedInput}
          isConverting={convertMutation.isPending}
          isError={convertMutation.isError}
          isSuccess={convertMutation.isSuccess}
          error={convertMutation.error}
          result={convertMutation.data}
          onAmountChange={setAmount}
          onFromCurrencyChange={setFromCurrency}
          onToCurrencyChange={setToCurrency}
          onConvert={() => convertMutation.mutate()}
        />

        <LatestRatesCard
          baseCurrency={latestBase}
          rows={latestRows}
          isLoading={latestQuery.isLoading}
          isError={latestQuery.isError}
          isSuccess={latestQuery.isSuccess}
          error={latestQuery.error}
          onBaseCurrencyChange={setLatestBase}
        />
      </section>

      <HistoricalRatesSection
        baseCurrency={historyBase}
        startDate={startDate}
        endDate={endDate}
        page={page}
        pageSize={pageSize}
        data={historicalQuery.data}
        isLoading={historicalQuery.isLoading}
        isError={historicalQuery.isError}
        isSuccess={historicalQuery.isSuccess}
        error={historicalQuery.error}
        onBaseCurrencyChange={setHistoryBase}
        onStartDateChange={setStartDate}
        onEndDateChange={setEndDate}
        onPageSizeChange={(size) => {
          setPageSize(size)
          setPage(1)
        }}
        onPreviousPage={() => setPage((current) => current - 1)}
        onNextPage={() => setPage((current) => current + 1)}
      />
    </main>
  )
}

export default App
