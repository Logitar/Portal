import { extend } from 'vee-validate'
import { isLetterOrDigit } from '@/helpers/stringUtils'

extend('require_non_alphanumeric', {
  validate(value) {
    return typeof value === 'string' && [...value].some(c => !isLetterOrDigit(c))
  }
})
