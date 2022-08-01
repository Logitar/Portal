import { extend } from 'vee-validate'
import { isDigit, isLetterOrDigit } from '@/helpers/stringUtils'

extend('key', {
  validate(value) {
    return typeof value === 'string' && [...value].every(c => isLetterOrDigit(c) || c === '_') && !isDigit(value[0])
  }
})
