export interface TagItem {
  id: string
  name: string
}

export interface ResumeDocumentDto {
  id?: string
  studentId?: string
  title: string
  fullName: string
  birthDate?: string | null
  gender?: string | null
  cityName?: string | null
  citizenship?: string | null
  updatedAt?: string | null
  avatarUrl?: string | null
  status?: string | null
  email?: string | null
  phone?: string | null
  universityName?: string | null
  facultyName?: string | null
  departmentName?: string | null
  studyDirectionName?: string | null
  courseNumber?: number | null
  studyForm?: string | null
  completedCourses?: string[]

  skillsList?: TagItem[] // Для редактирования Student
  skillNames?: string[] // Для просмотра Employer

  description?: string | null
  isDeleted?: boolean
  isFavorite?: boolean
  isBlocked?: boolean
}
