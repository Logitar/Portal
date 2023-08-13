import { extend } from 'vee-validate'
import { isLetter } from '@/helpers/stringUtils'

extend('require_lowercase', {
  validate(value) {
    return typeof value === 'string' && [...value].some(c => isLetter(c) && c === c.toLowerCase())
  }
})
