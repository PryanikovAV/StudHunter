export interface InvitationCandidateDto {
  studentId: string
  fullName: string
  courseNumber: number | null
  universityAbbreviation: string | null
  skills: string[]
  resumeId: string | null
}

export interface InvitationJobDto {
  employerId: string
  companyName: string
  vacancyTitle: string | null
  salary: number | null
  vacancyId: string | null
}

export interface InvitationCardDto {
  id: string
  status: string
  type: string
  direction: string
  sentAt: string
  message: string | null
  candidate: InvitationCandidateDto
  job: InvitationJobDto
}
