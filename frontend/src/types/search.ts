export interface PaginationParams {
  pageNumber: number
  pageSize: number
}

export interface PagedResult<T> {
  items: T[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

export interface VacancySearchDto {
  id: string
  employerId: string
  title: string
  description: string | null
  salary: number | null
  type: 'Internship' | 'Job'
  updatedAt: string
  employerName: string
  specializationName: string | null
  cityName: string | null
  actualAddress: string | null
  contactPhone: string | null
  contactEmail: string | null
  avatarUrl?: string | null
  isDeleted: boolean
  courses: string[]
  skills: string[]
  isFavorite: boolean
  isBlocked: boolean
}

export interface VacancySearchFilter {
  employerId?: string | null
  searchTerm?: string | null
  cityId?: string | null
  specializationIds?: string[]
  vacancyTypes?: string[]
  courseIds?: string[]
  skillIds?: string[]
  minSalary?: number | null
  onlyWithSalary?: boolean
  paging: PaginationParams
}

export interface ResumeSearchDto {
  id: string
  studentId: string
  title: string
  description: string | null
  updatedAt: string
  email: string | null
  phone: string | null
  fullName: string
  cityName: string | null
  universityName: string | null
  facultyName: string | null
  departmentName: string | null
  studyDirectionName: string | null
  courseNumber: number | null
  skills: string[]
  completedCourses: string[]
  isFavorite: boolean
  isBlocked: boolean
  avatarUrl: string | null
  birthDate: string | null
  gender: 'Male' | 'Female' | null
  isForeign: boolean
  status: 'Studying' | 'SeekingInternship' | 'SeekingJob' | 'Interning' | 'Working'
  studyForm: 'FullTime' | 'PartTime' | 'Correspondence' | null
}

export interface ResumeSearchFilter {
  searchTerm?: string | null
  skillIds?: string[]
  cityId?: string | null
  universityId?: string | null
  facultyId?: string | null
  departmentId?: string | null
  studyDirectionId?: string | null
  courseNumber?: number | null
  courseIds?: string[]
  paging: PaginationParams
}
