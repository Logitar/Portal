<template>
  <form-field
    :disabled="disabled || !custom"
    id="alias"
    label="realms.alias.label"
    :maxLength="validate ? 100 : null"
    placeholder="realms.alias.placeholder"
    :required="required"
    :rules="{ alias: validate }"
    :value="value"
    @input="$emit('input', $event)"
  >
    <template #after v-if="!disabled">
      <b-form-checkbox v-model="custom">{{ $t('realms.alias.custom') }}</b-form-checkbox>
    </template>
  </form-field>
</template>

<script>
import { isLetterOrDigit } from '@/helpers/stringUtils'

export default {
  name: 'AliasField',
  props: {
    disabled: {
      type: Boolean,
      default: false
    },
    name: {
      type: String,
      default: ''
    },
    required: {
      type: Boolean,
      default: false
    },
    validate: {
      type: Boolean,
      default: false
    },
    value: {}
  },
  data() {
    return {
      custom: false
    }
  },
  methods: {
    aliasify(value) {
      value = value ?? ''
      const words = []
      let word = ''
      for (let i = 0; i < value.length; i++) {
        const c = value[i]
        if (isLetterOrDigit(c)) {
          word += c
        } else if (word.length) {
          words.push(word)
          word = ''
        }
      }
      if (word.length) {
        words.push(word)
      }
      return words.join('-').toLowerCase()
    }
  },
  watch: {
    custom(custom) {
      if (custom) {
        this.$emit('input', '')
      } else {
        this.$emit('input', this.aliasify(this.name))
      }
    },
    name(name) {
      if (!this.custom) {
        this.$emit('input', this.aliasify(name))
      }
    }
  }
}
</script>
