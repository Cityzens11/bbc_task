import { useMemo, useState } from 'react'
import { useMutation, useQuery } from '@tanstack/react-query'
import {
  convertCurrency,
  getApiErrorMessage,
  getHistoricalRates,
  getLatestRates,
} from './api/currencyApi'

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
        <article className="card">
          <h2>Conversion</h2>
          <div className="form-grid conversion-grid">
            <label>
              Amount
              <input
                type="number"
                min={0}
                value={amount}
                onChange={(e) => setAmount(Number(e.target.value))}
              />
            </label>
            <label>
              Source
              <input
                value={fromCurrency}
                onChange={(e) => setFromCurrency(e.target.value.toUpperCase())}
                maxLength={3}
              />
            </label>
            <label>
              Target
              <input
                value={toCurrency}
                onChange={(e) => setToCurrency(e.target.value.toUpperCase())}
                maxLength={3}
              />
            </label>
          </div>

          {isExcludedInput && (
            <p className="error">
              TRY, PLN, THB, and MXN are excluded by business policy and cannot be converted.
            </p>
          )}

          <button
            onClick={() => convertMutation.mutate()}
            disabled={convertMutation.isPending || isExcludedInput || amount <= 0}
          >
            {convertMutation.isPending ? 'Converting...' : 'Convert'}
          </button>

          {convertMutation.isError && (
            <p className="error">{getApiErrorMessage(convertMutation.error)}</p>
          )}

          {convertMutation.isSuccess && (
            <p className="success">
              {convertMutation.data.amount} {convertMutation.data.fromCurrency} ={' '}
              {convertMutation.data.convertedAmount} {convertMutation.data.toCurrency}
            </p>
          )}
        </article>

        <article className="card">
          <h2>Latest Rates</h2>
          <label>
            Base Currency
            <input
              value={latestBase}
              onChange={(e) => setLatestBase(e.target.value.toUpperCase())}
              maxLength={3}
            />
          </label>

          {latestQuery.isLoading && <p>Loading latest rates...</p>}
          {latestQuery.isError && (
            <p className="error">{getApiErrorMessage(latestQuery.error)}</p>
          )}

          {latestQuery.isSuccess && (
            <div className="table-wrap">
              <table>
                <thead>
                  <tr>
                    <th>Currency</th>
                    <th>Rate</th>
                  </tr>
                </thead>
                <tbody>
                  {latestRows.map(([currency, rate]) => (
                    <tr key={currency}>
                      <td>{currency}</td>
                      <td>{rate}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </article>
      </section>

      <section className="card history">
        <h2>Historical Rates</h2>
        <div className="form-grid">
          <label>
            Base
            <input
              value={historyBase}
              onChange={(e) => setHistoryBase(e.target.value.toUpperCase())}
              maxLength={3}
            />
          </label>
          <label>
            Start
            <input type="date" value={startDate} onChange={(e) => setStartDate(e.target.value)} />
          </label>
          <label>
            End
            <input type="date" value={endDate} onChange={(e) => setEndDate(e.target.value)} />
          </label>
          <label>
            Page Size
            <select
              value={pageSize}
              onChange={(e) => {
                setPageSize(Number(e.target.value))
                setPage(1)
              }}
            >
              {[5, 10, 20, 50].map((size) => (
                <option key={size} value={size}>
                  {size}
                </option>
              ))}
            </select>
          </label>
        </div>

        {historicalQuery.isLoading && <p>Loading historical rates...</p>}
        {historicalQuery.isError && (
          <p className="error">{getApiErrorMessage(historicalQuery.error)}</p>
        )}

        {historicalQuery.isSuccess && (
          <>
            <div className="table-wrap">
              <table>
                <thead>
                  <tr>
                    <th>Date</th>
                    <th>Sample Rates</th>
                  </tr>
                </thead>
                <tbody>
                  {historicalQuery.data.items.map((item) => (
                    <tr key={item.date}>
                      <td>{item.date}</td>
                      <td>
                        {Object.entries(item.rates)
                          .slice(0, 4)
                          .map(([currency, rate]) => `${currency}: ${rate}`)
                          .join(' | ')}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            <div className="pager">
              <button
                disabled={page <= 1}
                onClick={() => setPage((current) => current - 1)}
              >
                Previous
              </button>
              <span>
                Page {historicalQuery.data.page} of {Math.max(1, historicalQuery.data.totalPages)}
              </span>
              <button
                disabled={page >= historicalQuery.data.totalPages}
                onClick={() => setPage((current) => current + 1)}
              >
                Next
              </button>
            </div>
          </>
        )}
      </section>
    </main>
  )
}

export default App
