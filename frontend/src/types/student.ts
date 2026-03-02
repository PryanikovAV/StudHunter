export interface DictionaryItem {
  id: string
  name: string
}

export interface StudentProfileDto {
  id: string
  email: string
  firstName: string
  lastName: string
  patronymic: string | null
  contactPhone: string | null
  contactEmail: string | null
  cityId: string | null
  gender: 'Male' | 'Female' | null
  birthDate: string | null
  avatarUrl: string | null
  isForeign: boolean
  status: 'Studying' | 'SeekingJob' | 'SeekingInternship' | 'Interning' | 'Working'
  universityId: string | null
  facultyId: string | null
  departmentId: string | null
  studyDirectionId: string | null
  courseNumber: number
  studyForm: 'FullTime' | 'PartTime' | 'Correspondence'
  courseIds: string[]
  courses: { id: string; name: string }[]
}

export interface ResumeFillDto {
  id?: string | null
  title: string
  description: string | null
  isDeleted: boolean
  skillIds: string[]
  skills: { id: string; name: string }[]
}
