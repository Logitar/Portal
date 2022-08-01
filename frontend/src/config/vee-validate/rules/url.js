import { extend } from 'vee-validate'

extend('alias', {
  validate(value) {
    if (typeof value !== 'string') {
      return false
    }
    let url
    try {
      url = new URL(value)
    } catch (_) {
      return false
    }
    return url.protocol === 'http:' || url.protocol === 'https:'
  }
})
