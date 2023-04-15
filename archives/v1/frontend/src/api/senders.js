import { _delete, get, patch, post, put } from '.'
import { getQueryString } from '@/helpers/queryUtils'

export async function createSender(payload) {
  return await post('/api/senders', payload)
}

export async function deleteSender(id) {
  return await _delete(`/api/senders/${id}`)
}

export async function getSender(id) {
  return await get(`/api/senders/${id}`)
}

export async function getSenders(params) {
  return await get('/api/senders' + getQueryString(params))
}

export async function setDefault(id) {
  return await patch(`/api/senders/${id}/default`)
}

export async function updateSender(id, payload) {
  return await put(`/api/senders/${id}`, payload)
}
