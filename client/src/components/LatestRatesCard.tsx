import { getApiErrorMessage } from '../api/currencyApi'

interface LatestRatesCardProps {
  baseCurrency: string
  rows: [string, number][]
  isLoading: boolean
  isError: boolean
  isSuccess: boolean
  error: unknown
  onBaseCurrencyChange: (currency: string) => void
}

export function LatestRatesCard({
  baseCurrency,
  rows,
  isLoading,
  isError,
  isSuccess,
  error,
  onBaseCurrencyChange,
}: LatestRatesCardProps) {
  return (
    <article className="card">
      <h2>Latest Rates</h2>
      <label>
        Base Currency
        <input
          value={baseCurrency}
          onChange={(e) => onBaseCurrencyChange(e.target.value.toUpperCase())}
          maxLength={3}
        />
      </label>

      {isLoading && <p>Loading latest rates...</p>}
      {isError && <p className="error">{getApiErrorMessage(error)}</p>}

      {isSuccess && (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Currency</th>
                <th>Rate</th>
              </tr>
            </thead>
            <tbody>
              {rows.map(([currency, rate]) => (
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
  )
}