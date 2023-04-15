import { _delete, get, post, put } from '.'
import { getQueryString } from '@/helpers/queryUtils'

export async function createApiKey(payload) {
  return await post('/api/keys', payload)
}

export async function deleteApiKey(id) {
  return await _delete(`/api/keys/${id}`)
}

export async function getApiKey(id) {
  return await get(`/api/keys/${id}`)
}

export async function getApiKeys(params) {
  return await get('/api/keys' + getQueryString(params))
}

export async function updateApiKey(id, payload) {
  return await put(`/api/keys/${id}`, payload)
}
