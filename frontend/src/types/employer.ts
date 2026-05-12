export interface EmployerDto {
  id: string
  email: string
  name: string
  cityId: string | null
  cityName: string | null
  contactPhone: string | null
  contactEmail: string | null
  avatarUrl: string | null
  description: string | null
  website: string | null
  specializationId: string | null
  inn: string | null
  ogrn: string | null
  kpp: string | null
  legalAddress: string | null
  actualAddress: string | null
}

export interface EmployerHeroDto {
  id: string
  name: string
  avatarUrl: string | null
  cityName: string | null
  specializationName: string | null
  website: string | null
  contactEmail: string | null
  contactPhone: string | null
  registrationStage: string
  inn: string | null
  ogrn: string | null
  kpp: string | null
  legalAddress: string | null
  actualAddress: string | null
  activeVacanciesCount: number
  isFavorite?: boolean
  isBlocked?: boolean
}

export interface VacancySearchDto {
  id: string
  employerId: string
  title: string
  description: string | null
  salary: number | null
  type: string
  updatedAt: string
  employerName: string
  specializationName: string | null
  cityName: string | null
  actualAddress: string | null
  contactPhone: string | null
  contactEmail: string | null
  isDeleted: boolean
  courses: string[]
  skills: string[]
  isFavorite?: boolean
}
