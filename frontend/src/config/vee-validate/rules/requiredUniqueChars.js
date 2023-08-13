import { extend } from 'vee-validate'

extend('required_unique_chars', {
  validate(value, { count }) {
    if (typeof value !== 'string') {
      return false
    }
    const index = {}
    ;[...value].forEach(c => {
      if (index[c]) {
        index[c]++
      } else {
        index[c] = 1
      }
    })
    return Object.keys(index).length >= count
  },
  params: ['count']
})
