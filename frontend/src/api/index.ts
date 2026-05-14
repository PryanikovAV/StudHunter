import axios from 'axios'
import { useToast } from 'vue-toastification'

const toast = useToast()

const apiClient = axios.create({
  baseURL: '/api/v1',
  headers: {
    'Content-Type': 'application/json',
  },
})

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

apiClient.interceptors.response.use(
  (response) => {
    return response
  },
  (error) => {
    if (!error.response) {
      toast.error('Ошибка сети. Проверьте подключение или доступность сервера.')
      return Promise.reject(error)
    }

    const status = error.response.status
    const data = error.response.data

    let errorMessage = 'Произошла непредвиденная ошибка'

    if (data) {
      if (data.errors && typeof data.errors === 'object') {
        const errorMessages: string[] = []
        for (const key in data.errors) {
          if (Array.isArray(data.errors[key])) {
            errorMessages.push(...data.errors[key])
          }
        }
        if (errorMessages.length > 0) {
          errorMessage = errorMessages.join('\n')
        } else if (data.title) {
          errorMessage = data.title
        }
      } else if (data.detail) {
        errorMessage = data.detail
      } else if (data.title) {
        errorMessage = data.title
      } else if (typeof data === 'string') {
        errorMessage = data
      }
    }

    switch (status) {
      case 400:
        toast.error(errorMessage)
        break
      case 401:
        localStorage.removeItem('token')
        localStorage.removeItem('userRole')
        toast.error('Ошибка авторизации. Пожалуйста, войдите в систему.')
        break
      case 403:
        toast.warning(errorMessage || 'Доступ запрещен.')
        break
      case 404:
        toast.info(errorMessage || 'Запрашиваемый ресурс не найден.')
        break
      case 500:
        toast.error(errorMessage || 'Внутренняя ошибка сервера.')
        break
      default:
        toast.error(errorMessage)
    }

    return Promise.reject(error)
  },
)

export default apiClient
