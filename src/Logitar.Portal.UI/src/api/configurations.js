import { post, put } from '.'

export async function initialize(payload) {
  return await post('/api/configurations', payload)
}

export async function updateConfiguration(payload) {
  return await put('/api/configurations', payload)
}
