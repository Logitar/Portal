import { _delete, get, post, put } from '.'
import { getQueryString } from '@/helpers/queryUtils'

export async function createRealm(payload) {
  return await post('/api/realms', payload)
}

export async function deleteRealm(id) {
  return await _delete(`/api/realms/${id}`)
}

export async function getRealm(id) {
  return await get(`/api/realms/${id}`)
}

export async function getRealms(params) {
  return await get('/api/realms' + getQueryString(params))
}

export async function updateRealm(id, payload) {
  return await put(`/api/realms/${id}`, payload)
}
