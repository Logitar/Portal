import { _delete, get, post, put } from '.'
import { getQueryString } from '@/helpers/queryUtils'

export async function createUser(payload) {
  return await post('/api/users', payload)
}

export async function deleteUser(id) {
  return await _delete(`/api/users/${id}`)
}

export async function getUser(id) {
  return await get(`/api/users/${id}`)
}

export async function getUsers(params) {
  return await get('/api/users' + getQueryString(params))
}

export async function updateUser(id, payload) {
  return await put(`/api/users/${id}`, payload)
}
