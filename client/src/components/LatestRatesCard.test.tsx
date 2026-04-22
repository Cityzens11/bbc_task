import { fireEvent, render, screen } from '@testing-library/react'
import { describe, expect, it, vi } from 'vitest'
import { LatestRatesCard } from './LatestRatesCard'

vi.mock('../api/currencyApi', () => ({
  getApiErrorMessage: vi.fn(() => 'Latest error'),
}))

describe('LatestRatesCard', () => {
  it('renders loading and error states', () => {
    render(
      <LatestRatesCard
        baseCurrency="EUR"
        rows={[]}
        isLoading
        isError
        isSuccess={false}
        error={new Error('fail')}
        onBaseCurrencyChange={vi.fn()}
      />,
    )

    expect(screen.getByText('Loading latest rates...')).toBeInTheDocument()
    expect(screen.getByText('Latest error')).toBeInTheDocument()
  })

  it('renders rows and uppercases base currency changes', () => {
    const onBaseCurrencyChange = vi.fn()

    render(
      <LatestRatesCard
        baseCurrency="EUR"
        rows={[
          ['USD', 1.1],
          ['GBP', 0.9],
        ]}
        isLoading={false}
        isError={false}
        isSuccess
        error={null}
        onBaseCurrencyChange={onBaseCurrencyChange}
      />,
    )

    expect(screen.getByText('USD')).toBeInTheDocument()
    expect(screen.getByText('GBP')).toBeInTheDocument()

    fireEvent.change(screen.getByLabelText('Base Currency'), { target: { value: 'chf' } })
    expect(onBaseCurrencyChange).toHaveBeenCalledWith('CHF')
  })
})