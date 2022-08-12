import { post } from '.'

export async function createToken(payload) {
  return await post('/api/tokens/create', payload)
}

export async function validateToken(payload) {
  return await post('/api/tokens/validate', payload)
}
