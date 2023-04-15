import { extend } from 'vee-validate'
import { isLetter } from '@/helpers/stringUtils'

extend('purpose', {
  validate(value) {
    return typeof value === 'string' && value.split('_').every(word => word.length && [...word].every(isLetter))
  }
})
