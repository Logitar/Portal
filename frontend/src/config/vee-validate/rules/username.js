import { extend } from 'vee-validate'

const allowedUserNameCharacters = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+' // TODO(fpion): use Realm configuration

extend('alias', {
  validate(value) {
    return typeof value === 'string' && [...value].every(c => allowedUserNameCharacters.includes(c))
  }
})
