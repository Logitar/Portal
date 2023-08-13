import { extend } from 'vee-validate'
import { isLetterOrDigit } from '@/helpers/stringUtils'

extend('alias', {
  validate(value) {
    return typeof value === 'string' && value.split('-').every(word => word.length && [...word].every(isLetterOrDigit))
  }
})
