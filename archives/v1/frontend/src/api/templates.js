import { _delete, get, post, put } from '.'
import { getQueryString } from '@/helpers/queryUtils'

export async function createTemplate(payload) {
  return await post('/api/templates', payload)
}

export async function deleteTemplate(id) {
  return await _delete(`/api/templates/${id}`)
}

export async function getTemplate(id) {
  return await get(`/api/templates/${id}`)
}

export async function getTemplates(params) {
  return await get('/api/templates' + getQueryString(params))
}

export async function updateTemplate(id, payload) {
  return await put(`/api/templates/${id}`, payload)
}
