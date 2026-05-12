import apiClient from '@/api'

export const downloadResumePdf = async (resumeId: string, studentFullName: string) => {
  try {
    const response = await apiClient.get(`/search/resumes/${resumeId}/pdf`, {
      responseType: 'blob',
    })

    const url = window.URL.createObjectURL(new Blob([response.data]))
    const link = document.createElement('a')
    link.href = url

    const safeName = studentFullName.replace(/\s+/g, '_')
    link.setAttribute('download', `Резюме_${safeName}.pdf`)

    document.body.appendChild(link)
    link.click()

    link.parentNode?.removeChild(link)
    window.URL.revokeObjectURL(url)
  } catch (error) {
    console.error('Ошибка при скачивании PDF:', error)
    window.alert('Не удалось скачать резюме. Попробуйте позже.')
  }
}
