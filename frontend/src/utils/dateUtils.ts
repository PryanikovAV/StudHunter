export function calculateAge(birthDate: string | Date | null | undefined): string {
  if (!birthDate) return ''

  const birth = new Date(birthDate)
  const today = new Date()

  let age = today.getFullYear() - birth.getFullYear()
  const m = today.getMonth() - birth.getMonth()

  if (m < 0 || (m === 0 && today.getDate() < birth.getDate())) {
    age--
  }

  const lastDigit = age % 10
  const lastTwo = age % 100

  if (lastTwo >= 11 && lastTwo <= 14) {
    return `${age} лет`
  }
  if (lastDigit === 1) {
    return `${age} год`
  }
  if (lastDigit >= 2 && lastDigit <= 4) {
    return `${age} года`
  }

  return `${age} лет`
}
