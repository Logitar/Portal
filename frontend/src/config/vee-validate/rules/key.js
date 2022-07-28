import { isLetterOrDigit } from '@/helpers/stringUtils'

export default function (value) {
  return typeof value === 'string' && [...value].every(c => isLetterOrDigit(c) || c === '_')
}
