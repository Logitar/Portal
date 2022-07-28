import { post } from '.'

export async function changePassword(payload) {
  return await post('/api/account/password/change', payload)
}

export async function signIn(payload) {
  return await post('/api/account/sign/in', payload)
}
