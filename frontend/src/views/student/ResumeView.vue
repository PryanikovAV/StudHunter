<script setup lang="ts">
import { ref, onMounted } from 'vue'
import apiClient from '@/api'
import ResumeDocument from '@/components/ResumeDocument.vue'
import DownloadButton from '@/components/DownloadButton.vue'
import { downloadResumePdf } from '@/utils/fileUtils'
import type { StudentProfileDto, DictionaryItem } from '@/types/student'
import type { ResumeDocumentDto, TagItem } from '@/types/resume'

const profile = ref<StudentProfileDto | null>(null)
const resume = ref<ResumeDocumentDto>({
  title: '',
  fullName: '',
  description: '',
  isDeleted: false,
  skillsList: [],
})

const dictionaries = ref({
  cities: [] as DictionaryItem[],
  universities: [] as DictionaryItem[],
  faculties: [] as DictionaryItem[],
  departments: [] as DictionaryItem[],
  directions: [] as DictionaryItem[],
})

const isLoading = ref(true)
const isSaving = ref(false)
const isActionLoading = ref(false)
const isDownloading = ref(false)

const getDictName = (dict: DictionaryItem[], id: string | null | undefined): string | null => {
  if (!id) return null
  const item = dict.find((d) => d.id === id)
  return item ? item.name : null
}

const fetchSkills = async (query: string): Promise<TagItem[]> => {
  const response = await apiClient.get(`/dictionaries/skills/search?q=${query}&limit=10`)
  return response.data
}

const fetchData = async () => {
  isLoading.value = true
  try {
    const [profRes, resRes, cities, unis, facs, deps, dirs] = await Promise.all([
      apiClient.get<StudentProfileDto>('/students/me/profile'),
      apiClient.get('/my-resume'),
      apiClient.get('/dictionaries/cities'),
      apiClient.get('/dictionaries/universities'),
      apiClient.get('/dictionaries/faculties'),
      apiClient.get('/dictionaries/departments'),
      apiClient.get('/dictionaries/specialities'),
    ])

    profile.value = profRes.data
    dictionaries.value = {
      cities: cities.data,
      universities: unis.data,
      faculties: facs.data,
      departments: deps.data,
      directions: dirs.data,
    }

    const p = profile.value

    let citizenshipStatus = null
    if (p.isForeign === true) {
      citizenshipStatus = 'Иностранный студент'
    } else if (p.isForeign === false) {
      citizenshipStatus = 'Резидент'
    }

    resume.value = {
      id: resRes.data?.id,
      title: resRes.data?.title || '',
      description: resRes.data?.description || '',
      skillsList: resRes.data?.skills || [],
      isDeleted: resRes.data?.isDeleted || false,
      updatedAt: resRes.data?.updatedAt || null,
      fullName: `${p.lastName} ${p.firstName} ${p.patronymic || ''}`.trim(),
      birthDate: p.birthDate,
      gender: p.gender,
      cityName: getDictName(dictionaries.value.cities, p.cityId),

      citizenship: citizenshipStatus,

      email: p.contactEmail,
      phone: p.contactPhone,
      universityName: getDictName(dictionaries.value.universities, p.universityId),
      facultyName: getDictName(dictionaries.value.faculties, p.facultyId),
      departmentName: getDictName(dictionaries.value.departments, p.departmentId),
      studyDirectionName: getDictName(dictionaries.value.directions, p.studyDirectionId),
      courseNumber: p.courseNumber,
      studyForm: p.studyForm,
      completedCourses: p.courses?.map((c: { name: string }) => c.name) || [],

      avatarUrl: p.avatarUrl || null,
      status: p.status || null,
    }
  } catch (error) {
    console.error('Ошибка загрузки данных', error)
  } finally {
    isLoading.value = false
  }
}

const saveResume = async () => {
  if (!resume.value.title.trim()) {
    window.alert('Поле "Желаемая должность" обязательно для заполнения.')
    return
  }
  isSaving.value = true
  try {
    const payload = {
      title: resume.value.title,
      description: resume.value.description,
      isDeleted: resume.value.isDeleted,
      skillIds: (resume.value.skillsList || []).map((s) => s.id),
    }

    await apiClient.put('/my-resume', payload)

    resume.value.updatedAt = new Date().toISOString()
    window.alert('Резюме успешно сохранено!')
  } catch (error) {
    console.error('Ошибка сохранения', error)
    window.alert('Не удалось сохранить резюме.')
  } finally {
    isSaving.value = false
  }
}

const hideResume = async () => {
  if (
    !window.confirm(
      'Вы уверены, что хотите скрыть резюме? Оно больше не будет доступно для поиска работодателями.',
    )
  )
    return
  isActionLoading.value = true
  try {
    await apiClient.delete('/my-resume')
    resume.value.isDeleted = true
  } catch (error) {
    console.error('Ошибка скрытия', error)
    window.alert('Не удалось скрыть резюме.')
  } finally {
    isActionLoading.value = false
  }
}

const restoreResume = async () => {
  isActionLoading.value = true
  try {
    await apiClient.post('/my-resume/restore')
    resume.value.isDeleted = false
  } catch (error) {
    console.error('Ошибка восстановления', error)
    window.alert('Не удалось восстановить резюме.')
  } finally {
    isActionLoading.value = false
  }
}

const handleDownload = async () => {
  if (!resume.value.id || isDownloading.value) return
  isDownloading.value = true
  await downloadResumePdf(resume.value.id, resume.value.fullName)
  isDownloading.value = false
}

onMounted(fetchData)
</script>

<template>
  <div class="page-narrow">
    <h1 class="page-title">Моё резюме</h1>

    <div v-if="isLoading" class="loading">Загрузка данных...</div>

    <template v-else-if="profile">
      <div v-if="resume.isDeleted" class="deleted-banner">
        <p>Вы скрыли своё резюме. Работодатели не смогут найти вас.</p>
        <button class="btn-main btn-dark" @click="restoreResume" :disabled="isActionLoading">
          Восстановить резюме
        </button>
      </div>

      <fieldset :disabled="resume.isDeleted" class="clean-fieldset">
        <ResumeDocument
          mode="student"
          :data="resume"
          :fetch-skills-fn="fetchSkills"
          @update:title="resume.title = $event"
          @update:description="resume.description = $event"
          @update:skillsList="resume.skillsList = $event"
        >
          <template #actions-left>
            <DownloadButton
              v-if="resume.id && !resume.isDeleted"
              :is-loading="isDownloading"
              title="Скачать моё резюме"
              @click="handleDownload"
            />
          </template>

          <template #actions-right>
            <template v-if="!resume.isDeleted">
              <button
                v-if="resume.id"
                class="btn-main btn-outline"
                @click="hideResume"
                :disabled="isSaving || isActionLoading"
              >
                Скрыть резюме
              </button>

              <button
                class="btn-main btn-dark"
                @click="saveResume"
                :disabled="isSaving || isActionLoading"
              >
                {{ isSaving ? 'Сохранение...' : 'Сохранить изменения' }}
              </button>
            </template>
          </template>
        </ResumeDocument>
      </fieldset>
    </template>
  </div>
</template>

<style scoped>
.page-title {
  margin-bottom: 24px;
  color: var(--dark-text);
}
.loading {
  text-align: center;
  color: var(--gray-text);
  padding: 40px;
}

.deleted-banner {
  background-color: #f8fafc;
  border: 1px solid var(--gray-border);
  border-radius: 8px;
  padding: 16px 20px;
  margin-bottom: 24px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
}
.deleted-banner p {
  margin: 0;
  font-size: 15px;
  font-weight: 500;
  color: var(--dark-text);
}

.clean-fieldset {
  border: none;
  padding: 0;
  margin: 0;
}

@media (max-width: 640px) {
  .deleted-banner {
    flex-direction: column;
    text-align: center;
  }
}
</style>
