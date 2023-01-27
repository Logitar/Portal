import { post } from '.'

export async function initialize(payload) {
  return await post('/api/configurations', payload)
}
