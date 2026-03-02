export interface VacancyListDto {
  id: string
  title: string
  salary: number | null
  type: string
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
