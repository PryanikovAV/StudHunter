import type { VacancySearchDto } from './search'

export interface CategoryCardDto {
  title: string
  count: number
  filterKey: string
  filterValue: string
}

export interface HomePageDto {
  popularCategories: CategoryCardDto[]
  latestVacancies: VacancySearchDto[]
}

export interface GeneralStatisticsDto {
  totalResumes: number
  totalVacancies: number
  accreditedEmployers: number
}
