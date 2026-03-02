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
