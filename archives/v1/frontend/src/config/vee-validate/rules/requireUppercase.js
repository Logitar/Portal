import { extend } from 'vee-validate'
import { isLetter } from '@/helpers/stringUtils'

extend('require_uppercase', {
  validate(value) {
    return typeof value === 'string' && [...value].some(c => isLetter(c) && c === c.toUpperCase())
  }
})
