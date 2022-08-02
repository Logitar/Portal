import { extend } from 'vee-validate'
import { isIdentifier } from '@/helpers/stringUtils'

extend('identifier', {
  validate(value) {
    return typeof value === 'string' && isIdentifier(value)
  }
})
