import { extend } from 'vee-validate'
import { isIdentifier } from '@/helpers/stringUtils'

extend('identifier', {
  validate: isIdentifier
})
