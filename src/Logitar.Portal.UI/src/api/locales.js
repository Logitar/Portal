import { get } from '.'

export async function getLocales() {
  return await get('/api/locales')
}
