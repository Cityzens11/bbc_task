import { fireEvent, render, screen } from '@testing-library/react'
import { describe, expect, it, vi } from 'vitest'
import { ConversionCard } from './ConversionCard'

vi.mock('../api/currencyApi', () => ({
  getApiErrorMessage: vi.fn(() => 'API error'),
}))

describe('ConversionCard', () => {
  it('calls callbacks for input changes and convert click', () => {
    const onAmountChange = vi.fn()
    const onFromCurrencyChange = vi.fn()
    const onToCurrencyChange = vi.fn()
    const onConvert = vi.fn()

    render(
      <ConversionCard
        amount={100}
        fromCurrency="EUR"
        toCurrency="USD"
        isExcludedInput={false}
        isConverting={false}
        isError={false}
        isSuccess={false}
        error={null}
        result={undefined}
        onAmountChange={onAmountChange}
        onFromCurrencyChange={onFromCurrencyChange}
        onToCurrencyChange={onToCurrencyChange}
        onConvert={onConvert}
      />,
    )

    fireEvent.change(screen.getByLabelText('Amount'), { target: { value: '250' } })
    fireEvent.change(screen.getByLabelText('Source'), { target: { value: 'gbp' } })
    fireEvent.change(screen.getByLabelText('Target'), { target: { value: 'jpy' } })
    fireEvent.click(screen.getByRole('button', { name: 'Convert' }))

    expect(onAmountChange).toHaveBeenCalledWith(250)
    expect(onFromCurrencyChange).toHaveBeenCalledWith('GBP')
    expect(onToCurrencyChange).toHaveBeenCalledWith('JPY')
    expect(onConvert).toHaveBeenCalledOnce()
  })

  it('renders error and excludes conversion when flagged', () => {
    render(
      <ConversionCard
        amount={100}
        fromCurrency="TRY"
        toCurrency="USD"
        isExcludedInput
        isConverting={false}
        isError
        isSuccess={false}
        error={new Error('fail')}
        result={undefined}
        onAmountChange={vi.fn()}
        onFromCurrencyChange={vi.fn()}
        onToCurrencyChange={vi.fn()}
        onConvert={vi.fn()}
      />,
    )

    expect(screen.getByText(/cannot be converted/i)).toBeInTheDocument()
    expect(screen.getByText('API error')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: 'Convert' })).toBeDisabled()
  })
})