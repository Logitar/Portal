import { get } from '.'

export async function getTimeZones() {
  return await get('/api/timezones')
}
