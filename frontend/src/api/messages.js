import { get } from '.'
import { getQueryString } from '@/helpers/queryUtils'

export async function getMessage(id) {
  return await get(`/api/messages/${id}`)
}

export async function getMessages(params) {
  return await get('/api/messages' + getQueryString(params))
}
