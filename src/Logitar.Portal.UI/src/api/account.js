import { post } from '.'

export async function signIn(payload) {
  return await post('/api/account/sign/in', payload)
}
