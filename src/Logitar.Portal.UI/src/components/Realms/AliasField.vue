<template>
  <form-field
    :disabled="disabled || !custom"
    :id="id"
    :label="label"
    :maxLength="validate ? 100 : null"
    :placeholder="placeholder"
    :ref="id"
    :required="required"
    :rules="{ alias: validate }"
    :value="value"
    @input="$emit('input', $event)"
  >
    <template #after v-if="!disabled">
      <b-form-checkbox v-model="custom">{{ $t('realms.uniqueName.custom') }}</b-form-checkbox>
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
    id: {
      type: String,
      default: 'uniqueName'
    },
    label: {
      type: String,
      default: 'realms.uniqueName.label'
    },
    name: {
      type: String,
      default: ''
    },
    placeholder: {
      type: String,
      default: 'realms.uniqueName.placeholder'
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
    },
    customize() {
      this.custom = true
    },
    focus() {
      this.$refs[this.id].focus()
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
