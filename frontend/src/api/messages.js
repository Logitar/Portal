import { get, post } from '.'
import { getQueryString } from '@/helpers/queryUtils'

export async function getMessage(id) {
  return await get(`/api/messages/${id}`)
}

export async function getMessages(params) {
  return await get('/api/messages' + getQueryString(params))
}

export async function sendDemoMessage(payload) {
  return await post('/api/messages/demo', payload)
}
