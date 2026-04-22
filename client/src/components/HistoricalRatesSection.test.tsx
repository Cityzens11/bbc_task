import { fireEvent, render, screen } from '@testing-library/react'
import { describe, expect, it, vi } from 'vitest'
import { HistoricalRatesSection } from './HistoricalRatesSection'

vi.mock('../api/currencyApi', () => ({
  getApiErrorMessage: vi.fn(() => 'Historical error'),
}))

describe('HistoricalRatesSection', () => {
  it('renders loading and error states', () => {
    render(
      <HistoricalRatesSection
        baseCurrency="EUR"
        startDate="2020-01-01"
        endDate="2020-01-10"
        page={1}
        pageSize={10}
        data={undefined}
        isLoading
        isError
        isSuccess={false}
        error={new Error('fail')}
        onBaseCurrencyChange={vi.fn()}
        onStartDateChange={vi.fn()}
        onEndDateChange={vi.fn()}
        onPageSizeChange={vi.fn()}
        onPreviousPage={vi.fn()}
        onNextPage={vi.fn()}
      />,
    )

    expect(screen.getByText('Loading historical rates...')).toBeInTheDocument()
    expect(screen.getByText('Historical error')).toBeInTheDocument()
  })

  it('renders data and calls filter and pager callbacks', () => {
    const onBaseCurrencyChange = vi.fn()
    const onStartDateChange = vi.fn()
    const onEndDateChange = vi.fn()
    const onPageSizeChange = vi.fn()
    const onPreviousPage = vi.fn()
    const onNextPage = vi.fn()

    render(
      <HistoricalRatesSection
        baseCurrency="EUR"
        startDate="2020-01-01"
        endDate="2020-01-10"
        page={2}
        pageSize={10}
        data={{
          baseCurrency: 'EUR',
          startDate: '2020-01-01',
          endDate: '2020-01-10',
          page: 2,
          pageSize: 10,
          totalItems: 2,
          totalPages: 3,
          items: [
            { date: '2020-01-01', rates: { USD: 1.1, GBP: 0.9 } },
            { date: '2020-01-02', rates: { USD: 1.2 } },
          ],
        }}
        isLoading={false}
        isError={false}
        isSuccess
        error={null}
        onBaseCurrencyChange={onBaseCurrencyChange}
        onStartDateChange={onStartDateChange}
        onEndDateChange={onEndDateChange}
        onPageSizeChange={onPageSizeChange}
        onPreviousPage={onPreviousPage}
        onNextPage={onNextPage}
      />,
    )

    expect(screen.getByText('2020-01-01')).toBeInTheDocument()
    expect(screen.getByText(/USD: 1.1/)).toBeInTheDocument()
    expect(screen.getByText('Page 2 of 3')).toBeInTheDocument()

    fireEvent.change(screen.getByLabelText('Base'), { target: { value: 'chf' } })
    fireEvent.change(screen.getByLabelText('Start'), { target: { value: '2020-01-03' } })
    fireEvent.change(screen.getByLabelText('End'), { target: { value: '2020-01-11' } })
    fireEvent.change(screen.getByLabelText('Page Size'), { target: { value: '20' } })
    fireEvent.click(screen.getByRole('button', { name: 'Previous' }))
    fireEvent.click(screen.getByRole('button', { name: 'Next' }))

    expect(onBaseCurrencyChange).toHaveBeenCalledWith('CHF')
    expect(onStartDateChange).toHaveBeenCalledWith('2020-01-03')
    expect(onEndDateChange).toHaveBeenCalledWith('2020-01-11')
    expect(onPageSizeChange).toHaveBeenCalledWith(20)
    expect(onPreviousPage).toHaveBeenCalledOnce()
    expect(onNextPage).toHaveBeenCalledOnce()
  })
})