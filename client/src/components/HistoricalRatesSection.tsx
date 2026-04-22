import { getApiErrorMessage } from '../api/currencyApi'
import type { HistoricalRatesResponse } from '../types'

interface HistoricalRatesSectionProps {
  baseCurrency: string
  startDate: string
  endDate: string
  page: number
  pageSize: number
  data: HistoricalRatesResponse | undefined
  isLoading: boolean
  isError: boolean
  isSuccess: boolean
  error: unknown
  onBaseCurrencyChange: (currency: string) => void
  onStartDateChange: (date: string) => void
  onEndDateChange: (date: string) => void
  onPageSizeChange: (pageSize: number) => void
  onPreviousPage: () => void
  onNextPage: () => void
}

export function HistoricalRatesSection({
  baseCurrency,
  startDate,
  endDate,
  page,
  pageSize,
  data,
  isLoading,
  isError,
  isSuccess,
  error,
  onBaseCurrencyChange,
  onStartDateChange,
  onEndDateChange,
  onPageSizeChange,
  onPreviousPage,
  onNextPage,
}: HistoricalRatesSectionProps) {
  return (
    <section className="card history">
      <h2>Historical Rates</h2>
      <div className="form-grid">
        <label>
          Base
          <input
            value={baseCurrency}
            onChange={(e) => onBaseCurrencyChange(e.target.value.toUpperCase())}
            maxLength={3}
          />
        </label>
        <label>
          Start
          <input type="date" value={startDate} onChange={(e) => onStartDateChange(e.target.value)} />
        </label>
        <label>
          End
          <input type="date" value={endDate} onChange={(e) => onEndDateChange(e.target.value)} />
        </label>
        <label>
          Page Size
          <select value={pageSize} onChange={(e) => onPageSizeChange(Number(e.target.value))}>
            {[5, 10, 20, 50].map((size) => (
              <option key={size} value={size}>
                {size}
              </option>
            ))}
          </select>
        </label>
      </div>

      {isLoading && <p>Loading historical rates...</p>}
      {isError && <p className="error">{getApiErrorMessage(error)}</p>}

      {isSuccess && data && (
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
                {data.items.map((item) => (
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
            <button disabled={page <= 1} onClick={onPreviousPage}>
              Previous
            </button>
            <span>
              Page {data.page} of {Math.max(1, data.totalPages)}
            </span>
            <button disabled={page >= data.totalPages} onClick={onNextPage}>
              Next
            </button>
          </div>
        </>
      )}
    </section>
  )
}