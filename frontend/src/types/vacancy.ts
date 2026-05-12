export interface VacancyListDto {
  id: string
  title: string
  salary: number | null
  type: string
  totalResponses?: number | null
  activeResponses?: number | null
  updatedAt: string
  isDeleted: boolean
}

export interface VacancyFillDto {
  id?: string | null
  title: string
  description: string | null
  salary: number | null
  type: string
  skillIds: string[]
  courseIds: string[]
  skills: { id: string; name: string }[]
  courses: { id: string; name: string }[]
}

export interface VacancyDetailsDto {
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
  isBlocked?: boolean
}
