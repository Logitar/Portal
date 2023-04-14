import { post } from '.'

export async function consumeToken(payload) {
  return await post('/api/tokens/consume', payload)
}

export async function createToken(payload) {
  return await post('/api/tokens/create', payload)
}

export async function validateToken(payload) {
  return await post('/api/tokens/validate', payload)
}
