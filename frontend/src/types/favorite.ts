export interface FavoriteCardDto {
  id: string
  targetId: string
  type: 'Vacancy' | 'Student' | 'Employer'
  title: string
  subtitle: string | null
  avatarUrl: string | null
  addedAt: string
}

export interface BlackListDto {
  id: string
  blockedUserId: string
  displayName: string
  avatarUrl: string | null
  role: string
  blockedAt: string
}
