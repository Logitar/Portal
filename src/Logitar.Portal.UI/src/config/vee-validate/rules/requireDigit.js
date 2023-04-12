import { extend } from 'vee-validate'
import { isDigit } from '@/helpers/stringUtils'

extend('require_digit', {
  validate(value) {
    return typeof value === 'string' && [...value].some(isDigit)
  }
})
