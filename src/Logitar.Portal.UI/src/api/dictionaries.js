import { _delete, get, post, put } from '.'
import { getQueryString } from '@/helpers/queryUtils'

export async function createDictionary(payload) {
  return await post('/api/dictionaries', payload)
}

export async function deleteDictionary(id) {
  return await _delete(`/api/dictionaries/${id}`)
}

export async function getDictionary(id) {
  return await get(`/api/dictionaries/${id}`)
}

export async function getDictionaries(params) {
  return await get('/api/dictionaries' + getQueryString(params))
}

export async function updateDictionary(id, payload) {
  return await put(`/api/dictionaries/${id}`, payload)
}
