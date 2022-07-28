import { isLetterOrDigit } from '@/helpers/stringUtils'

export default function (value) {
  return typeof value === 'string' && value.split('-').every(word => word.length && [...word].every(isLetterOrDigit))
}
