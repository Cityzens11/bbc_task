import { getApiErrorMessage } from '../api/currencyApi'
import type { ConversionResponse } from '../types'

interface ConversionCardProps {
  amount: number
  fromCurrency: string
  toCurrency: string
  isExcludedInput: boolean
  isConverting: boolean
  isError: boolean
  isSuccess: boolean
  error: unknown
  result: ConversionResponse | undefined
  onAmountChange: (amount: number) => void
  onFromCurrencyChange: (currency: string) => void
  onToCurrencyChange: (currency: string) => void
  onConvert: () => void
}

export function ConversionCard({
  amount,
  fromCurrency,
  toCurrency,
  isExcludedInput,
  isConverting,
  isError,
  isSuccess,
  error,
  result,
  onAmountChange,
  onFromCurrencyChange,
  onToCurrencyChange,
  onConvert,
}: ConversionCardProps) {
  return (
    <article className="card">
      <h2>Conversion</h2>
      <div className="form-grid conversion-grid">
        <label>
          Amount
          <input
            type="number"
            min={0}
            value={amount}
            onChange={(e) => onAmountChange(Number(e.target.value))}
          />
        </label>
        <label>
          Source
          <input
            value={fromCurrency}
            onChange={(e) => onFromCurrencyChange(e.target.value.toUpperCase())}
            maxLength={3}
          />
        </label>
        <label>
          Target
          <input
            value={toCurrency}
            onChange={(e) => onToCurrencyChange(e.target.value.toUpperCase())}
            maxLength={3}
          />
        </label>
      </div>

      {isExcludedInput && (
        <p className="error">
          TRY, PLN, THB, and MXN are excluded by business policy and cannot be converted.
        </p>
      )}

      <button onClick={onConvert} disabled={isConverting || isExcludedInput || amount <= 0}>
        {isConverting ? 'Converting...' : 'Convert'}
      </button>

      {isError && <p className="error">{getApiErrorMessage(error)}</p>}

      {isSuccess && result && (
        <p className="success">
          {result.amount} {result.fromCurrency} = {result.convertedAmount} {result.toCurrency}
        </p>
      )}
    </article>
  )
}